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
    public class GenericQueryTests
    {
        [Fact]
        public void BasicExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(GenericsSchema));

            var query = @"
                echoGenerics {
                    echoGenerics{data}
                }
            ";

            var expected = @"{
              echoGenerics: {
                data: """"
              }
            }";

           GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void BasicInputExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(GenericsSchema));

            var query = @"
                echoGenerics {
                    echoGenerics(
                        int1:{data:2}
                        string1:{data:""test""},
                    )
                    {data}
                }
            ";

            var expected = @"{
              echoGenerics: {
                data: ""2test""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void BasicClassInputExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(GenericsSchema));

            var query = @"
                {
                    echoClassGenerics{
                        list{innerInt}
                    }
                    echoClassGenerics2{
                        list{inner2Int}
                    }
                }
            ";

            var expected = @"{
              echoClassGenerics: {
                list: [{innerInt:1}]
              },
             echoClassGenerics2: {
                list: [{inner2Int:2}]
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void Schema_HasUniqueTypes()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(GenericsSchema));

            var query = @"
                {
                    __schema{
                        queryType {
                          name,
                          fields{
                            name
                            type{
                              name
                              kind
                            }
                          }
                        }
                      }
                }
            ";

            var expected = @"{
              __schema: {
                queryType: {
                            name: ""RootQueries"",
                  fields: [
                    {
                      name: ""echoGenerics"",
                      type: {
                        name: ""EchoGeneric__String"",
                        kind: ""OBJECT""
                      }
            },
                    {
                      name: ""echoClassGenerics"",
                      type: {
                        name: ""EchoGenericList__Inner"",
                        kind: ""OBJECT""
                      }
                    },
                    {
                      name: ""echoClassGenerics2"",
                      type: {
                        name: ""EchoGenericList__Inner2"",
                        kind: ""OBJECT""
                      }
                    }
                  ]
                }
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }
    }
}
