using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphQL.SchemaGenerator.Attributes;

namespace GraphQL.SchemaGenerator.Helpers
{
    public static class TypeHelper
    {
        public static string GetFullName(Type t)
        {
            if (!t.IsGenericType)
            {
                return StringHelper.SafeString(t.Name);
            }

            var sb = new StringBuilder();

            string typePrefix = StringHelper.SafeString(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            if (!typePrefix.EndsWith("Wrapper"))
            {
                sb.Append(typePrefix);
            }
            sb.Append(t.GetGenericArguments().Aggregate("_",
                (string aggregate, Type type) =>
                {
                    return StringHelper.SafeString(aggregate) + (aggregate == "_" ? "" : "_") + GetFullName(type);
                }
                ));

            return sb.ToString();
        }

        public static IEnumerable<GraphKnownTypeAttribute> GetGraphKnownTypes(Type graphType)
        {
            if (graphType != null)
            {
                foreach (GraphKnownTypeAttribute attr in graphType.GetCustomAttributes(typeof(GraphKnownTypeAttribute), true))
                {
                    yield return attr;
                }
            }
        }

        public static Type GetGraphType(FieldInfo field)
        {
            var graphTypeAttribute = field.GetCustomAttribute<GraphTypeAttribute>();
            return getGraphType(graphTypeAttribute, field.FieldType);
        }

        public static Type GetGraphType(PropertyInfo property)
        {
            var graphTypeAttribute = property.GetCustomAttribute<GraphTypeAttribute>();
            return getGraphType(graphTypeAttribute, property.PropertyType);
        }

        public static Type GetGraphType(MethodInfo method)
        {
            var graphTypeAttribute = method.GetCustomAttribute<GraphTypeAttribute>();
            return getGraphType(graphTypeAttribute, method.ReturnType);
        }

        public static Type GetGraphType(Type type)
        {
            var graphTypeAttribute = type.GetCustomAttribute<GraphTypeAttribute>();
            if (graphTypeAttribute != null)
            {
                return graphTypeAttribute.Type ?? type;
            }

            return null;
        }

        public static Type GetGraphType(ParameterInfo parameter)
        {
            return GetGraphType(parameter.ParameterType);
        }

        private static Type getGraphType(GraphTypeAttribute graphTypeAttribute, Type type)
        {
            if (graphTypeAttribute == null)
            {
                return type != null ? GetGraphType(type) : null;
            }

            return graphTypeAttribute.Type;
        }

        public static string GetDescription(PropertyInfo property)
        {
            return getDescriptionValue(property.GetCustomAttribute<DescriptionAttribute>());
        }

        public static string GetDescription(MethodInfo method)
        {
            return getDescriptionValue(method.GetCustomAttribute<DescriptionAttribute>());
        }

        public static string GetDescription(ParameterInfo parameter)
        {
            return getDescriptionValue(parameter.GetCustomAttribute<DescriptionAttribute>());
        }

        private static string getDescriptionValue(DescriptionAttribute desc)
        {
            if (desc != null)
            {
                return desc.Description;
            }

            return String.Empty;
        }

        public static string GetDeprecationReason(PropertyInfo property)
        {
            var obsoleteAttr = property.GetCustomAttribute<ObsoleteAttribute>();
            return GetObsoleteMessage(obsoleteAttr);
        }

        public static string GetDeprecationReason(FieldInfo field)
        {
            var obsoleteAttr = field.GetCustomAttribute<ObsoleteAttribute>();
            return GetObsoleteMessage(obsoleteAttr);
        }

        public static string GetDeprecationReason(MethodInfo method)
        {
            var obsoleteAttr = method.GetCustomAttribute<ObsoleteAttribute>();
            return GetObsoleteMessage(obsoleteAttr);
        }

        private static string GetObsoleteMessage(ObsoleteAttribute obsoleteAttr)
        {
                return obsoleteAttr?.Message;
        }

        public static object GetDefaultValue(PropertyInfo property)
        {
            var defaultValueAttr = property.GetCustomAttribute<DefaultValueAttribute>();
            return getDefaultValue(defaultValueAttr);
        }

        public static object GetDefaultValue(ParameterInfo parameter)
        {
            var defaultValueAttr = parameter.GetCustomAttribute<DefaultValueAttribute>();
            return getDefaultValue(defaultValueAttr);
        }

        private static object getDefaultValue(DefaultValueAttribute defaultValueAttr)
        {
            if (defaultValueAttr != null)
            {
                return defaultValueAttr.Value;
            }

            return null;
        }

        public static bool IsNotNull(MethodInfo method)
        {
            return method.GetCustomAttribute<NotNullAttribute>() != null;
        }

        public static bool IsNotNull(FieldInfo field)
        {
            return field.GetCustomAttribute<NotNullAttribute>() != null ||
                field.GetCustomAttribute<RequiredAttribute>() != null;
        }

        public static bool IsNotNull(PropertyInfo property)
        {
            return property.GetCustomAttribute<NotNullAttribute>() != null ||
                property.GetCustomAttribute<RequiredAttribute>() != null;
        }

        public static bool IsNotNull(ParameterInfo parameter)
        {
            return parameter.GetCustomAttribute<NotNullAttribute>() != null ||
                parameter.GetCustomAttribute<RequiredAttribute>() != null;
        }

        public static string GetDisplayName(Type type)
        {
            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null)
            {
                return StringHelper.SafeString(displayNameAttr.DisplayName);
            }

            return StringHelper.GetRealTypeName(type);
        }
    }
}
