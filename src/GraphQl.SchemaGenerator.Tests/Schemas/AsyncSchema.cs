using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.SchemaGenerator.Attributes;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    [GraphType]
    public class AsyncSchema
    {
        [Description(@"Tests a variety or request and response types.{VerifyComment}")]
        [GraphRoute]
        public Task<SchemaResponse> TestRequest(Schema1Request request)
        {
            var schema = new EchoSchema();
            return Task.FromResult(schema.TestRequest(request));
        }

        [GraphRoute]
        public async Task<SchemaResponse> TestEnumerableRequest(IEnumerable<Schema1Request> request)
        {
            var schema = new EchoSchema();
            return await Task.FromResult(schema.TestEnumerableRequest(request));
        }


        [GraphRoute]
        public async Task<IEnumerable<SchemaResponse>> TestEnumerable(Schema1Request request)
        {
            var schema = new EchoSchema();
            return await Task.FromResult(schema.TestEnumerable(request));
        }

        [GraphRoute]
        public async Task NotRecommendedToReturnATask()
        {
            await Task.Delay(1);
        }

    }

  
}
