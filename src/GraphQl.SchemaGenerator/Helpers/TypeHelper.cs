using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphQL.SchemaGenerator.Attributes;
using GraphQL.SchemaGenerator.Extensions;
using GraphQL.SchemaGenerator.Models;

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

        public static RequiredType IsNotNull(MethodInfo method)
        {
            var explicitSetting = method.GetCustomAttribute<GraphNotRequiredAttribute>();
            if (explicitSetting != null)
            {
                return explicitSetting.Type;
            }

            if (method.GetCustomAttribute<NotNullAttribute>() != null ||
                method.GetCustomAttribute<RequiredAttribute>() != null)
            {
                return RequiredType.Required;
            }

            return RequiredType.Default;
        }

        public static RequiredType IsNotNull(FieldInfo field)
        {
            var explicitSetting = field.GetCustomAttribute<GraphNotRequiredAttribute>();
            if (explicitSetting != null)
            {
                return explicitSetting.Type;
            }

            if (field.GetCustomAttribute<NotNullAttribute>() != null ||
                (field.GetCustomAttribute<RequiredAttribute>() != null &&
                 ShouldBeNotNullWithRequiredAttribute(field.FieldType)))
            {
                return RequiredType.Required;
            }

            return RequiredType.Default;
        }

        public static RequiredType IsNotNull(PropertyInfo property)
        {
            var explicitSetting = property.GetCustomAttribute<GraphNotRequiredAttribute>();
            if (explicitSetting != null)
            {
                return explicitSetting.Type;
            }

            if (property.GetCustomAttribute<NotNullAttribute>() != null ||
                (property.GetCustomAttribute<RequiredAttribute>() != null &&
                 ShouldBeNotNullWithRequiredAttribute(property.PropertyType)))
            {
                return RequiredType.Required;
            }

            return RequiredType.Default;
        }

        public static bool ShouldBeNotNullWithRequiredAttribute(Type type)
        {
            if (type.IsAssignableToGenericType(typeof(Nullable<>)))
            {
                return false;
            }

            if (type.IsAssignableToGenericType(typeof(IDictionary<,>)))
            {
                return false;
            }

            if (type.IsAssignableToGenericType(typeof(IEnumerable<>)))
            {
                return false;
            }

            return true;
        }

        public static RequiredType IsNotNull(ParameterInfo parameter)
        {
            var explicitSetting = parameter.GetCustomAttribute<GraphNotRequiredAttribute>();
            if (explicitSetting != null)
            {
                return explicitSetting.Type;
            }

            if (parameter.GetCustomAttribute<NotNullAttribute>() != null ||
                (parameter.GetCustomAttribute<RequiredAttribute>() != null &&
                 !parameter.ParameterType.IsAssignableToGenericType(typeof(Nullable<>))))
            {
                return RequiredType.Required;
            }

            return RequiredType.Default;
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
