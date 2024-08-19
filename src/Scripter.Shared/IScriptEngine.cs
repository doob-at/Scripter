using System;
using System.Threading.Tasks;

namespace doob.Scripter.Shared
{
    public interface IScriptEngine: IDisposable
    {

        Func<string, string> CompileScript { get; }

        bool NeedsCompiledScript { get; }


        void SetValue(string name, object value);
        string GetValueAsJson(string name);
        T? GetValue<T>(string name);

        object GetValue(string name);

        Task<object?> ExecuteAsync(string script);
        
        void Stop();

        object? ConvertToDefaultObject(object? value);
        object? JsonParse(string? json);
        string JsonStringify(object? value);


        void AddModuleParameterInstance(Type type, Func<object> factory);

        void AddTaggedModules(params string[] tags);

        T? GetModuleState<T>();

        ScripterFunction? GetFunction(string name);

        object InvokeFunction(string name, params object[] args);

    }

    public interface IScriptEngine<TOptions> : IScriptEngine where TOptions : IScriptEngineOptions
    {
    }
}
