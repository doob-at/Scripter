﻿using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using doob.Reflectensions;
using doob.Scripter.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx.Synchronous;

namespace doob.Scripter.Module.Http
{
    public class GenericHttpContent : IDisposable
    {

        private readonly HttpContent _httpContent;
        private readonly IScriptEngine _scriptEngine;
        
        public GenericHttpContent(HttpContent httpContent, IScriptEngine scriptEngine)
        {
            _httpContent = httpContent;
            _scriptEngine = scriptEngine;
        }

        public string AsText()
        {
            return _httpContent.ReadAsStringAsync().WaitAndUnwrapException();
        }

        [Obsolete("Use 'AsObjectFromJson' instead")]
        public object? AsObject()
        {
            return AsObjectFromJson();
        }
        
        public Stream AsStream()
        {
            return _httpContent.ReadAsStream();
        }


        public object? AsObjectFromJson()
        {
            return _scriptEngine.JsonParse(AsText());
        }

        public void Dispose()
        {
            _httpContent?.Dispose();
        }
    }
}