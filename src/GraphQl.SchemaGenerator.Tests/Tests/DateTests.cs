using System.Collections.Generic;
using GraphQL.SchemaGenerator.Helpers;
using GraphQL.SchemaGenerator.Tests.Helpers;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.SchemaGenerator.Tests.Schemas;
using GraphQL.StarWars;
using Xunit;

namespace GraphQL.SchemaGenerator.Tests.Tests
{
    public class DateTests
    {
        [Fact]
        public void GetOffset_IsSafe()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(DateSchema));

            var query = @"
                {dates(dates:{offset:""5/1/2012""}) {
                  offset
                }}
            ";

            var expected = @"{
              dates: {
                offset:""2012-05-01T06:00:00Z""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }
    }
}
