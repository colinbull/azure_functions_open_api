using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Inno.AzureFunctionsV2.OpenApi
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey,TValue> a, IDictionary<TKey, TValue> b)
        {
            foreach(var kvp in b)
            {
                if(!a.ContainsKey(kvp.Key)) {
                    a[kvp.Key] = kvp.Value;
                }
            }

            return a;
        }
    }

    public static class TypeExtensions 
    {
        private static IDictionary<Type, (string, string)> TypeToOpenApiTypeFormatMap = new Dictionary<Type,(string,string)> {
            { typeof(String), ("string", null) },
            { typeof(Int16), ("integer", "int32")},
            { typeof(UInt16), ("integer", "int32") },
            { typeof(Int32), ("integer", "int32")},
            { typeof(UInt32), ("integer", "int32") },
            { typeof(Int64), ("integer", "int64")},
            { typeof(UInt64), ("integer", "int64") },
            { typeof(Single), ("number", "float") },
            { typeof(Double), ("number", "double")},
            { typeof(byte), ("string", "byte") },
            { typeof(Boolean), ("boolean", null)},
            { typeof(DateTime), ("string", "date-time") }
        };

        public static bool IsIEnumerable(this Type type)
        {
            return type.IsArray || (type.GetInterface(typeof(System.Collections.IEnumerable).Name) != null) && type != typeof(String);
        }

        public static bool TryMapPrimitive(this Type type, out OpenApiSchema schema)
        {
            if(TypeToOpenApiTypeFormatMap.TryGetValue(type, out var openApiType))
            {
                schema = new OpenApiSchema {
                    Type = openApiType.Item1,
                    Format = openApiType.Item2
                };
                return true;
            }

            schema = null;
            return false;
        }

        public static bool TryMapIEnumerable(this Type type, out Tuple<OpenApiSchema, Type> schema)
        {
            if(type.IsIEnumerable())
            {
                var elementSchema = (type.IsArray ? type.GetElementType() : type.IsGenericType ? type.GenericTypeArguments[0] : typeof(object));

                schema = Tuple.Create(new OpenApiSchema {
                    Type = "array",
                    Items = new OpenApiSchema { Reference = new OpenApiReference { Id = elementSchema.Name, Type = ReferenceType.Schema }}
                }, elementSchema);

                return true;
            }

            schema = null;
            return false;
        }

        public static IDictionary<string, OpenApiSchema> GetSchemas(this Type type)
        {
            var schemas = new Dictionary<string, OpenApiSchema> {
                [type.Name] = new OpenApiSchema {
                    Properties = new Dictionary<string, OpenApiSchema>()
                }
            };

            var rootSchema = schemas[type.Name];
            foreach(var prop in type.GetProperties())
            {
                if(TryMapPrimitive(prop.PropertyType, out var primitive))
                {
                    rootSchema.Properties[prop.Name] = primitive;
                    continue;
                }

                if(TryMapIEnumerable(prop.PropertyType, out var collection))
                {
                    rootSchema.Properties[prop.Name] = collection.Item1;
                    schemas.Merge(collection.Item2.GetSchemas());
                    continue;
                }

                schemas.Merge(prop.PropertyType.GetSchemas());
            }

            return schemas;
        }


        public static IEnumerable<ParameterInfo> GetParametersExcluding(this MethodInfo m, HashSet<Type> excludedParameterTypes)
        {
            return m.GetParameters()
                    .Where(p => 
                            !excludedParameterTypes.Contains(p.ParameterType)
                            && !p.GetCustomAttributes().Any(a => excludedParameterTypes.Contains(a.GetType())));
        }

        public static IEnumerable<T> GetAttributes<T>(this MethodInfo m)
            where T : Attribute
        {
            return m.GetCustomAttributes(typeof(T), false).OfType<T>();
        }

    }



}
