using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using doob.Scripter.JintTsDefinition.Definitions;

namespace doob.Scripter.JintTsDefinition
{
    public class TypeScriptRenderer
    {

        TypeScriptRendererDefaults Defaults = new TypeScriptRendererDefaults();

        private List<Type> AllowedTypes = new List<Type>();

        public TypeScriptRenderer()
        {

        }

        private string BuildDocComments(int indent, params string[] lines)
        {
            return BuildDocComments(indent, lines.ToList());
        }
        private string BuildDocComments(int indent, IEnumerable<string> lines)
        {

            var linesList = lines.ToList();
            if (linesList.Any())
            {
                var indentString = GetIndentString(indent);
                var comments = new StringBuilder();
                comments.AppendLine();
                comments.AppendLine($"{indentString}/**");
                foreach (var s in linesList)
                {
                    comments.AppendLine($"{indentString}* {s}");
                }
                comments.AppendLine($"{indentString}*/");
                return comments.ToString();
            }

            return null;
        }

        private string GetIndentString(int indent = 0)
        {
            return new string(' ', indent);
        }
        public string Render(PropertyDefinition propertyDefinition, int indent)
        {

            var prop = BuildTypeString(propertyDefinition.Type);
            var otherType = GetTypeString(propertyDefinition.Type);

            string comments = null;
            if (prop != otherType)
            {
                comments = BuildDocComments(indent, otherType);
            }

            var name = propertyDefinition.Name.Contains(".")
                ? propertyDefinition.Name.Split('.').Last() : propertyDefinition.Name;
            var priv = propertyDefinition.IsPublic ? "" : "private ";

            return $"{comments}{GetIndentString(indent)}{priv}{name}: {prop};";

        }

        public string Render(ConstructorDefinition constructorDefinition, int indent)
        {
            var parameters = new List<string>();
            var commentLines = new List<string>();
            foreach (var parameter in constructorDefinition.Parameters)
            {

                var paramName = Defaults.NormalizeIdentifier(parameter.Name ?? "p");


                var buildType = BuildTypeString(parameter.Type);
                if (parameter.Type.IsArray && !buildType.EndsWith("[]"))
                {
                    buildType += "[]";
                }


                var optional = parameter.IsOptional ? "?" : null;
                parameters.Add($"{paramName}{optional}: {buildType}");

                if (parameter.Type.RawType?.Name == "Action" || parameter.Type.RawType?.Name.StartsWith("Action`") == true)
                {
                    continue;
                }
                if (parameter.Type.RawType?.Name.StartsWith("Func`") == true)
                {
                    continue;
                }
                if (parameter.Type.RawType?.Name.StartsWith("Predicate`") == true)
                {
                    continue;
                }


                var getType = GetTypeString(parameter.Type);

                if (buildType != getType || parameter.IsOptional)
                {
                    string defaultValue = null;
                    if (parameter.IsOptional)
                    {
                        var valueString = "null";

                        if (parameter.DefaultValue != null)
                        {
                            switch (parameter.DefaultValue)
                            {

                                case Enum enu:
                                    {
                                        valueString = $"{parameter.Type.FriendlyName}.{enu.ToString()}";
                                        break;
                                    }
                                case bool b:
                                    {
                                        valueString = b.ToString().ToLower();
                                        break;
                                    }
                                default:
                                    {
                                        valueString = parameter.DefaultValue.ToString();
                                        break;
                                    }
                            }
                        }

                        defaultValue = $" = {valueString}";
                    }


                    commentLines.Add($"@param {paramName} {getType}{defaultValue}");
                }

            }

            var comments = BuildDocComments(indent, commentLines);

            return $"{comments}{GetIndentString(indent)}{constructorDefinition.Name}({String.Join(", ", parameters)});";

        }

        public string Render(IndexerDefinition indexerDefinition, int indent)
        {
            var parameters = new List<string>();
            var commentLines = new List<string>();

            var returnType = BuildTypeString(indexerDefinition.ReturnType);
            var otherReturnType = GetTypeString(indexerDefinition.ReturnType);

            foreach (var indexerDescriptionParameter in indexerDefinition.Parameters)
            {
                var buildType = BuildTypeString(indexerDescriptionParameter.Type);
                parameters.Add($"{indexerDescriptionParameter.Name}: {buildType}");

                var getType = GetTypeString(indexerDescriptionParameter.Type);

                if (buildType != getType)
                {
                    commentLines.Add($"@param {indexerDescriptionParameter.Name} {getType}");
                }
            }

            if (returnType != otherReturnType)
            {
                commentLines.Add($@"@returns {otherReturnType}");
            }

            var comments = BuildDocComments(indent, commentLines);


            return $"{comments}{GetIndentString(indent)}[{String.Join(", ", parameters)}]: {returnType};";
        }


        public string Render(MethodDefinition methodDefinition, int indent)
        {

            var parameters = new List<string>();
            var commentLines = new List<string>();

            var returnType = BuildTypeString(methodDefinition.ReturnType);
            if (methodDefinition.ReturnType.IsArray && !returnType.EndsWith("[]"))
            {
                returnType += "[]";
            }
            var otherReturnType = GetTypeString(methodDefinition.ReturnType);
            if (methodDefinition.ReturnType.IsArray && !otherReturnType.EndsWith("[]"))
            {
                otherReturnType += "[]";
            }



            foreach (var methodDescriptionParameter in methodDefinition.Parameters)
            {

                var paramName = Defaults.NormalizeIdentifier(methodDescriptionParameter.Name ?? "p");

                var buildType = BuildTypeString(methodDescriptionParameter.Type);
                if (methodDescriptionParameter.Type.IsArray && !buildType.EndsWith("[]"))
                {
                    buildType += "[]";
                }


                var optional = methodDescriptionParameter.IsOptional ? "?" : null;
                parameters.Add($"{paramName}{optional}: {buildType}");

                if (methodDescriptionParameter.Type.RawType?.Name == "Action" || methodDescriptionParameter.Type.RawType?.Name.StartsWith("Action`") == true)
                {
                    continue;
                }
                if (methodDescriptionParameter.Type.RawType?.Name.StartsWith("Func`") == true)
                {
                    continue;
                }
                if (methodDescriptionParameter.Type.RawType?.Name.StartsWith("Predicate`") == true)
                {
                    continue;
                }


                var getType = GetTypeString(methodDescriptionParameter.Type);

                if (buildType != getType || methodDescriptionParameter.IsOptional)
                {
                    string defaultValue = null;
                    if (methodDescriptionParameter.IsOptional)
                    {
                        var valueString = "null";

                        if (methodDescriptionParameter.DefaultValue != null)
                        {
                            switch (methodDescriptionParameter.DefaultValue)
                            {

                                case Enum enu:
                                    {
                                        valueString = $"{methodDescriptionParameter.Type.FriendlyName}.{enu.ToString()}";
                                        break;
                                    }
                                case bool b:
                                    {
                                        valueString = b.ToString().ToLower();
                                        break;
                                    }
                                default:
                                    {
                                        valueString = methodDescriptionParameter.DefaultValue.ToString();
                                        break;
                                    }
                            }
                        }

                        defaultValue = $" = {valueString}";
                    }


                    commentLines.Add($"@param {paramName} {getType}{defaultValue}");
                }

            }

            if (returnType != otherReturnType)
            {
                commentLines.Add($@"@returns {otherReturnType}");
            }

            var comments = BuildDocComments(indent, commentLines);


            var genericArguments = string.Empty;
            if (methodDefinition.GenericArguments.Any())
            {
                genericArguments = $"<{string.Join(", ", methodDefinition.GenericArguments)}>";
            }

            var ret = $"{comments}{GetIndentString(indent)}{methodDefinition.Name}{genericArguments}({String.Join(", ", parameters)}): {returnType};";
            return ret;

        }


        private List<TypeDefinition> GetParamtersTypesRecurse(TypeDefinition typeDefinition)
        {
            var l = new List<TypeDefinition>();
            if (typeDefinition.IsGeneric)
            {
                l.Add(typeDefinition);
            }
            else
            {
                foreach (var typeDefinitionGenericArgument in typeDefinition.GenericArguments)
                {
                    l.AddRange(GetParamtersTypesRecurse(typeDefinitionGenericArgument));
                }
            }

            return l;
        }


        public string Render(TypeDefinition typeDefinition, int indent = 0)
        {
            var strb = new StringBuilder();

            string kind;

            switch (typeDefinition.Kind)
            {
                case "class":
                    {
                        kind = "class";
                        break;
                    }
                case "enum":
                    {
                        kind = "enum";
                        break;
                    }
                default:
                    {
                        kind = "interface";
                        break;
                    }
            }

            var indentString = GetIndentString(indent);
            strb.Append($"{GetIndentString(indent)}{kind} {BuildTypeDefinitionTypeString(typeDefinition)}");

            var extends = String.Empty;
            if (typeDefinition.BaseType != null)
            {

                var tdInfo = typeDefinition.BaseType.RawType.GetTypeInfo();

                var checkType = tdInfo.IsGenericType
                    ? tdInfo.GetGenericTypeDefinition()
                    : tdInfo;

                if (
                    checkType == typeof(object) ||
                    checkType == typeof(Enum) ||
                    checkType == typeof(ValueType) ||
                    AllowedTypes?.Contains(checkType) != true
                )
                {

                }
                else
                {
                    extends = $" extends {BuildTypeString(typeDefinition.BaseType)}";
                }
            }


            if (typeDefinition.ImplementedInterfaces?.Any() == true)
            {
                var impl = kind == "interface" ? "extends" : "implements";
                var names = typeDefinition.ImplementedInterfaces.Select(i => BuildTypeString(i)).Where(n => n != "any" && n != "any[]").ToList();
                if (names.Any())
                {
                    extends += $" {impl} {String.Join(", ", names)}";
                }
            }

            if (!string.IsNullOrWhiteSpace(extends))
            {
                strb.Append(extends);
            }

            strb.AppendLine(" {");


            var missingDefinitions = new TypeDefinition();

            if (typeDefinition.ImplementedInterfaces != null)
            {
                foreach (var interf in typeDefinition.ImplementedInterfaces)
                {
                    foreach (var methodDefinition in interf.Methods)
                    {
                        var contains = typeDefinition.Methods.Any(m => m.Name == methodDefinition.Name);
                        if (!contains)
                        {
                            missingDefinitions.Methods.Add(methodDefinition);
                        }
                    }
                }
            }
            


            strb.AppendLine(RenderBody(typeDefinition, indent + 4));
            strb.AppendLine(RenderBody(missingDefinitions, indent + 4));

            strb.AppendLine($"{indentString}}}");
            return strb.ToString();
        }
        public string RenderBody(TypeDefinition typeDefinition, int indent = 0, Func<IDefinition, string, string> definitionString = null)
        {
            var strb = new StringBuilder();

            var indentNext = indent;

            if (typeDefinition.EnumValueDefinitions?.Any() == true)
            {
                var enumLines = new List<string>();

                foreach (var enumValue in typeDefinition.EnumValueDefinitions)
                {
                    enumLines.Add($"{GetIndentString(indentNext)}{enumValue.Name} = {enumValue.Value}");
                }
                strb.AppendLine();
                strb.AppendLine(string.Join($",{Environment.NewLine}", enumLines));
            }


            if (typeDefinition.Properties?.Any() == true)
            {
                strb.AppendLine();
                var processed = new List<string>();
                foreach (var typeDescriptionProperty in typeDefinition.Properties.OrderByDescending(p => p.IsPublic))
                {
                    if (processed.Contains(typeDescriptionProperty.Name))
                        continue;

                    processed.Add(typeDescriptionProperty.Name);

                    if (typeDescriptionProperty.IsPublic || !string.IsNullOrEmpty(typeDescriptionProperty.FromType))
                    {
                        var rendered = Render(typeDescriptionProperty, indentNext);
                        if (definitionString != null)
                        {
                            rendered = definitionString(typeDescriptionProperty, rendered);
                        }

                        if (!String.IsNullOrWhiteSpace(rendered))
                        {
                            strb.AppendLine(rendered);
                        }

                    }

                }
            }

            if (typeDefinition.Constructors.Any())
            {
                strb.AppendLine();
                foreach (var constructor in typeDefinition.Constructors)
                {
                    var rendered = Render(constructor, indentNext);

                    if (definitionString != null)
                    {
                        rendered = definitionString(constructor, rendered);
                    }

                    if (!String.IsNullOrWhiteSpace(rendered))
                    {
                        strb.AppendLine(rendered);
                    }
                }
            }

            if (typeDefinition.Indexer?.Any() == true)
            {
                strb.AppendLine();
                foreach (var typeDescriptionProperty in typeDefinition.Indexer)
                {
                    var rendered = Render(typeDescriptionProperty, indentNext);

                    if (definitionString != null)
                    {
                        rendered = definitionString(typeDescriptionProperty, rendered);
                    }

                    if (!String.IsNullOrWhiteSpace(rendered))
                    {
                        strb.AppendLine(rendered);
                    }

                }
            }

            if (typeDefinition.Methods?.Any() == true)
            {

                strb.AppendLine();
                foreach (var typeDescriptionMethod in typeDefinition.Methods)
                {
                    var isGenericMethod = typeDescriptionMethod.GenericArguments.Any();
                    if (isGenericMethod && !Defaults.IncludeGenericMethods)
                    {
                        continue;
                    }

                    var hasReferenceParameter = typeDescriptionMethod.Parameters.Any(p => !String.IsNullOrWhiteSpace(p.Ref));
                    if (hasReferenceParameter && !Defaults.IncludeMethodsWithReferenceParameters)
                    {
                        continue;
                    }

                    var rendered = Render(typeDescriptionMethod, indentNext);

                    if (definitionString != null)
                    {
                        rendered = definitionString(typeDescriptionMethod, rendered);
                    }

                    if (!String.IsNullOrWhiteSpace(rendered))
                    {
                        strb.AppendLine(rendered);
                    }
                }
            }

            return strb.ToString();
        }



        private string BuildTypeString(TypeDefinition typeDefinition, bool includeNamespace = true)
        {

            if (typeDefinition.TryGetPayload<NormalizedNonGenericTypeName>(out var normalized))
            {
                if (includeNamespace)
                {
                    return $"{normalized.Namespace}.{normalized.TypeName}";
                }
                return normalized.TypeName;
            }


            if (typeDefinition.FriendlyName == "System.Action" || typeDefinition.FriendlyName?.StartsWith("System.Action<") == true)
            {
                var args = new List<string>();
                if (typeDefinition.GenericArguments.Any())
                {
                    args = typeDefinition.GenericArguments.Select(s => BuildTypeString(s)).ToList();
                }
                return TypeCache.BuildActionTypeName(typeDefinition.RawType, args);
            }

            if (typeDefinition.FriendlyName?.StartsWith("System.Func<") == true)
            {
                var args = new List<string>();
                if (typeDefinition.GenericArguments.Any())
                {
                    args = typeDefinition.GenericArguments.Select(s => BuildTypeString(s)).ToList();
                }
                return TypeCache.BuildFuncTypeName(typeDefinition.RawType, args);
            }


            if (typeDefinition.FriendlyName?.StartsWith("System.Predicate<") == true)
            {
                var args = new List<string>();
                if (typeDefinition.GenericArguments.Any())
                {
                    args = typeDefinition.GenericArguments.Select(s => BuildTypeString(s)).ToList();
                }
                return TypeCache.BuildPredicateTypeName(typeDefinition.RawType, args);
            }



            var name = Defaults.NormalizeTypeName(typeDefinition, AllowedTypes, includeNamespace);
            if (typeDefinition.IsNullable)
            {
                return $"({name} | null)";
            }


            if (name != "any" && name != "any[]")
            {
                if (typeDefinition.GenericArguments.Any())
                {
                    if (name.EndsWith("[]"))
                    {
                        name = name.Substring(0, name.Length - 2);
                    }


                    name += $"<{String.Join(", ", typeDefinition.GenericArguments.Select(s => BuildTypeString(s)))}>";



                    if (typeDefinition.IsArray)
                    {
                        name = $"{name}[]";
                    }
                }
            }


            return name;
        }

        private string GetTypeString(TypeDefinition typeDefinition, bool includeNamespace = true)
        {

            var name = Defaults.NormalizeTypeName(typeDefinition, null, includeNamespace);
            if (typeDefinition.IsNullable)
            {
                return $"{name}?";
            }
            if (typeDefinition.GenericArguments.Any())
            {
                if (name.EndsWith("[]"))
                {
                    name = name.Substring(0, name.Length - 2);
                }
                name += $"<{String.Join(", ", typeDefinition.GenericArguments.Select(s => GetTypeString(s)))}>";
                if (typeDefinition.IsArray)
                {
                    name = $"{name}[]";
                }
            }

            return name;
        }


        private string BuildTypeDefinitionTypeString(TypeDefinition typeDefinition)
        {


            if (typeDefinition.TryGetPayload<NormalizedNonGenericTypeName>(out var normalizedNonGenericTypeName))
            {
                return normalizedNonGenericTypeName!.TypeName;
            }


            if (typeDefinition.FriendlyName == "System.Action" || typeDefinition.FriendlyName?.StartsWith("System.Action<") == true)
            {
                var args = new List<string>();
                if (typeDefinition.GenericArguments.Any())
                {
                    args = typeDefinition.GenericArguments.Select(s => BuildTypeString(s)).ToList();
                }
                return TypeCache.BuildActionTypeName(typeDefinition.RawType, args);
            }

            if (typeDefinition.FriendlyName?.StartsWith("System.Func<") == true)
            {
                var args = new List<string>();
                if (typeDefinition.GenericArguments.Any())
                {
                    args = typeDefinition.GenericArguments.Select(s => BuildTypeString(s)).ToList();
                }
                return TypeCache.BuildFuncTypeName(typeDefinition.RawType, args);
            }


            if (typeDefinition.FriendlyName?.StartsWith("System.Predicate<") == true)
            {
                var args = new List<string>();
                if (typeDefinition.GenericArguments.Any())
                {
                    args = typeDefinition.GenericArguments.Select(s => BuildTypeString(s)).ToList();
                }
                return TypeCache.BuildPredicateTypeName(typeDefinition.RawType, args);
            }



            var name = Defaults.NormalizeTypeName(typeDefinition, AllowedTypes, false);

            if (name != "any" && name != "any[]")
            {
                if (typeDefinition.GenericArguments.Any())
                {
                    if (name.EndsWith("[]"))
                    {
                        name = name.Substring(0, name.Length - 2);
                    }


                    name += $"<{String.Join(", ", typeDefinition.GenericArguments.Select(s => BuildTypeString(s)))}>";


                    if (typeDefinition.IsArray)
                    {
                        name = $"{name}[]";
                    }
                }
            }


            return name;
        }

        private NormalizedNonGenericTypeName? GetNormalizedNonGenericTypeName(TypeDefinition typeDefinition)
        {
            if (typeDefinition.FriendlyName == "System.Action" || typeDefinition.FriendlyName?.StartsWith("System.Action<") == true)
            {
                return null;
            }

            if (typeDefinition.FriendlyName?.StartsWith("System.Func<") == true)
            {
                return null;
            }


            if (typeDefinition.FriendlyName?.StartsWith("System.Predicate<") == true)
            {
                return null;
            }

            if (typeDefinition.GenericArguments.All(s => s.IsGeneric))
            {
                return null;
            }


            var name = Defaults.NormalizeTypeName(typeDefinition, AllowedTypes, false);

            if (name != "any" && name != "any[]")
            {
                if (typeDefinition.GenericArguments.Any())
                {
                    if (name.EndsWith("[]"))
                    {
                        name = name.Substring(0, name.Length - 2);
                    }

                    var genArgs = typeDefinition.GenericArguments.ToList();
                    var noneGenericArguments = typeDefinition.GenericArguments.Where(arg => !arg.IsGeneric);
                    genArgs.RemoveAll(arg => !arg.IsGeneric);

                    ///TODO: Whats this???
                    foreach (var noneGenericArgument in noneGenericArguments)
                    {
                        name += $"$${BuildTypeString(noneGenericArgument).Replace(".","$")}";
                    }

                    if (genArgs.Any())
                    {
                        name += $"<{String.Join(", ", genArgs.Select(s => BuildTypeString(s)))}>";
                    }

                    if (typeDefinition.IsArray)
                    {
                        name = $"{name}[]";
                    }
                }
            }


            return new NormalizedNonGenericTypeName(name, typeDefinition.Namespace);
        }
        public Dictionary<string, string> Render(DefinitionBuilder definitionBuilder)
        {


            //var typeDescriptions = new Dictionary<string, string>();
            var namespaceDefinition = new NamespaceDefinition();
            AllowedTypes = definitionBuilder.GetTypesToProcess().ToList();

            foreach (var calculatedType in AllowedTypes)
            {
                var tdesc = TypeDefinition.FromType(calculatedType, AllowedTypes);
                var gn = GetNormalizedNonGenericTypeName(tdesc);
                if (gn != null)
                {
                    tdesc.SetPayload<NormalizedNonGenericTypeName>(gn);
                }

                if (tdesc.IsGeneric)
                {
                    continue;
                }

                if (tdesc.IsArray)
                {
                    continue;
                }

                if (tdesc.RawType == typeof(char))
                {
                    continue;
                }



                var typeString = BuildTypeString(tdesc);
                if (!typeString.Contains("."))
                {
                    continue;
                }

                if (typeString.EndsWith("&") || typeString.EndsWith("*"))
                {
                    continue;
                }
                //if (!typeDescriptions.ContainsKey(typeString))
                //{
                //    typeDescriptions.Add(typeString, Render(tdesc));
                //}

                if (!string.IsNullOrWhiteSpace(tdesc.Namespace))
                {
                    var ns = namespaceDefinition.AddNamespaceDefinition(tdesc.Namespace);
                    //var nsParts = tdesc.Namespace.Split('.');

                    //var ns = namespaceDefinition;
                    //foreach (var nsPart in nsParts)
                    //{
                    //    var foundNs = ns.Namespaces.FirstOrDefault(n => n.Name == nsPart);
                    //    if (foundNs == null)
                    //    {
                    //        foundNs = new NamespaceDefinition
                    //        {
                    //            Name = nsPart
                    //        };
                    //        ns.Namespaces.Add(foundNs);
                    //    }

                    //    ns = foundNs;
                    //}

                    ns.Types.Add(tdesc);
                }

            }


            var extMethods = definitionBuilder.GetExtensionMethods();
            foreach (var methodInfo in extMethods)
            {
                var mi = MethodDefinition.FromMethodInfo(methodInfo);

                switch (mi.IsExtensionMethodFor?.Name.ToLower())
                {
                    case "string":
                        {
                            TypeCache.JsString.Methods.Add(mi);
                            break;
                        }
                    default:
                        {

                            var ns = namespaceDefinition.GetNameSpaceDefinition(mi.IsExtensionMethodFor.Namespace) ??
                                     namespaceDefinition.AddNamespaceDefinition(mi.IsExtensionMethodFor.Namespace);

                            if (!mi.IsExtensionMethodFor.HasPayload<NormalizedNonGenericTypeName>())
                            {
                                var normalizedTypeName = GetNormalizedNonGenericTypeName(mi.IsExtensionMethodFor);
                                if (normalizedTypeName != null)
                                {
                                    mi.IsExtensionMethodFor.SetPayload<NormalizedNonGenericTypeName>(normalizedTypeName);
                                }
                            }

                            var existingType = ns.Types.FirstOrDefault(t => t == mi.IsExtensionMethodFor);
                            if (existingType == null)
                            {
                                ns.Types.Add(mi.IsExtensionMethodFor);
                                existingType = ns.Types.FirstOrDefault(t => t == mi.IsExtensionMethodFor);
                            }

                            existingType.Methods.Add(mi);

                            break;
                        }
                }

            }

            var dict = new Dictionary<string, string>();

            foreach (var definition in namespaceDefinition.Namespaces.OrderBy(n => n.Name))
            {
                var def = Render(definition);
                dict.Add($"{definition.Name}.d.ts", def);
            }
            //var renderedTypes = namespaceDefinition.Namespaces.OrderBy(n => n.Name).Select(s => Render(s)).ToList();

            var builtInExtensions = RenderBuiltInExtensions();

            dict.Add("extensions.d.ts", builtInExtensions);

            //renderedTypes.Insert(0, builtInExtensions);

            return dict;


        }

        private string RenderBuiltInExtensions()
        {
            var definitions = new List<string>();

            if (TypeCache.JsString.Methods.Any())
            {
                definitions.Add(Render(TypeCache.JsString));
            }
            //foreach (var typeDefinition in TypeCache.Cache.Values)
            //{
            //    definitions.Add(Render(typeDefinition));
            //}



            return string.Join(Environment.NewLine, definitions);
        }

        public string Render(NamespaceDefinition namespaceDefinition, int indent = 0)
        {
            var indentString = GetIndentString(indent);
            var strbuilder = new StringBuilder();
            if (indent == 0)
            {
                strbuilder.Append("declare ");
            }
            strbuilder.AppendLine($"{indentString}namespace {namespaceDefinition.Name} {{");
            indent += 4;

            strbuilder.AppendLine();
            foreach (var namespaceDefinitionType in namespaceDefinition.Types.OrderBy(t => t.Name))
            {
                if (namespaceDefinitionType.IsStatic)
                {
                    continue;
                }

                if (namespaceDefinitionType.IsGeneric)
                {
                    continue;
                }

                if (namespaceDefinitionType.Name.EndsWith("&"))
                {
                    continue;
                }
                var tdRendered = Render(namespaceDefinitionType, indent);

                strbuilder.AppendLine(tdRendered);
            }

            foreach (var namespaceDefinitionNamespace in namespaceDefinition.Namespaces.OrderBy(n => n.Name))
            {
                var nsRendered = Render(namespaceDefinitionNamespace, indent);

                strbuilder.AppendLine(nsRendered);
            }

            strbuilder.AppendLine($"{indentString}}}");
            return strbuilder.ToString();
        }
    }

    internal record NormalizedNonGenericTypeName(string TypeName, string Namespace);
}
