using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doob.Scripter.JintTsDefinition.ExtensionMethods;

namespace doob.Scripter.JintTsDefinition.Definitions
{
    public class IndexerDefinition: IDefinition
    {
        public string Name { get; set; }

        public string AccessModifier { get; set; }

        public TypeDefinition ReturnType { get; set; }

        public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();

        public string GetterModifer { get; set; }
        public string SetterModifier { get; set; }

        public static IndexerDefinition FromPropertyInfo(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.IsIndexerProperty())
            {
                throw new InvalidCastException($"'{propertyInfo.Name}' is NOT an Indexer Property!");
            }
                
            var pDesc = new IndexerDefinition();
            pDesc.Name = propertyInfo.Name;
            pDesc.ReturnType = propertyInfo.GetMethod != null ? TypeDefinition.FromType(propertyInfo.GetMethod?.ReturnType) : TypeDefinition.FromType(typeof(void));

            pDesc.GetterModifer = propertyInfo.GetMethod != null ? MethodDefinition.GetAccessModifier(propertyInfo.GetMethod.Attributes) : null;
            pDesc.SetterModifier = propertyInfo.SetMethod != null ? MethodDefinition.GetAccessModifier(propertyInfo.SetMethod.Attributes) : null;

            var hasGetter = !String.IsNullOrWhiteSpace(pDesc.GetterModifer);
            var hasSetter = !String.IsNullOrWhiteSpace(pDesc.SetterModifier);

            if (hasGetter && hasSetter)
            {
                if (pDesc.GetterModifer == pDesc.SetterModifier)
                {
                    pDesc.AccessModifier = pDesc.GetterModifer;
                }
                else
                {
                    var i1 = Array.IndexOf(Constants.AccessModifiers, pDesc.GetterModifer);
                    var i2 = Array.IndexOf(Constants.AccessModifiers, pDesc.SetterModifier);

                    if (i1 < i2)
                    {
                        pDesc.AccessModifier = pDesc.GetterModifer;
                    }
                    else
                    {
                        pDesc.AccessModifier = pDesc.SetterModifier;
                    }
                }

                pDesc.Parameters = propertyInfo.GetMethod.GetParameters().Select(ParameterDefinition.FromParameterInfo)
                    .ToList();

            }
            else if (hasGetter)
            {
                pDesc.AccessModifier = pDesc.GetterModifer;
                pDesc.Parameters = propertyInfo.GetMethod.GetParameters().Select(ParameterDefinition.FromParameterInfo)
                    .ToList();
            }
            else if (hasSetter)
            {
                pDesc.AccessModifier = pDesc.SetterModifier;
                pDesc.Parameters = propertyInfo.SetMethod.GetParameters().Select(ParameterDefinition.FromParameterInfo)
                    .ToList();
            }


            return pDesc;

        }

    }
}
