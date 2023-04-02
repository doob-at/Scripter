using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doob.Scripter.JintTsDefinition.ExtensionMethods;


namespace doob.Scripter.JintTsDefinition
{
    public class DefinitionBuilder
    {
        private List<Type> TypesToProcess { get; set; } = new List<Type>();
        private List<Type> CalculatedTypes { get; set; } = new List<Type>();
        private List<MethodInfo> ExtensionMethods { get; set; } = new List<MethodInfo>();
        public DefinitionBuilder AddTypes(params Type[] types)
        {
            return AddTypes(types.ToList());
        }
        public DefinitionBuilder AddTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                
                var t = type;
                if (t.IsGenericType)
                {
                    t = t.GetGenericTypeDefinition();
                }
                if (!TypesToProcess.Contains(t))
                {
                    TypesToProcess = TypesToProcess.Append(t).ToList();
                }
            }

            return this;
        }
        public DefinitionBuilder AddType<T>()
        {
            return AddTypes(typeof(T));
        }


        public DefinitionBuilder AddExtensionMethods(params Type[] types)
        {
            return AddExtensionMethods(types.ToList());
        }
        public DefinitionBuilder AddExtensionMethods(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AddExtensionMethods(type.GetExtensionMethods());
            }

            return this;
        }
        public DefinitionBuilder AddExtensionMethods(IEnumerable<MethodInfo> methodInfos)
        {
            foreach (var methodInfo in methodInfos)
            {
                if (!methodInfo.IsExtensionMethod())
                {
                    continue;
                }

                if (!ExtensionMethods.Contains(methodInfo))
                {
                    ExtensionMethods.Add(methodInfo);
                }
            }

            return this;
        }

        public DefinitionBuilder AddTypesFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsPublic)
                {
                    continue;
                }

                AddTypes(type);
            }

            return this;
        }

        internal List<Type> GetTypesToProcess()
        {
           
            foreach (var type in TypesToProcess)
            {
                AddDependedTypes(type);
            }
            
            return CalculatedTypes.ToList();
        }

        internal List<MethodInfo> GetExtensionMethods()
        {
            return ExtensionMethods.ToList();
        }


        private void AddDependedTypes(Type type)
        {
            if (type.IsGenericType)
            {

                foreach (var typeGenericTypeArgument in type.GenericTypeArguments)
                {
                    AddDependedTypes(typeGenericTypeArgument);
                }
                type = type.GetGenericTypeDefinition();
            }

            if (CalculatedTypes.Contains(type))
            {
                return;
            }

            CalculatedTypes.Add(type);
            

            if (type.BaseType != null)
            {
                AddDependedTypes(type.BaseType);
            }

            

            var n = type.FullName;
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                AddDependedTypes(propertyInfo.PropertyType);
            }

            var directInterfaces = type.GetInterfaces().ToList();
            foreach (var interfaceType in directInterfaces)
            {
                AddDependedTypes(interfaceType);
            }

            foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                AddDependedTypes(methodInfo.ReturnType);

                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    AddDependedTypes(parameterInfo.ParameterType);
                }
            }

           
        }

        private void AddBaseType(Type type)
        {
            AddTypes(type);
            if (type.BaseType != null)
            {
                AddBaseType(type.BaseType);
            }

            foreach (var interfaceType in type.GetDirectInterfaces())
            {
                AddTypes(interfaceType);
            }
            
        }
        
        public Dictionary<string, string> Render()
        {
            var tsenderer = new TypeScriptRenderer();
            return tsenderer.Render(this);
        }
    }
}
