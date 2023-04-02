namespace doob.Scripter.Shared
{
    public interface IScripter
    {
        IScripterModuleRegistry ModuleRegistry { get; }
        IScripterEngineRegistry EngineRegistry { get; }
        
        IScriptEngine GetScriptEngine(string name);
    }
}