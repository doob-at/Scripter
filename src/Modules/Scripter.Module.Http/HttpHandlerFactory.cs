using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace doob.Scripter.Module.Http
{
    public static class HttpHandlerFactory
    {
        private static readonly ConcurrentDictionary<ReadonlyHttpHandlerOptions, HttpMessageHandler> HttpHandlers = new ConcurrentDictionary<ReadonlyHttpHandlerOptions, HttpMessageHandler>();

        public static HttpMessageHandler Build(HttpHandlerOptions handlerOptions)
        {
            var roHttpHandlerOptions = new ReadonlyHttpHandlerOptions(handlerOptions);
            return HttpHandlers.GetOrAdd(roHttpHandlerOptions, _ => ValueFactory(handlerOptions));
        }

        private static HttpMessageHandler ValueFactory(HttpHandlerOptions handlerOptions)
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromSeconds(60),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(20),
                MaxConnectionsPerServer = 2,
                AutomaticDecompression = DecompressionMethods.All,
                
            };

            if (handlerOptions.Proxy != null)
            {
                socketsHandler.Proxy = handlerOptions.Proxy;
            }

            if (handlerOptions.IgnoreProxy)
            {
                socketsHandler.UseProxy = false;
            }
            
            if (handlerOptions.ClientCertificates != null)
            {
                foreach (var handlerOptionsClientCertificate in handlerOptions.ClientCertificates)
                {
                    if (socketsHandler.SslOptions.ClientCertificates == null)
                    {
                        socketsHandler.SslOptions.ClientCertificates = new X509CertificateCollection();
                    }
                    socketsHandler.SslOptions.ClientCertificates.Add(handlerOptionsClientCertificate);
                }
            }

            socketsHandler.SslOptions.RemoteCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            socketsHandler.SslOptions.AllowRenegotiation = true;
            socketsHandler.SslOptions.EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13 |SslProtocols.Tls11 | SslProtocols.Tls11 | SslProtocols.Ssl2 | SslProtocols.Ssl3;
            return socketsHandler;

        }

    }
}