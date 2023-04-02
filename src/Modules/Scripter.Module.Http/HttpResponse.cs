using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using doob.Reflectensions;
using doob.Reflectensions.ExtensionMethods;
using doob.Scripter.Module.Http.ExtensionMethods;
using doob.Scripter.Shared;

namespace doob.Scripter.Module.Http
{
    public class HttpResponse : IDisposable
    {

        private readonly HttpResponseMessage _httpResponseMessage;
        private readonly IScriptEngine _scriptEngine;

        public Version Version => _httpResponseMessage.Version;

        private GenericHttpContent? _content;
        public GenericHttpContent Content => _content ??= new GenericHttpContent(_httpResponseMessage.Content, _scriptEngine);

        //private SimpleDictionary<string>? _contentHeaders;
        //public SimpleDictionary<string> ContentHeaders => _contentHeaders ??= new SimpleDictionary<string>(_httpResponseMessage.GetHeaders());

        public HttpStatusCode StatusCode => _httpResponseMessage.StatusCode;
        public string? ReasonPhrase => _httpResponseMessage.ReasonPhrase;

        private Dictionary<string, string>? _headers;

        public Dictionary<string, string> Headers => _headers ??=
            _httpResponseMessage.GetHeaders().Merge(_httpResponseMessage.GetContentHeaders()); // new SimpleDictionary<string>(_httpResponseMessage.GetHeaders()).Merge(new SimpleDictionary<string>(_httpResponseMessage.GetContentHeaders()));


        private ExpandableObject? _trailingheaders;
        public ExpandableObject? TrailingHeaders => _trailingheaders ??= new ExpandableObject(_httpResponseMessage.TrailingHeaders);


        public bool IsSuccessStatusCode => _httpResponseMessage.IsSuccessStatusCode;
        public HttpResponse EnsureSuccessStatusCode()
        {
            _httpResponseMessage.EnsureSuccessStatusCode();
            return this;
        }

        public override string ToString() => _httpResponseMessage.ToString();


        internal HttpResponse(HttpResponseMessage httpResponseMessage, IScriptEngine scriptEngine)
        {
            _httpResponseMessage = httpResponseMessage;
            _scriptEngine = scriptEngine;
            //Content = new GenericHttpContent(_httpResponseMessage.Content);
        }


        public void Dispose()
        {
            _httpResponseMessage?.Dispose();
        }

        
    }
}