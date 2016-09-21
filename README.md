# Schema Generator - GraphQL for .NET

This uses [GraphQL for .NET](https://github.com/graphql-dotnet/graphql-dotnet) and wraps it to easily generate a graph ql schema based on c# models. The shema generator will automatically create a schema on existing c# models. This includes every response model, request model, and composed classes in these models. This can save a lot of time with an existing SDK or API project that is adding graph ql support.

## Installation

*Todo

## Configuration

Define your routes with the GraphRoute attribute:

```
/// <summary>
///     An example of the sdk that could be exposed. This is decorated with attributes to self generate a graph schema. 
/// </summary>
public class StarWarsAttributeSchema
{
    private readonly StarWarsData _data = new StarWarsData();

    /// <summary>
    ///     Get the current hero.
    /// </summary>
    /// <remarks>
    ///     Example of graph ql attribute using the defaults.
    /// </remarks>
    /// <returns>Droid.</returns>
    [GraphRoute]
    public Droid Hero()
    {
        var item = _data.GetDroidByIdAsync("3").Result;

        return item;
    }
}
```

## Example Usage

```

IServiceProvider provider = new MockServiceProvider(); //Resolves your classes
var schemaGenerator = new SchemaGenerator(provider);
//See the readme.md in the schema generator project for more details on what this is doing.
var schema = schemaGenerator.CreateSchema(typeof(StarWarsAttributeSchema));

//Standard graph ql execution
 var query = @"
                query HeroNameQuery {
                  hero {
                    name
                  }
                }
            ";
var exec = new DocumentExecuter(new AntlrDocumentBuilder(), new DocumentValidator());
var result = exec.ExecuteAsync(schema, null, query, null).Result;

```

## Roadmap

### Supported Data Types
- [x] Enums
- [x] Dictionaries
- [x] IEnumerable
- [x] DateTime, DateTimeOffset
- [x] Timespan
- [x] Byte Array
- [x] Key value pair

### Supported Conversions
- [x] Mutations
- [x] Queries
- [ ] Interfaces
- [x] Descriptions (via description attribute)
- [ ] Descriptions (via summary text)
- [x] Enumerations
- [x] Input Objects
- [x] Mutations
- [ ] Unions
- [ ] Async execution
- Void return types are not supported, doesn't make sense per the graph spec.

