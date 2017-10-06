using System;
using System.Collections.Generic;
using System.Threading;
using GraphQL.SchemaGenerator.Attributes;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    [GraphType]
    public class PerformanceSchema
    {
        private readonly List<SchemaResponse> largeList;

        public PerformanceSchema()
        {
            largeList = new List<SchemaResponse>();

            for (var x = 0; x < 10000; x++)
                largeList.Add(new SchemaResponse
                {
                    Date = DateTimeOffset.Now
                });
        }

        [GraphRoute]
        public List<SchemaResponse> TestList()
        {
            return largeList;
        }

        [GraphRoute]
        public List<SchemaResponse> SlowCall()
        {
            Thread.Sleep(1000);
            return new List<SchemaResponse>();
        }
    }
}
