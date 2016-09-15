using System;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.SchemaGenerator.Tests.Schemas;
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
            //this should pass whenever the change is pushed.
            Assert.NotNull(e);
            Assert.Contains("SameRoute", e.Message);
        }
    }
}
