using System;
using System.Collections.Generic;

namespace doob.Scripter.JintTsDefinition
{
    internal static class Constants
    {
        public static string[] AccessModifiers = new[]
        {
            "public",
            "protected internal",
            "protected",
            "internal",
            "private protected",
            "private"
        };

        internal static List<Type> NumericTypes { get; } = new List<Type>
        {
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(float),
            typeof(ulong),
            typeof(double),
            typeof(decimal),
        };
    }
}
