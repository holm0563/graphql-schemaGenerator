using System.ComponentModel;
using GraphQL.SchemaGenerator.Attributes;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    [GraphType]
    public class PrimitiveSchema
    {
        [Description(@"Tests a null response mutation")]
        [GraphRoute(IsMutation = true)]
        public void TestRequest(bool clear)
        {
            
        }

        [Description(@"Tests a string response mutation")]
        [GraphRoute]
        public string TestString()
        {
            return "Hi";
        }

    }
}
