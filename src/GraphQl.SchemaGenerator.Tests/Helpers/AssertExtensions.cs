using System.Collections.Generic;
using System.Linq;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GraphQL.SchemaGenerator.Tests.Helpers
{
    public static class GraphAssert
    {
        public static void QuerySuccess(GraphQL.Types.Schema schema, string query, string expected, string variables = null, bool compareBoth = true)
        {
            var exec = new DocumentExecuter(new GraphQLDocumentBuilder(), new DocumentValidator(), new ComplexityAnalyzer());
            var result = exec.ExecuteAsync(schema, null, query, null, variables?.ToInputs()).Result;
            var result2 = DocumentOperations.ExecuteOperationsAsync(schema, null, query, variables?.ToInputs()).Result;

            var writtenResult = JsonConvert.SerializeObject(result.Data);
            var writtenResult2 = JsonConvert.SerializeObject(result2.Data);
            var queryResult = CreateQueryResult(expected);
            var expectedResult = JsonConvert.SerializeObject(queryResult.Data);

            var errors = result.Errors?.FirstOrDefault();
            var errors2 = result2.Errors?.FirstOrDefault();
            //for easy debugging
            var allTypes = schema.AllTypes;

            Assert.Null(errors?.Message);
            Assert.Null(errors2?.Message);
            Assert.Equal(expectedResult, writtenResult);
            Assert.Equal(expectedResult, writtenResult2);
        }

        public static void QueryOperationsSuccess(GraphQL.Types.Schema schema, string query, string expected, string variables = null, bool compareBoth = true, IList<string> blackListedOperations = null)
        {
            var result2 = DocumentOperations.ExecuteOperationsAsync(schema, null, query, variables?.ToInputs(), blackListedOperations: blackListedOperations).Result;

            var writtenResult2 = JsonConvert.SerializeObject(result2.Data);
            var queryResult = CreateQueryResult(expected);
            var expectedResult = JsonConvert.SerializeObject(queryResult.Data);

            var errors = result2.Errors?.FirstOrDefault();
            //for easy debugging
            var allTypes = schema.AllTypes;

            Assert.Null(errors?.Message);
            Assert.Equal(expectedResult, writtenResult2);
        }

        private static ExecutionResult CreateQueryResult(string result)
        {
            object expected = null;
            if (!string.IsNullOrWhiteSpace(result))
            {
                expected = JObject.Parse(result);
            }

            var eResult = new ExecutionResult { Data = expected };

            return eResult;
        }
    }
}
