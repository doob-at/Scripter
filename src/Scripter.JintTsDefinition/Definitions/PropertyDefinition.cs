using System;
using System.Reflection;
using System.Text;
using doob.Scripter.JintTsDefinition.ExtensionMethods;

namespace doob.Scripter.JintTsDefinition.Definitions
{
    public class PropertyDefinition : IDefinition
    {
        public string Name { get; set; }

        public TypeDefinition Type { get; set; }

        public string AccessModifier { get; set; }
        public string FromType { get; set; }
        public bool IsPublic { get; set; }
        public string GetterModifer { get; set; }
        public string SetterModifier { get; set; }
        public override string ToString()
        {

            var str = new StringBuilder();
            str.Append($"{AccessModifier} {Type.Name} {Name} {{");
            if (!string.IsNullOrWhiteSpace(GetterModifer))
            {
                if (GetterModifer != AccessModifier)
                {
                    str.Append($" {GetterModifer} get;");
                }
                else
                {
                    str.Append($" get;");
                }

            }
            if (!String.IsNullOrWhiteSpace(SetterModifier))
            {
                if (SetterModifier != AccessModifier)
                {
                    str.Append($" {SetterModifier} set;");
                }
                else
                {
                    str.Append($" set;");
                }
            }

            str.Append(" }");

            return str.ToString();
        }

        public static PropertyDefinition FromPropertyInfo(PropertyInfo propertyInfo)
        {
            var pDesc = new PropertyDefinition();
            pDesc.Name = propertyInfo.Name;
            pDesc.Type = TypeDefinition.FromType(propertyInfo.PropertyType);
            pDesc.IsPublic = propertyInfo.IsPublic();

            if (propertyInfo.Name.Contains("."))
            {
                var lastDot = pDesc.Name.LastIndexOf('.');
                pDesc.FromType = pDesc.Name.Substring(0, lastDot);
                pDesc.Name = pDesc.Name.Substring(lastDot+1);
            }

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



            }
            else if (hasGetter)
            {
               
                pDesc.AccessModifier = pDesc.GetterModifer;
            }
            else if (hasSetter)
            {
               
                pDesc.AccessModifier = pDesc.SetterModifier;
            }

            return pDesc;
        }

    }
}
