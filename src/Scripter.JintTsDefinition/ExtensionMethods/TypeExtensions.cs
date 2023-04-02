using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace doob.Scripter.JintTsDefinition.ExtensionMethods
{
    public static class TypeExtensions
    {

        public static bool IsGenericTypeParameter(this Type type)
        {
            return type.IsGenericParameter && (object) type.DeclaringMethod == null;
        }

        public static string GetFriendlyName(this Type type)
        {

            var tdInfo = type.GetTypeInfo();
            var isGenericTypeParameter = tdInfo.IsGenericTypeParameter();
            bool isArray = tdInfo.IsArray;

            if (isArray)
            {

                var arrElementInfo = tdInfo.GetElementType()?.GetTypeInfo();
                tdInfo = arrElementInfo;
                if (arrElementInfo?.IsGenericTypeParameter() == true)
                {
                    isGenericTypeParameter = true;
                }

            }

            string name = tdInfo.Name;
            string genericTypesString = null;
            if (tdInfo.IsGenericType)
            {
                name = name.Contains('`') ? name.Remove(name.IndexOf('`')) : name;
                var genericTypes = tdInfo.GenericTypeParameters.Any() ?
                    tdInfo.GenericTypeParameters.Select(GetFriendlyName):
                    tdInfo.GenericTypeArguments.Select(GetFriendlyName);

                genericTypesString = $"<{String.Join(", ", genericTypes)}>" ;
            }

            if (isGenericTypeParameter || tdInfo.IsGenericParameter)
            {
                name = $"{name}{genericTypesString}";
            }
            else
            {
                name = $"{tdInfo.Namespace}.{name}{genericTypesString}";
            }

            if (isArray)
            {
                name = $"{name}[]";
            }

            return name;

        }

        public static bool IsNullableType(this Type type, bool throwOnError = true)
        {
            if (type == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            return IsGenericTypeOf(type, typeof(Nullable<>));
        }

        public static bool IsGenericTypeOf(this Type type, Type genericType, bool throwOnError = true)
        {

            if (type == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }


            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsGenericTypeOf<T>(this Type type, bool throwOnError = true)
        {
            return IsGenericTypeOf(type, typeof(T), throwOnError);
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.IsExtensionMethod());
        }

        public static bool IsExtensionMethod(this MethodBase methodInfo)
        {
            return methodInfo.IsDefined(typeof(ExtensionAttribute), true);
        }

        public static bool IsStatic(this Type type, bool throwOnError = true)
        {
            if (type == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            return type.IsAbstract && type.IsSealed;
        }

        public static IEnumerable<Type> GetDirectInterfaces(this Type type)
        {
            var allInterfaces = new List<Type>();
            var childInterfaces = new List<Type>();

            foreach (var i in type.GetInterfaces())
            {
                allInterfaces.Add(i);
                foreach (var ii in i.GetInterfaces())
                    childInterfaces.Add(ii);
            }

            if (type.BaseType != null)
            {
                foreach (var baseTypeInterface in type.BaseType.GetInterfaces())
                {
                    childInterfaces.Add(baseTypeInterface);
                }
            }
            
            return allInterfaces.Except(childInterfaces);
        }

    }
}
