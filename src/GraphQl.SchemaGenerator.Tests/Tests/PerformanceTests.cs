using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        public async Task LargeLists_Perform_UnderThreshold()
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

            var result = await DocumentOperations.ExecuteOperationsAsync(schema, null, query, validate:false);
            
            stopwatch.Stop();

            _output.WriteLine($"Total Milliseconds: {stopwatch.ElapsedMilliseconds}");

            Assert.True(stopwatch.Elapsed.TotalSeconds < 2);
            Assert.Null(result.Errors);
        }

        [Fact]
        public async Task Methods_Perform_Async()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());

            var schema = schemaGenerator.CreateSchema(typeof(PerformanceSchema));

            var query = @"{
                 slow1:slowCall{
                    date                   
                 }
                 slow2:slowCall{
                    date                   
                 }
                 slow3:slowCall{
                    date                   
                 }

            }";

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var result = await DocumentOperations.ExecuteOperationsAsync(schema, null, query, validate: false);

            stopwatch.Stop();

            _output.WriteLine($"Total Milliseconds: {stopwatch.ElapsedMilliseconds}");

            Assert.True(stopwatch.Elapsed.TotalSeconds < 2);
            Assert.Null(result.Errors);
        }

        private readonly ITestOutputHelper _output;

        public PerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }
    }
}
