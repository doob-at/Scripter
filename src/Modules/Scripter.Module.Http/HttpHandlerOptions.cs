using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace doob.Scripter.Module.Http
{
    public class HttpHandlerOptions
    {

        public Uri RequestUri { get; }
        public WebProxy? Proxy { get; set; }
        public bool IgnoreProxy { get; set; }

        public List<X509Certificate2> ClientCertificates { get; set; } = new List<X509Certificate2>();

        public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;

        public HttpHandlerOptions(Uri uri)
        {
            RequestUri = uri;
        }

    }
}