using System;
using System.Linq;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.SchemaGenerator.Tests.Helpers;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.SchemaGenerator.Tests.Schemas;
using GraphQL.StarWars;
using GraphQL.StarWars.IoC;
using GraphQL.StarWars.Types;
using GraphQL.Types;
using GraphQL.Validation;
using Xunit;

namespace GraphQL.SchemaGenerator.Tests.Tests
{
    public class ErrorTests
    {
        [Fact]
        public void Duplicate_Name_Throws()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            Exception e = null;
            try
            {
                schemaGenerator.CreateSchema(typeof(DuplicateSchema));
            }
            catch (Exception er)
            {
                e = er;
            }

            Assert.NotNull(e);
            Assert.Contains("SameRoute", e.Message);
        }
    }
}
