using System;
using doob.Scripter.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace doob.Scripter.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScripter(this IServiceCollection services, Action<ScripterBuilder> options)
        {

            services.TryAddTransient<IScripter>(sp =>
            {
                var builder = BuildScripter(options);
                return new Scripter(sp, builder._moduleRegistry, builder._engineRegistry);
            });
            return services;
        }

        public static IServiceCollection AddNamedScripter(this IServiceCollection services,string name, Action<ScripterBuilder> options)
        {
            services.TryAddNamedTransient<IScripter>(name, sp =>
            {
                var builder = BuildScripter(options);
                return new Scripter(sp, builder._moduleRegistry, builder._engineRegistry);
            });
            return services;
        }

        private static ScripterBuilder BuildScripter(Action<ScripterBuilder> options)
        {
            var scripterModuleRegistry = new ScripterModuleRegistry();
            var scripterEngineRegistry = new ScripterEngineRegistry();
            var scripterBuilder = new ScripterBuilder(scripterModuleRegistry, scripterEngineRegistry);
            options(scripterBuilder);

            return scripterBuilder;
        }

    }
}
