using doob.Scripter.Shared;

namespace doob.Scripter.Engine.Powershell
{
    public static class IScripterContextExtensions
    {
        public static IScripterContext AddPowerShellCoreEngine(this IScripterContext services)
        {
            return services.AddScripterEngine<PowerShellCoreEngine>();
        }
    }
}
