﻿using System.Linq;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.SchemaGenerator.Tests.Helpers;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.SchemaGenerator.Tests.Schemas;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Xunit;

namespace GraphQL.SchemaGenerator.Tests.Tests
{
    public class AsyncSchemaTests
    {
        [Fact]
        public void CreateSchema_WithClassArgument_HasExpectedSchema()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var sut = schema.AllTypes;
            Assert.NotNull(sut.FirstOrDefault(t=>t.Name == "Input_Schema1Request"));
        }

        [Fact]
        public void BasicParameterExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest {value}
                }";

            var expected = @"{
              testRequest: {value:5}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithParameterExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest(request:{echo:1}) {value}
                }";

            var expected = @"{
              testRequest: {value:1}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithEnumerableParameterExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testEnumerableRequest(request:[{echo:1}]) {value}
                }";

            var expected = @"{
              testEnumerableRequest: {value:1}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithStringParameterExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest(request:{data:""yes""}) {stringValue}
                }";

            var expected = @"{
              testRequest: {stringValue:""yes""}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithStringEscapedParameterExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            //data = y\es m"am
            var query = @"{
                  testRequest(request:{data:""y\\es m\""am""}) {stringValue} 
                }";

            var expected = @"{
              testRequest: {stringValue:""y\\es m\""am""}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithComplexParameters_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest(request:{
                    complexRequests:[{
                            innerData:""345""
                        }]
                    }) {value}
                }";

            var expected = @"{
              testRequest: {value:5}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithComplexParameters_HaveCorrectType()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  __type(name : ""Input_InnerRequest"") {
                    name
                    kind
                }}";

            var expected = @"{
              __type: {
                name: ""Input_InnerRequest"",
                kind: ""INPUT_OBJECT""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithEnumerableExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testEnumerable{value}
                }";

            var expected = @"{
                  testEnumerable: [{value: 1},{value: 5}]
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithEnum_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest {enum}
                }";

            var expected = @"{
              testRequest: {enum:""NEWHOPE""}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithDateTimeOffset_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest {date}
                }";

            var expected = @"{
              testRequest: {date:""1999-01-01T00:00:00-07:00""}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void FieldDescription_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                     __schema{
                        types{
                          name,
                          fields {
                            name
                            description
                          }
                        }
                      }
                }";

            var exec = new DocumentExecuter(new GraphQLDocumentBuilder(), new DocumentValidator(), new ComplexityAnalyzer());
            var result = exec.ExecuteAsync(schema, null, query, null).Result;

            var writer = new DocumentWriter(indent: true);
            var writtenResult = writer.Write(result.Data);

            var errors = result.Errors?.FirstOrDefault();

            Assert.Null(errors?.Message);
            Assert.True(writtenResult.Contains("{VerifyComment}"));
        }

        [Fact]
        public void WithNull_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest {nullValue}
                }";

            var expected = @"{
              testRequest: {nullValue:null}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithDictionary_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(AsyncSchema));

            var query = @"{
                  testRequest {
                    values{
                        key
                        value{
                            complicatedResponse{
                                echo
                            }
                        }
                    }
                  }
                }";

            var expected = @"{
              testRequest: {
                values: [
                  {
                   key: ""99"",
                    value: {
                      complicatedResponse: {
                        echo: 99
                      }
            }
                  },
                  {
                    key: ""59"",
                    value: {
                      complicatedResponse: {
                        echo: 59
                      }
                    }
                  },
                  {
                    key: ""null"",
                    value: null
                  }
                ]
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }
    }
}
