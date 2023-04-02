using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Shared
{
    public static class TypeHelper
    {

        private static Dictionary<string, string> TypeMapping = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["number"] = "double",
            ["any"] = "object",
            ["date"] = "System.DateTime"
        };

        private static List<string> BuiltInTypeScriptType = new List<string>()
        {
            "date"
        };

        public static object? CreateObject(string typeName, object[] parameters)
        {
            //typeName = typeName.Replace("$", "`");
            //typeName = RemoveGenericMarker(typeName);

            var type = doob.Reflectensions.Helper.TypeHelper.FindType(typeName, TypeMapping);
            if (type == null)
                return null;

            if (parameters?.Any() == true)
            {
                return Activator.CreateInstance(type, parameters);
            }
            else
            {
                return Activator.CreateInstance(type);
            }

        }
        
        public static object? CreateObjectWithDI(IServiceProvider serviceProvider, string typeName, object[] parameters)
        {
            //typeName = typeName.Replace("$", "`");
            //typeName = RemoveGenericMarker(typeName);

            var type = doob.Reflectensions.Helper.TypeHelper.FindType(typeName, TypeMapping);
            if (type == null)
                return null;

            if (parameters?.Any() == true)
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, type, parameters);
            }
            else
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, type);
            }

        }

        public static Type? FindConstructorReplaceType(string typeName)
        {
            if (BuiltInTypeScriptType.Contains(typeName.ToLower()))
            {
                return null;
            }
            return doob.Reflectensions.Helper.TypeHelper.FindType(typeName, TypeMapping);
        }

    }
}
