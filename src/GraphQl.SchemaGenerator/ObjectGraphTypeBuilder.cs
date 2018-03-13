using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.SchemaGenerator.Attributes;
using GraphQL.SchemaGenerator.Extensions;
using GraphQL.SchemaGenerator.Helpers;
using GraphQL.Types;

namespace GraphQL.SchemaGenerator
{
    public static class ObjectGraphTypeBuilder
    {
        public static void Build(ObjectGraphType graphType, Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                throw new InvalidOperationException("type must not be an abstract type or an interface");
            }

            ProcessObjectType(graphType, type);

            bool hasDataContract = type.ShouldIncludeInGraph();

            // KnownTypeAttribute could be used when SchemaType and DomainType are the same
            ProcessType(graphType, type);
            ProcessProperties(graphType, GetProperties(hasDataContract, type));
            ProcessFields(graphType, GetFields(hasDataContract, type));
            ProcessMethods(graphType, type, type.GetMethods());
        }

        public static void Build(InterfaceGraphType graphType, Type type)
        {
            if (!type.IsInterface && !type.IsAbstract)
            {
                throw new InvalidOperationException("type must be an abstract type or an interface");
            }

            ProcessInterfaceType(graphType, type);

            bool hasDataContract = type.ShouldIncludeInGraph();

            // KnownTypeAttribute could be used when SchemaType and DomainType are the same
            ProcessType(graphType, type);
            ProcessProperties(graphType, GetProperties(hasDataContract, type));
            ProcessFields(graphType, GetFields(hasDataContract, type));
            ProcessMethods(graphType, type, type.GetMethods());
        }

        public static void Build(InputObjectGraphType graphType, Type type)
        {
            ProcessType(graphType, type);
            bool hasDataContract = type.ShouldIncludeInGraph();
            ProcessProperties(graphType, GetProperties(hasDataContract, type), true);
            ProcessFields(graphType, GetFields(hasDataContract, type), true);
            ProcessMethods(graphType, type, type.GetMethods());
        }

        private static IEnumerable<PropertyInfo> GetProperties(bool hasDataContract, Type type)
        {
            var properties = type.GetProperties();
            if (hasDataContract)
            {
                return properties.Where(p => p.ShouldIncludeMemberInGraph());
            }
            else
            {
                return properties;
            }
        }

        private static IEnumerable<FieldInfo> GetFields(bool hasDataContract, Type type)
        {
            var fields = type.GetFields();
            if (hasDataContract)
            {
                return fields.Where(f => f.ShouldIncludeMemberInGraph());
            }
            else
            {
                return fields;
            }
        }

        private static void ProcessInterfaceType(InterfaceGraphType interfaceGraphType, Type type)
        {
            interfaceGraphType.ResolveType = CreateResolveType(type);
        }

        private static void ProcessObjectType(ObjectGraphType objectGraphType, Type type)
        {
            var interfaces = new List<Type>();
            foreach (var @interface in type.GetInterfaces())
            {
                if (!IsGraphType(@interface))
                {
                    continue;
                }
                interfaces.Add(GraphTypeConverter.ConvertTypeToGraphType(type));
            }

            objectGraphType.Interfaces = interfaces;
        }

        private static bool IsGraphType(Type @interface)
        {
            return TypeHelper.GetGraphType(@interface) != null ||
                @interface.ShouldIncludeInGraph();
        }

        private static void ProcessType(GraphType graphType, Type type)
        {
            graphType.Name = TypeHelper.GetDisplayName(type);
            
            var descAttr = type.GetCustomAttribute<DescriptionAttribute>();
            if (descAttr != null)
            {
                graphType.Description = descAttr.Description;
            }
            // explicit - include with DataMember, implicit - exclude with GraphIgnore            
        }

        private static Func<object, ObjectGraphType> CreateResolveType(Type type)
        {
            var expressions = new List<Expression>();
            var knownTypes = TypeHelper.GetGraphKnownTypes(type);

            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var returnLabel = Expression.Label(typeof(ObjectGraphType));

            foreach (var knownType in knownTypes)
            {
                var graphType = GraphTypeConverter.ConvertTypeToGraphType(knownType.SchemaType);
                var lookup = Expression.IfThen(
                    Expression.TypeIs(instanceParam, knownType.DomainType),
                    Expression.Return(returnLabel, Expression.Convert(Expression.New(graphType), typeof(ObjectGraphType)))
                );

                expressions.Add(lookup);
            }

            var result = Expression.Convert(Expression.Constant(null), typeof(ObjectGraphType));
            expressions.Add(Expression.Label(returnLabel, result));
            var body = Expression.Block(expressions);

            return Expression.Lambda<Func<object, ObjectGraphType>>(
                body,
                instanceParam).Compile();
        }

        private static void ProcessProperties(IComplexGraphType graphType, IEnumerable<PropertyInfo> properties, bool isInputType = false)
        {
            foreach (var property in properties.OrderBy(p => p.Name))
            {
                var required = TypeHelper.IsNotNull(property);

                var propertyGraphType = TypeHelper.GetGraphType(property);
                if (propertyGraphType != null)
                {
                    propertyGraphType = GraphTypeConverter.ConvertTypeToGraphType(propertyGraphType, required, isInputType);
                    propertyGraphType = EnsureList(property.PropertyType, propertyGraphType);
                }
                else
                {
                    propertyGraphType = GraphTypeConverter.ConvertTypeToGraphType(property.PropertyType, required, isInputType);
                }

                var name = StringHelper.GraphName(property.Name);
                var field = graphType.AddField(new FieldType {
                    Type=propertyGraphType,
                    Name = name,
                    Description = TypeHelper.GetDescription(property)
                });

                field.DefaultValue = TypeHelper.GetDefaultValue(property);
                field.DeprecationReason = TypeHelper.GetDeprecationReason(property);
            }
        }

        private static void ProcessFields(IComplexGraphType graphType, IEnumerable<FieldInfo> fields, bool isInputType = false)
        {
            foreach (var field in fields.OrderBy(f => f.Name))
            {
                var required = TypeHelper.IsNotNull(field);

                var fieldGraphType = TypeHelper.GetGraphType(field);
                if (fieldGraphType != null)
                {
                    fieldGraphType = GraphTypeConverter.ConvertTypeToGraphType(fieldGraphType, required, isInputType);
                    fieldGraphType = EnsureList(field.FieldType, fieldGraphType);
                }
                else
                {
                    fieldGraphType = GraphTypeConverter.ConvertTypeToGraphType(field.FieldType, required, isInputType);
                }

                var addedField = graphType.AddField(new FieldType { 
                    Type = fieldGraphType,
                    Name = StringHelper.GraphName(field.Name)
                });

                addedField.DeprecationReason = TypeHelper.GetDeprecationReason(field);

            }
        }

        private static void ProcessMethods(IComplexGraphType graphType, Type type, IEnumerable<MethodInfo> methods)
        {
            if (!typeof(GraphType).IsAssignableFrom(type) &&
                !type.IsDefined(typeof(GraphTypeAttribute)))
            {
                return;
            }
            foreach (var method in methods.OrderBy(m => m.Name))
            {
                if (IsSpecialMethod(method))
                {
                    continue;
                }

                var required = TypeHelper.IsNotNull(method);
                var returnGraphType = TypeHelper.GetGraphType(method);
                var methodGraphType = returnGraphType;
                if (methodGraphType != null)
                {
                    methodGraphType = GraphTypeConverter.ConvertTypeToGraphType(methodGraphType, required);
                    methodGraphType = EnsureList(method.ReturnType, methodGraphType);
                }
                else
                {
                    methodGraphType = GraphTypeConverter.ConvertTypeToGraphType(method.ReturnType, required);
                }

                var arguments =
                    new QueryArguments(
                        method.GetParameters()
                            .Where(p => p.ParameterType != typeof(ResolveFieldContext))
                            .Select(CreateArgument));

                // todo: need to fix method execution - not called currently so lower priority
                graphType.AddField(new FieldType { 
                    Type = methodGraphType,
                    Name = StringHelper.GraphName(method.Name),
                    Arguments = arguments,
                    DeprecationReason = TypeHelper.GetDeprecationReason(method),
                    //Resolver = new AsyncFuncFieldResolver(()=>ResolveField(context, field))
                });
            }
        }

        private static Type EnsureList(Type type, Type methodGraphType)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                methodGraphType = typeof(ListGraphType<>).MakeGenericType(methodGraphType);
            }

            return methodGraphType;
        }

        public static bool IsSpecialMethod(MethodInfo method)
        {
            return method.IsSpecialName || method.DeclaringType == typeof(object);
        }

        private static QueryArgument CreateArgument(ParameterInfo parameter)
        {
            var required = TypeHelper.IsNotNull(parameter);
            var parameterGraphType = TypeHelper.GetGraphType(parameter);
            if (parameterGraphType != null)
            {
                parameterGraphType = GraphTypeConverter.ConvertTypeToGraphType(parameterGraphType, required);
                parameterGraphType = EnsureList(parameter.ParameterType, parameterGraphType);
            }
            else
            {
                parameterGraphType = GraphTypeConverter.ConvertTypeToGraphType(parameter.ParameterType, required);
            }

            return new QueryArgument(parameterGraphType)
            {
                Name = parameter.Name,
                DefaultValue = TypeHelper.GetDefaultValue(parameter),
                Description = TypeHelper.GetDescription(parameter),
            };
        }
    }
}
