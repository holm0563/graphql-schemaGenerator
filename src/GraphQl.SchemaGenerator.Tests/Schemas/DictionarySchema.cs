using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GraphQL.SchemaGenerator.Attributes;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    [GraphType]
    public class DictionarySchema
    {
        [GraphRoute]
        public IDictionary<string, string> DictionaryRequest(DictionaryRequest request)
        {
            return request.Dictionary;
        }
    }

    public class DictionaryRequest
    {
        public IDictionary<string, string> Dictionary { get; set; }
    }
}
