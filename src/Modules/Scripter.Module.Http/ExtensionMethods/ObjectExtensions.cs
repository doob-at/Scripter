using System;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using doob.Reflectensions;

namespace doob.Scripter.Module.Http.ExtensionMethods
{
    public static class ObjectExtensions
    {
        
        public static ExpandoObject? ToExpandoObject(this object @object)
        {
            return Json.Converter.ToObject<ExpandoObject>(Json.Converter.ToJson(@object));
        }

        public static ExpandoObject? ToExpandoObject(this HttpHeaders headers)
        {
            return headers.ToDictionary(h => h.Key, h => String.Join("; ", h.Value)).ToExpandoObject();
        }
    }
}
