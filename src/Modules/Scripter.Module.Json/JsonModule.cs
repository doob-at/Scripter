using doob.Scripter.Shared;

namespace doob.Scripter.Module.Json
{

    public class JsonModule : IScripterModule
    {
        private JsonConverter Json { get; }

        public JsonModule(IScriptEngine scriptEngine)
        {
            Json = new JsonConverter(scriptEngine);
        }

        public string Stringify(object obj)
        {
            return Json.Stringify(obj);
        }

        public object? Parse(string value)
        {
            return Json.Parse(value);
        }

        public string Flatten(string json)
        {
            var flattenedObject = Reflectensions.Json.Converter.Flatten(Reflectensions.Json.Converter.ToJObject(json));
            return Stringify(flattenedObject);
        }

        public object? Flatten(object obj)
        {
            var json = Json.Stringify(obj);
            return Parse(Flatten(json));
        }

        public string UnFlatten(string json)
        {
            var jObject = Reflectensions.Json.Converter.ToJObject(json);
            var dict = Reflectensions.Json.Converter.ToDictionary(jObject);
            var unflattenObject = Reflectensions.Json.Converter.Unflatten(dict);
            return Stringify(unflattenObject);
        }

        public object? UnFlatten(object obj)
        {
            var json = Json.Stringify(obj);
            return Parse(UnFlatten(json));
        }

        public string Beautify(string json)
        {
            return Reflectensions.Json.Converter.Beautify(json);
        }

        public string Minify(string json)
        {
            return Reflectensions.Json.Converter.Minify(json);
        }
    }


    public class JsonConverter
    {
        private readonly IScriptEngine _scriptEngine;

        public JsonConverter(IScriptEngine scriptEngine)
        {
            _scriptEngine = scriptEngine;
        }

        public object? Parse(string value)
        {
            return _scriptEngine.JsonParse(value);
        }

        public string Stringify(object value)
        {
            return _scriptEngine.JsonStringify(value);
        }
    }

}
