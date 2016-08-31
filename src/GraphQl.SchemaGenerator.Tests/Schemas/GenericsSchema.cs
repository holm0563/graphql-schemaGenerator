using System.Collections.Generic;
using System.ComponentModel;
using GraphQL.SchemaGenerator.Attributes;

namespace GraphQL.SchemaGenerator.Tests.Schemas
{
    public class GenericsSchema
    {
        [Description(@"Tests a multiple types")]
        [GraphRoute]
        public EchoGeneric<string> EchoGenerics(EchoGeneric<int> int1, EchoGeneric<int> int2,
            EchoGeneric<string> string1)
        {
            return new EchoGeneric<string>
            {
                data = $"{int1?.data}{int2?.data}{string1?.data}"
            };
        }

        [Description(@"Tests int types")]
        [GraphRoute]
        public EchoGenericList<Inner> EchoClassGenerics()
        {
            return new EchoGenericList<Inner>(new List<Inner>
                {
                    new Inner
                    {
                        InnerInt = 1,
                        InnerString = "Hi"
                    }
                }
            );
        }

        [Description(@"Tests int types")]
        [GraphRoute]
        public EchoGenericList<Inner2> EchoClassGenerics2()
        {
            return new EchoGenericList<Inner2>(new List<Inner2>
                {
                    new Inner2
                    {
                        Inner2Int = 2,
                        Inner2String = "Bye"
                    }
                }
            );
        }
    }

    public class EchoGeneric<T>
    {
        public T data { get; set; }
    }

    public class EchoGenericList<T> : EchoGeneric<T> where T : class
    {
        public EchoGenericList(IEnumerable<T> values)
        {
            list = values;
        }

        public IEnumerable<T> list { get; }
    }

    public class Inner
    {
        public string InnerString { get; set; }
        public int InnerInt { get; set; }
    }

    public class Inner2
    {
        public string Inner2String { get; set; }
        public int Inner2Int { get; set; }
    }
}