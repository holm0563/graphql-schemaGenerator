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

        public DateTime? Y2k { get; } = new DateTime(2000, 1, 1);

        public DateTime? Y2kUtc { get; } = new DateTime(2000,1,1).ToUniversalTime();

    }
}
