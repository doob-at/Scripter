using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doob.Reflectensions.Common;
using doob.Scripter.Engine.Javascript;
using doob.Scripter.JintTsDefinition.Definitions;
using doob.Scripter.Shared;

namespace doob.Scripter.JintTsDefinition {
    public class TsDefinitionService {
        private readonly IScripter _scripter;

        private Dictionary<string, string>? Definitions { get; set; }
        private Dictionary<string, string>? Imports { get; set; }

        public TsDefinitionService(IScripter scripter) {
            _scripter = scripter;
        }

        private readonly object _tsDefinitionsLock = new object();
        public Dictionary<string, string> GetTsDefinitions() {

            lock (_tsDefinitionsLock) {
                if (Definitions != null) {
                    return new Dictionary<string, string>(Definitions);
                }

                var definitions = _scripter.ModuleRegistry.GetRegisteredModuleDefinitions().ToList();
                var defBuilder = new DefinitionBuilder();


                var engine = _scripter.EngineRegistry.GetRegisteredEngine("typescript");

                if (engine.Options is JavaScriptEngineOptions jOpts) {
                    defBuilder.AddExtensionMethods(jOpts.AllowedExtensionMethods);
                }

                foreach (var md in definitions) {

                    defBuilder.AddTypes(md.ModuleType);

                }

                Definitions = defBuilder.Render();
                var assembly = GetType().Assembly;

                Definitions["global.d.ts"] = assembly.ReadResourceAsString("global.d.ts")!;
                Definitions["lib.es2015.core.d.ts"] = assembly.ReadResourceAsString("lib.es2015.core.d.ts")!;
                Definitions["lib.es5.d.ts"] = assembly.ReadResourceAsString("lib.es5.d.ts")!;

                return new Dictionary<string, string>(Definitions);
            }

        }

        private readonly object _tsImportsLock = new object();

        public Dictionary<string, string> GetTsImports() {

            lock (_tsImportsLock) {
                if (Imports != null) {
                    return new Dictionary<string, string>(Imports);
                }
                var definitions = _scripter.ModuleRegistry.GetRegisteredModuleDefinitions().ToList();

                Imports = definitions.ToDictionary(
                    md => $"{md.Name}.ts",
                    md => {
                        var tsr = new TypeScriptRenderer();
                        var td = TypeDefinition.FromType(md.ModuleType);

                        Task.Delay(1000).GetAwaiter().GetResult();

                        var body = tsr.RenderBody(td, 0, (definition, s) => {
                            switch (definition) {
                                case MethodDefinition: {
                                        return $"export function {s}";
                                    }
                                case PropertyDefinition: {
                                        return $"export const {s}";
                                    }
                            }

                            return s;
                        });
                        return body;
                    });

                return new Dictionary<string, string>(Imports);
            }

        }

    }


}
