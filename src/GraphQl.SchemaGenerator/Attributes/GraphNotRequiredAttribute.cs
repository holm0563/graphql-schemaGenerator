using System;
using GraphQL.SchemaGenerator.Models;

namespace GraphQL.SchemaGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
    public class GraphNotRequiredAttribute : Attribute
    {
        public RequiredType Type { get; set; }

        public GraphNotRequiredAttribute(RequiredType type = RequiredType.NotRequired)
        {
            Type = type;
        }
    }
}
