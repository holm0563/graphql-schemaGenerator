﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.SchemaGenerator.Attributes;
using GraphQL.StarWars;
using GraphQL.StarWars.Types;

namespace GraphQL.SchemaGenerator.Tests
{
    [GraphType]
    public class SchemaEcho
    {
        [GraphRoute]
        public SchemaResponse TestRequest(Schema1Request request)
        {
            return new SchemaResponse
            {
                Value = request?.Echo ?? 5
            };
        }


        [GraphRoute]
        public IEnumerable<SchemaResponse> TestEnumerable(Schema1Request request)
        {
            return new List<SchemaResponse>
            {
                new SchemaResponse
                {
                    Value = 1
                },
                new SchemaResponse
                {
                    Value = request?.Echo ?? 5
                },
            };
        }
    }

    public class Schema1Request
    {
        public int? Echo { get; set; }
        public string Data { get; set; }
    }

    public class SchemaResponse
    {
        public Episode Enum { get; set; } = Episode.NEWHOPE;

        public int Value { get; set; }

        public int? NullValue { get; } = null;

        public Dictionary<string, Response2> Values { get; set; } = new Dictionary<string, Response2>
        {
            {"99", new Response2 {ComplicatedResponse = new Schema1Request {Data = "99", Echo = 99} } },
            {"59", new Response2 {ComplicatedResponse = new Schema1Request {Data = "59", Echo = 59} } },
            {"null", null}
        };
    }

    public class Response2
    {
        public Schema1Request ComplicatedResponse { get; set; } 
    }
}