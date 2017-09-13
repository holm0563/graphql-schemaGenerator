using System.Collections.Generic;
using System.Linq;
using GraphQL.SchemaGenerator.Helpers;
using GraphQL.SchemaGenerator.Tests.Schemas;
using Xunit;

namespace GraphQL.SchemaGenerator.Tests.Tests
{
    public class TypeHelperTests
    {
        [Fact]
        public void GetFullName_With_NestedDictionary_IsSafe()
        {
            var data = new SchemaResponse()
            {
                NestDictionary = new Dictionary<string, IDictionary<string, Episode>>()
            };
            var type = data.NestDictionary.GetType();

            var sut = TypeHelper.GetFullName(type);

            var prohibitedCharacters = new List<char> {'~', ' ', ','};

            Assert.NotNull(sut);
            Assert.True(!sut.Any(t=> prohibitedCharacters.Contains(t)));
        }
    }
}
