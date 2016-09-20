using System;
using System.Collections.Generic;
using GraphQL.Types;

namespace GraphQL.SchemaGenerator.Wrappers
{
    /// <summary>
    /// Note: the iobjectgraphtype is only used to get graph introspection to work. This is a work around until the bug is fixed.
    /// see: https://github.com/graphql-dotnet/graphql-dotnet/issues/183
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputObjectGraphTypeWrapper<T> : InputObjectGraphType, IIgnore, IObjectGraphType
    {
        public InputObjectGraphTypeWrapper()
        {
            ObjectGraphTypeBuilder.Build(this, typeof(T));
            Name = "Input_" + Name;
        }

        public IEnumerable<Type> Interfaces { get; } = new List<Type>();

        public Func<object, bool> IsTypeOf { get; set; }
    }
}
