## Schema Generator
The shema generator will automatically create a schema on existing c# models. This includes every response model, request model, and composed classes in these models. This can save a lot of time with an existing SDK or API project that is adding graph ql support.

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

Would be equivalent to:


```csharp
public class StarWarsSchema : Schema
{
  public StarWarsSchema()
  {
    Query = new StarWarsQuery();
  }
}

public class StarWarsQuery : ObjectGraphType
{
  public StarWarsQuery()
  {
    var data = new StarWarsData();
    Name = "Query";
    Field<CharacterInterface>(
      "hero",
      resolve: context => data.GetDroidById("3")
    );
  }
}

public class DroidType : ObjectGraphType
{
  public DroidType()
  {
    var data = new StarWarsData();
    Name = "Droid";
    Field<NonNullGraphType<StringGraphType>>("id", "The id of the droid.");
    Field<NonNullGraphType<StringGraphType>>("name", "The name of the droid.");
    Field<ListGraphType<CharacterInterface>>(
        "friends",
        resolve: context => data.GetFriends(context.Source as StarWarsCharacter)
    );
    IsTypeOf = value => value is Droid;
  }
}
```

