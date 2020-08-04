using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;

namespace GraphQL.SchemaGenerator
{
    /// <summary>
    ///     Handles multiple operations.
    /// </summary>
    public static class DocumentOperations
    {
        private static ComplexityAnalyzer ComplexityAnalyzerInstance { get; } = new ComplexityAnalyzer();
        private static DocumentValidator DocumentValidator { get; } = new DocumentValidator();

        /// <summary>
        ///     Execute all operations async.
        /// </summary>
        /// <returns>Aggregated results.</returns>
        public static async Task<ExecutionResult> ExecuteOperationsAsync(
            ISchema schema,
            object root,
            string query,
            Inputs inputs = null,
            CancellationToken cancellationToken = default(CancellationToken),
            IEnumerable<IValidationRule> rules = null,
            bool validate = true,
            IDocumentBuilder documentBuilder = null,
            int? maxOperationNodes = null,
            int? maxTasksAllowed = null,
            IList<string> blackListedOperations = null
        )
        {
            var savedDocument = new SavedDocumentBuilder(query, documentBuilder);

            if (savedDocument.OperationNodes > maxOperationNodes)
            {
                throw new InvalidOperationException($"Graph query contains more than the allowed operation limit ({maxOperationNodes}) for one request.");
            }

            if (blackListedOperations != null && blackListedOperations.Any())
            {
                var selections = savedDocument.Document.Operations.SelectMany(i => i.SelectionSet.Selections);
                foreach (var selection in selections)
                {

                    if (selection != null)
                    {
                        var name = (selection as Field)?.Name;
                        if (blackListedOperations.Contains(name))
                        {
                            throw new InvalidOperationException($"Graph query contains a restricted operation '{name}'.");
                        }
                    }
                }
            }

            if (savedDocument.Document.Operations == null || savedDocument.Document.Operations.Count() <= 1)
            {
                //run the typical way.
                var defaultBuilder = new DocumentExecuter(savedDocument, DocumentValidator, ComplexityAnalyzerInstance, maxTasksAllowed);

                return
                    await
                        defaultBuilder.ExecuteAsync(
                            new ExecutionOptions
                            {
                                Schema = schema,
                                Root = root,
                                Query = query,
                                Inputs = inputs,
                                CancellationToken = cancellationToken,
                                ValidationRules = rules,
                                EnableDocumentValidation = validate,
                                EnableMetrics = false,
                                SetFieldMiddleware = false
                            });
            }

            var result = new ExecutionResult();
            var nonValidatedExecutionar =
                new DocumentExecuter(savedDocument, DocumentValidator, ComplexityAnalyzerInstance, maxTasksAllowed);
            var aggregateData = new Dictionary<string, object>();

            try
            {
                if (validate)
                {
                    var validationResult = DocumentValidator.Validate(query, schema, savedDocument.Document, rules);

                    if (!validationResult.IsValid)
                    {
                        result.Data = aggregateData;
                        result.Errors = validationResult.Errors;

                        return result;
                    }
                }

                foreach (var operation in savedDocument.Document.Operations)
                {
                    var opResult =
                        await nonValidatedExecutionar.ExecuteAsync(new ExecutionOptions
                        {
                            Schema = schema,
                            Root = root,
                            Query = query,
                            Inputs = inputs,
                            CancellationToken = cancellationToken,
                            ValidationRules = rules,
                            OperationName = operation.Name,
                            EnableDocumentValidation = false,
                            EnableMetrics = false,
                            SetFieldMiddleware = false
                        });

                    if (opResult.Errors != null && opResult.Errors.Any())
                        return opResult;

                    aggregateData.Add(operation.Name, opResult.Data);
                }
            }
            catch (Exception exc)
            {
                if (result.Errors == null)
                    result.Errors = new ExecutionErrors();

                result.Data = null;
                result.Errors.Add(new ExecutionError(exc.Message, exc));
                return result;
            }

            result.Data = aggregateData;

            return result;
        }
    }


    /// <summary>
    ///     Caches a saved document for reuse.
    /// </summary>
    public class SavedDocumentBuilder : IDocumentBuilder
    {
        public SavedDocumentBuilder(string query, IDocumentBuilder builder)
        {
            builder = builder ?? new GraphQLDocumentBuilder();

            Document = builder.Build(query);
        }

        /// <summary>
        ///     The saved document.
        /// </summary>
        public Document Document { get; }

        public Document Build(string body)
        {
            return Document;
        }

        public int OperationNodes
        {
            get
            {
                if (Document?.Operations == null)
                {
                    return 0;
                }

                var count = Document.Operations.Sum(t => t.SelectionSet.Selections.Count());

                return count;
            }
        }
    }
}
