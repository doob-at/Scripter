using System;
using System.Collections.Generic;
using System.Linq;
using doob.Scripter.JintTsDefinition.Definitions;
using doob.Scripter.JintTsDefinition.ExtensionMethods;

namespace doob.Scripter.JintTsDefinition
{
    public class TypeScriptRendererDefaults
    {
        public Dictionary<Type, string> TypeMappings = new Dictionary<Type, string>
        {
            [typeof(char)] = "string",
            [typeof(string)] = "string",
            [typeof(int)] = "number",
            [typeof(void)] = "void",
            [typeof(object)] = "any",
            [typeof(bool)] = "boolean",
            [typeof(DateTime)] = "Date"
        };

        public string NormalizeTypeName(TypeDefinition typeDefinition, List<Type> allowedTypes, bool includeNamespace = true)
        {
            if (typeDefinition.RawType == null)
                return typeDefinition.Name;

            var type = typeDefinition.RawType;

           
            if (typeDefinition.IsArray)
            {
                type = type.GetElementType() ?? type;
            }

            if (type.IsGenericTypeParameter() || type.IsGenericMethodParameter)
            {
                return typeDefinition.Name;
            }

            //if (typeDefinition.IsNullable)
            //{
            //    type = Nullable.GetUnderlyingType(type);
            //}
           
            if (type == typeof(char))
            {
                type = typeof(string);
            }


            if (Constants.NumericTypes.Contains(type))
            {
                type = typeof(int);
            }

           
            if (!TypeMappings.TryGetValue(type, out var name))
            {


                var isAllowed = true;
                if (allowedTypes?.Any() == true)
                {
                    var checkType = type.IsGenericType
                        ? type.GetGenericTypeDefinition()
                        : type;
                    
                    if (!allowedTypes.Contains(checkType))
                    {
                        isAllowed = false;
                    }

                }

                if (isAllowed || typeDefinition.IsGeneric)
                {
                    if (!includeNamespace)
                    {
                        name = typeDefinition.Name;
                    }
                    else
                    {
                        name = $"{typeDefinition.Namespace}.{typeDefinition.Name}".Trim('.');
                    }
                }
                else
                {
                    name = "any";
                }

            }



            if (typeDefinition.IsArray)
            {
                name = $"{name}[]";
            }

           

            return name;

        }

        public string NormalizeIdentifier(string value)
        {
            if (ReservedWordsDictionary.ContainsKey(value))
            {
                return ReservedWordsDictionary[value];
            }

            return value;
        }

        public Dictionary<string, string> ReservedWordsDictionary = new Dictionary<string, string>
        {
            ["finally"] = "finaly"
        };

        public bool IncludeGenericMethods { get; set; } = false;
        public bool IncludeMethodsWithReferenceParameters { get; set; }
    }
}
