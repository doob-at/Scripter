using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using doob.Scripter.JintTsDefinition.Definitions;

namespace doob.Scripter.JintTsDefinition
{
    internal static class TypeCache
    {
        public static ConcurrentDictionary<string, TypeDefinition> Cache { get; } = new ConcurrentDictionary<string, TypeDefinition>();


        public static TypeDefinition JsAny { get; } = new TypeDefinition
        {
            Name = "any"
        };

        public static TypeDefinition JsNone { get; } = new TypeDefinition
        {
            Name = "none"
        };

        public static TypeDefinition JsString { get; }  = new TypeDefinition
        {
            Name = "String"
        };

        public static string BuildActionTypeName(Type type, List<string> args)
        {

            var l = new List<string>();
            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                l.Add($"arg{i}: {arg}");
            }

            return $"(({string.Join(", ", l)}) => void)";

        }

        public static string BuildFuncTypeName(Type type, List<string> args)
        {

            var l = new List<string>();
            for (var i = 0; i < args.Count - 1; i++)
            {
                var arg = args[i];
                l.Add($"arg{i}: {arg}");
            }

            return $"(({string.Join(", ", l)}) => {args.Last()})";
        }

        public static string BuildPredicateTypeName(Type type, List<string> args)
        {

            var l = new List<string>();
            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                l.Add($"arg{i}: {arg}");
            }

            return $"(({string.Join(", ", l)}) => boolean)";
        }
    }
}
