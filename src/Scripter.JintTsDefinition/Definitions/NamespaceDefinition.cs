using System.Collections.Generic;
using System.Linq;

namespace doob.Scripter.JintTsDefinition.Definitions
{
    public class NamespaceDefinition : IDefinition
    {
        public string Name { get; set; }
        public List<NamespaceDefinition> Namespaces { get; set; } = new List<NamespaceDefinition>();

        public List<TypeDefinition> Types { get; set; } = new List<TypeDefinition>();

        public NamespaceDefinition? GetNameSpaceDefinition(string name)
        {
            var nsParts = name.Split('.');

            var current = this;
            foreach (var nsPart in nsParts)
            {
                var foundNs = current.Namespaces.FirstOrDefault(n => n.Name == nsPart);
                if (foundNs == null)
                {
                    return null;
                }
                current = foundNs;
            }

            return current;
        }

        public NamespaceDefinition AddNamespaceDefinition(string name)
        {
            var nsParts = name.Split('.');

            var ns = this;
            
            foreach (var nsPart in nsParts)
            {
                var foundNs = ns.Namespaces.FirstOrDefault(n => n.Name == nsPart);
                if (foundNs == null)
                {
                    foundNs = new NamespaceDefinition
                    {
                        Name = nsPart
                    };
                    ns.Namespaces.Add(foundNs);
                }

                ns = foundNs;
            }

            return ns;
        }
    }
}
