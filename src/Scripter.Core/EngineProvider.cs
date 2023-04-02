using System;
using doob.Scripter.Shared;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Core
{
    public class EngineProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public EngineProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IScriptEngine GetEngine(string scriptLanguage)
        {
            return _serviceProvider.GetRequiredNamedService<IScriptEngine>(scriptLanguage);
        }

    }
}
