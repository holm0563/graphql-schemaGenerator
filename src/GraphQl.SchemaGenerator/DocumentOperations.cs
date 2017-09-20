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
            IDocumentBuilder documentBuilder = null
            )
        {
            var savedDocument = new SavedDocumentBuilder(query, documentBuilder);
            var analyzer = new ComplexityAnalyzer();
            var validator = validate ? (IDocumentValidator)new DocumentValidator() : new ValidValidator();

            if ((savedDocument.Document.Operations == null) || (savedDocument.Document.Operations.Count() <= 1))
            {
                //run the typical way.
                var defaultBuilder = new DocumentExecuter(savedDocument, validator, analyzer);

                return
                    await
                        defaultBuilder.ExecuteAsync(schema, root, query, null, inputs, rules: rules,
                            cancellationToken: cancellationToken);
            }

            var result = new ExecutionResult();
            var nonValidatedExecutionar = new DocumentExecuter(savedDocument, new ValidValidator(), analyzer);
            var aggregateData = new Dictionary<string, object>();

            try
            {
                var validationResult = validator.Validate(query, schema, savedDocument.Document, rules);

                if (validationResult.IsValid)
                {
                    foreach (var operation in savedDocument.Document.Operations)
                    {
                        var opResult =
                            await nonValidatedExecutionar.ExecuteAsync(schema, root, query, operation.Name, inputs,
                                rules: rules, cancellationToken: cancellationToken);

                        if ((opResult.Errors != null) && opResult.Errors.Any())
                            return opResult;

                        aggregateData.Add(operation.Name, opResult.Data);
                    }
                }
                else
                {
                    result.Data = null;
                    result.Errors = validationResult.Errors;
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
    ///     Skips validation and returns true.
    /// </summary>
    public class ValidValidator : IDocumentValidator
    {
        public IValidationResult Validate(string originalQuery, ISchema schema, Document document,
            IEnumerable<IValidationRule> rules = null,
            object userContext = null)
        {
            return new ValidationResult();
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
    }
}
