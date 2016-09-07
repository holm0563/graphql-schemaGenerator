using System;
using System.Collections.Generic;
using System.ComponentModel;
using GraphQL.SchemaGenerator.Attributes;
using GraphQL.StarWars;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    [GraphType]
    public class DuplicateSchema
    {
        [Description(@"Tests error handling.")]
        [GraphRoute(name:"SameRoute")]
        public DuplicateResponse TestRequest()
        {
            return new DuplicateResponse
            {
                Value = 9
            };
        }

        [Description(@"Tests error handling.")]
        [GraphRoute(name: "SameRoute")]
        public DuplicateResponse TestRequest2()
        {
            return new DuplicateResponse
            {
                Value = 2
            };
        }
    }

    public class DuplicateResponse
    {
        public int Value { get; set; }

    }
}
