using System;
using System.Collections.Generic;
using System.ComponentModel;
using GraphQL.SchemaGenerator.Attributes;
using GraphQL.StarWars;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    [GraphType]
    public class DateSchema
    {
        [GraphRoute]
        public Dates Dates(Dates dates)
        {
            return dates;
        }

    }

    public class Dates
    {
        public DateTimeOffset? Offset { get; set; }
        public DateTime? DateTime { get; set; }

    }
}
