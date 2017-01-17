using System;
using System.Globalization;
using GraphQL.Language.AST;

namespace GraphQL.Types
{
    /// <summary>
    ///     Keep dates in the original format.
    /// </summary>
    public class OriginalDateGraphType : ScalarGraphType
    {
        public OriginalDateGraphType()
        {
            Name = "Date";
            Description =
                "The `Date` scalar type represents a timestamp provided in UTC. `Date` expects timestamps " +
                "to be formatted in accordance with the [ISO-8601](https://en.wikipedia.org/wiki/ISO_8601) standard.";
        }

        public override object Serialize(object value)
        {
            return ParseValue(value);
        }

        public override object ParseValue(object value)
        {
            if (value is DateTime)
            {
                return (DateTime) value;
            }

            var inputValue = value?.ToString().Trim('"');

            DateTime outputValue;
            if (DateTime.TryParse(
                inputValue,
                CultureInfo.CurrentCulture,
                DateTimeStyles.NoCurrentDateDefault,
                out outputValue))
            {
                return outputValue;
            }

            return null;
        }

        public override object ParseLiteral(IValue value)
        {
            var timeValue = value as DateTimeValue;
            if (timeValue != null)
            {
                return ParseValue(timeValue.Value);
            }

            var stringValue = value as StringValue;
            if (stringValue != null)
            {
                return ParseValue(stringValue.Value);
            }

            return null;
        }
    }
}