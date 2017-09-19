using System;
using System.Diagnostics;
using GraphQL.Execution;
using GraphQL.SchemaGenerator.Tests.Helpers;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.SchemaGenerator.Tests.Schemas;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Xunit;
using Xunit.Abstractions;

namespace GraphQL.SchemaGenerator.Tests.Tests
{
    public class PerformanceTests
    {
        [Fact]
        public void LargeLists_Perform_UnderThreshold()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());

            var schema = schemaGenerator.CreateSchema(typeof(PerformanceSchema));

            var query = @"{
                  testList{
                    date
                    enum
                    value
                    nullValue
                    decimalValue
                    timeSpan
                    byteArray
                    stringValue
                    values{
                        value{
                            complicatedResponse{
                            echo
                            data
                            }
                        }
                    }
                 }
            }";

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var exec = new DocumentExecuter(new GraphQLDocumentBuilder(), new DocumentValidator(),
                new ComplexityAnalyzer());
            var result = exec.ExecuteAsync(schema, null, query, null).Result;

            stopwatch.Stop();

            _output.WriteLine($"Total Milliseconds: {stopwatch.ElapsedMilliseconds}");

            Assert.True(stopwatch.Elapsed.TotalSeconds < 1);
            Assert.Empty(result.Errors);
        }

        private readonly ITestOutputHelper _output;

        public PerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }
    }
}
