using System;
using System.Globalization;
using System.Threading;
using GraphQL.SchemaGenerator.Attributes;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    [GraphType]
    public class DateSchema
    {
        [GraphRoute]
        public Dates Dates(Dates dates)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            return dates;
        }

    }

    public class Dates
    {
        public DateTimeOffset? Offset { get; set; }
        public DateTime? DateTime { get; set; }

        public DateTime? DateUTC => DateTime.Value.ToUniversalTime();

        public DateTime? Y2k { get; } = new DateTime(2000, 1, 1);

        public DateTime? Y2kUtc { get; } = new DateTime(2000,1,1,0,0,0, DateTimeKind.Utc);

        public TimeSpan? TimeSpan { get; set; } = new TimeSpan(0,1,1,1);

    }
}
