using System.Reflection;
using doob.Scripter.JintTsDefinition.ExtensionMethods;

namespace doob.Scripter.JintTsDefinition.Definitions
{
    public class ParameterDefinition : IDefinition
    {
        public string Name { get; set; }

        public TypeDefinition Type { get; set; }

        public string Ref { get; set; }

        public object DefaultValue { get; set; }
        public bool IsOptional { get; set; }

        public override string ToString()
        {
            return $"{Ref}{Type} {Name}";
        }

        public static ParameterDefinition FromParameterInfo(ParameterInfo parameterInfo)
        {

            var desc = new ParameterDefinition();
            desc.Name = parameterInfo.Name;

            var pType = parameterInfo.ParameterType;

            if (pType.IsByRef)
            {
                desc.Ref = parameterInfo.IsOut ? "out " : "ref ";
                pType = parameterInfo.ParameterType.GetElementType();
            }
            
            if (pType.IsGenericType && pType.IsGenericTypeParameter() && !pType.IsArray)
            {
                desc.Type = TypeDefinition.FromType(pType.GetGenericTypeDefinition());
            }
            else
            {
                desc.Type = TypeDefinition.FromType(pType);
            }

            desc.IsOptional = parameterInfo.IsOptional;
            desc.DefaultValue = parameterInfo.DefaultValue;

            return desc;
        }
    }
}
