using GraphQL.SchemaGenerator.Tests.Helpers;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.SchemaGenerator.Tests.Schemas;
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
                {dates(dates:{offset:""2013-07-02T09:00:00"", dateTime:""2013-07-02T09:00""}) {
                  offset
                  dateTime
                }}
            ";

            var expected = @"{
              dates: {
                offset:""2013-07-02T09:00:00-06:00"",
                dateTime:""2013-07-02T09:00:00""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void GetUtcDates_IsSafe()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(DateSchema));

            var query = @"
                {dates(dates:{offset:""2013-07-02T09:00:00Z"", dateTime:""2013-07-02T09:00Z""}) {
                  y2k
                  y2kUtc
                }}
            ";

            var expected = @"{
              dates: {
                y2k:""2000-01-01T00:00:00"",
                y2kUtc:""2000-01-01T07:00:00Z""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void GetZuluOffset_IsSafe()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(DateSchema));

            var query = @"
                {dates(dates:{offset:""2013-07-02T09:00:00Z"", dateTime:""2013-07-02T09:00Z""}) {
                  offset
                  dateTime
                }}
            ";

            var expected = @"{
              dates: {
                offset:""2013-07-02T03:00:00-06:00"",
                dateTime:""2013-07-02T03:00:00-06:00""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }
    }
}