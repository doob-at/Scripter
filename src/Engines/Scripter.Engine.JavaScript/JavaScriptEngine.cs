using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using doob.Reflectensions;
using doob.Scripter.Shared;
using Esprima;
using Esprima.Ast;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using Newtonsoft.Json.Linq;

namespace doob.Scripter.Engine.Javascript
{

    public class JavaScriptEngine : IScriptEngine<JavaScriptEngineOptions>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IScripter _scripter;
        public Func<string, string> CompileScript => s => s;
        public bool NeedsCompiledScript => false;

        public JavaScriptEngineOptions ScriptEngineOptions { get; }

        private Jint.Engine _engine;

        private readonly Dictionary<Type, Func<object>> _providedTypeFactories = new();
        private readonly List<string> _useTaggedModules = new();
        private readonly Dictionary<Type, object> _instantiatedModules = new();

        private static readonly ConcurrentDictionary<Type, string> EsModules = new();

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private ObjectInstance? _mainModule = null;

        public JavaScriptEngine(IServiceProvider serviceProvider, IScripter scripter, JavaScriptEngineOptions scriptEngineOptions)
        {
            _serviceProvider = serviceProvider;
            _scripter = scripter;
            ScriptEngineOptions = scriptEngineOptions;
            _engine = new Jint.Engine(ScriptEngineOptions.JintOptions.CancellationToken(_cancellationTokenSource.Token));
            Initialize();

        }

        private void Initialize()
        {
            _engine.SetValue("exit", new Action(Stop));
            _engine.SetValue("NewObject", new Func<string, object[], object?>(TypeHelper.CreateObject));
            _engine.SetValue("require", new Func<string, JsValue>(Require));
        }

        private StepMode EngineOnStep(object sender, DebugInformation e)
        {
            return StepMode.Over;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public object? ConvertToDefaultObject(object? value)
        {
            if (value == null)
                return null;

            var json = JsonStringify(value);
            return JsonParse(json);
        }

        public object? JsonParse(string? json)
        {
            if (json == null)
                return null;

            var val = JsValue.FromObject(_engine, json);
            return _engine.Realm.Intrinsics.Json.Parse(val, new[] { val });
        }

        public string JsonStringify(object? value)
        {
            switch (value)
            {
                case JsValue jsValue:
                {
                    return _engine.Realm.Intrinsics.Json.Stringify(jsValue, new[] { jsValue }).AsString();
                }
                case JToken jToken:
                    {
                        return jToken.ToString();
                    }
            }

            value ??= JValue.CreateNull();

            return Json.Converter.ToJson(value);
        }

        public void AddModuleParameterInstance(Type type, Func<object> factory)
        {
            _providedTypeFactories[type] = factory;
        }

        public void AddTaggedModules(params string[] tags)
        {
            _useTaggedModules.AddRange(tags);
        }

        public T? GetModuleState<T>()
        {
            var type = typeof(T);
            if (_instantiatedModules.ContainsKey(type))
            {
                return (T)_instantiatedModules[type];
            }

            return default;
        }

        public ScriptFunction? GetFunction(string name)
        {
            var val = InternalGetValue(name);

            if (val is ScriptFunctionInstance func)
            {
                var paramsTypeDict = new Dictionary<string, Type>();
                var functionDeclarationParams = func.FunctionDeclaration.Params;
                foreach (var functionDeclarationParam in functionDeclarationParams)
                {
                    if (functionDeclarationParam is Identifier identifier)
                    {
                        paramsTypeDict.Add(identifier.Name, typeof(UnknownType));
                    }
                }
                return new ScriptFunction(func.FunctionDeclaration?.Id?.Name ?? name, paramsTypeDict, this);
            }

            return null;
        }

        public object InvokeFunction(string name, params object[] args)
        {
            if (_mainModule != null)
            {
                return _engine.Invoke(_mainModule.Get(name), args);
            }
            return _engine.Invoke(name, args);
        }


        public void SetValue(string name, object value)
        {
            switch (value)
            {
                case string str:
                    _engine.SetValue(name, str);
                    break;
                case double dbl:
                    _engine.SetValue(name, dbl);
                    break;
                case bool b:
                    _engine.SetValue(name, b);
                    break;
                default:
                    var obj = JsValue.FromObject(_engine, value);
                    _engine.SetValue(name, obj);
                    break;
            }

        }

        public string GetValueAsJson(string name)
        {
            var value = InternalGetValue(name);
            return _engine.Realm.Intrinsics.Json.Stringify(value, new[] { value }).AsString();
        }

        public T? GetValue<T>(string name)
        {
            return Json.Converter.ToObject<T>(GetValueAsJson(name));
        }

        public object GetValue(string name)
        {
            return InternalGetValue(name);
        }

        private JsValue InternalGetValue(string name)
        {
            if (_mainModule != null)
            {
                return _mainModule.Get(name);
            }
            return _engine.GetValue(name);
        }

        public Task<object?> ExecuteAsync(string script)
        {
            return Task.Run(() => InternalExecute(script));

        }

        private object? InternalExecute(string script)
        {

            if (string.IsNullOrWhiteSpace(script))
                return null;


            try
            {
                AddModules();
                _engine.AddModule("__main__", script);
                _mainModule = _engine.ImportModule("__main__");
                return null;

            }
            catch (ExecutionCanceledException canceledException)
            {
                //ignore
            }
            catch (Exception exception)
            {
                var ex = exception.GetBaseException();
                throw ex;
            }

            return null;

        }


        private void AddModules()
        {
            var moduleDefinitions = _scripter.ModuleRegistry.GetRegisteredModuleDefinitions();

            if (_useTaggedModules?.Any() == true)
            {
                moduleDefinitions = moduleDefinitions.Where(md => md.Tags.Count == 0 || md.Tags.Any(argTag => _useTaggedModules.Contains(argTag, StringComparer.OrdinalIgnoreCase)));
            }

            foreach (var moduleDefinition in moduleDefinitions)
            {
                var src = EsModules.GetOrAdd(moduleDefinition.ModuleType, type =>
                {
                    var methods = moduleDefinition.ModuleType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                    var properties =
                        moduleDefinition.ModuleType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    var sourceParts = new StringBuilder();
                    sourceParts.AppendLine($"var __{moduleDefinition.Name} = require('{moduleDefinition.Name}');");
                    foreach (var methodInfo in methods)
                    {
                        sourceParts.AppendLine($"export function {methodInfo.Name}() {{ return __{moduleDefinition.Name}.{methodInfo.Name}(...arguments); }}");
                    }

                    foreach (var propertyInfo in properties)
                    {
                        sourceParts.AppendLine($"export var {propertyInfo.Name} = __{moduleDefinition.Name}.{propertyInfo.Name}");
                    }

                    return sourceParts.ToString();
                });
                
                _engine.AddModule(moduleDefinition.Name, src);
            }
            
        }

        private JsValue Require(string value) {
            var inst = _scripter.ModuleRegistry.BuildModuleInstance(value, _serviceProvider, this, _providedTypeFactories, _useTaggedModules);
            _instantiatedModules[inst.GetType()] = inst;
            return JsValue.FromObject(_engine, inst);
        }

        public void Dispose()
        {
            _engine = null!;
        }


    }
    
}
