using System;
using GraphQL.SchemaGenerator.Attributes;

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
