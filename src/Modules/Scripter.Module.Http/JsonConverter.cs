using System;
using doob.Reflectensions;
using doob.Reflectensions.JsonConverters;

namespace doob.Scripter.Module.Http
{
    public class Converter
    {
        private static readonly Lazy<Json> lazyJson = new Lazy<Json>(() => new Json()
            .RegisterJsonConverter<ExpandoObjectConverter>(0)
        );

        public static Json Json => lazyJson.Value;

    }
}
