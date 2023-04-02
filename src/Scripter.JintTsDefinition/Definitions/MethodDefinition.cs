using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using doob.Scripter.JintTsDefinition.ExtensionMethods;

namespace doob.Scripter.JintTsDefinition.Definitions
{
    public class MethodDefinition : IDefinition
    {
        public string Name { get; set; }

        public TypeDefinition ReturnType { get; set; }
        
        public string AccessModifier { get; set; }
        public bool IsPublic { get; set; }
        public string FromType { get; set; }

        public TypeDefinition DeclaringType { get; set; }
        public TypeDefinition IsExtensionMethodFor { get; set; }
        public List<TypeDefinition> GenericArguments { get; set; } = new List<TypeDefinition>();

        public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();


        public override string ToString()
        {
            var strb = new StringBuilder();
            strb.Append($"{AccessModifier} {ReturnType} {Name}");
            if (GenericArguments.Any())
            {
                strb.Append($"<{String.Join(", ", GenericArguments)}>");
            }

            strb.Append($"({string.Join(", ", Parameters)})");

            return strb.ToString();
        }

        public static MethodDefinition FromMethodInfo(MethodInfo methodInfo)
        {

            if (methodInfo.Name.Equals("ToArray", StringComparison.OrdinalIgnoreCase))
            {
                var m = methodInfo;
            }
            var methodDescription = new MethodDefinition();
            methodDescription.Name = methodInfo.Name;
            methodDescription.AccessModifier = GetAccessModifier(methodInfo.Attributes);
            methodDescription.DeclaringType = TypeDefinition.FromType(methodInfo.DeclaringType);

            if (methodDescription.Name.Contains("."))
            {
                var lastDot = methodDescription.Name.LastIndexOf('.');
                methodDescription.FromType = methodDescription.Name.Substring(0, lastDot);
                methodDescription.Name = methodDescription.Name.Substring(lastDot+1);
            }

            var parameters = methodInfo.GetParameters();

            if (methodInfo.IsExtensionMethod())
            {
                methodDescription.Parameters =
                    parameters.Skip(1).Select(ParameterDefinition.FromParameterInfo).ToList();
                methodDescription.IsExtensionMethodFor = TypeDefinition.FromType(parameters.First().ParameterType);
            }
            else
            {
                methodDescription.Parameters =
                    parameters.Select(ParameterDefinition.FromParameterInfo).ToList();
            }
            
            


            methodDescription.ReturnType = TypeDefinition.FromType(methodInfo.ReturnType);

            if (methodInfo.IsGenericMethod)
            {

                var args = methodInfo.GetGenericArguments().Select(t => TypeDefinition.FromType(t)).ToList();

                if (methodDescription.IsExtensionMethodFor?.IsGeneric == true)
                {
                    args = args.Where(arg => arg.Name != methodDescription.IsExtensionMethodFor.Name).ToList();
                }

                methodDescription.GenericArguments = args;

            }


            return methodDescription;
        }
        
        internal static string GetAccessModifier(MethodAttributes attributes)
        {
            if ((attributes & MethodAttributes.Public) == MethodAttributes.Public)
            {
                return "public";
            }

            if ((attributes & MethodAttributes.FamORAssem) == MethodAttributes.FamORAssem)
            {
                return "protected internal";
            }

            if ((attributes & MethodAttributes.Family) == MethodAttributes.Family)
            {
                return "protected";
            }

            if ((attributes & MethodAttributes.Assembly) == MethodAttributes.Assembly)
            {
                return "internal";
            }

            if ((attributes & MethodAttributes.FamANDAssem) == MethodAttributes.FamANDAssem)
            {
                return "private protected";
            }

            if ((attributes & MethodAttributes.Private) == MethodAttributes.Private)
            {
                return "private";
            }

            return null;
        }

    }
}
