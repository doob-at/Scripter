using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace doob.Scripter.JintTsDefinition.Definitions
{
    public class ConstructorDefinition : IDefinition
    {
        public string Name { get; } = "constructor";

        public TypeDefinition DeclaringType { get; set; }
        
        public List<TypeDefinition> GenericArguments { get; set; } = new List<TypeDefinition>();

        public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();


        public override string ToString()
        {
            
            return $"{Name}({string.Join(", ", Parameters)})";
        }

        public static ConstructorDefinition FromConstructorInfo(ConstructorInfo constructorInfo)
        {
            var constructorDefinition = new ConstructorDefinition();

            constructorDefinition.DeclaringType = TypeDefinition.FromType(constructorInfo.DeclaringType);

            var parameters = constructorInfo.GetParameters();
            constructorDefinition.Parameters = parameters.Select(ParameterDefinition.FromParameterInfo).ToList();

            if (constructorInfo.IsGenericMethod)
            {

                var args = constructorInfo.GetGenericArguments().Select(t => TypeDefinition.FromType(t)).ToList();
                
                constructorDefinition.GenericArguments = args;

            }

            return constructorDefinition;
        }
    }
}
