using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Agile.AServer
{
    public interface IServer
    {
        IServer SetPort(int port);
        IWebHost Host { get; }

        IServer AddHandler(HttpHandler handler);

        Task Run();

        Task Stop();
    }

    public class Server : IServer
    {
        private IDictionary<string, HttpHandler> _handlers = new Dictionary<string, HttpHandler>();
        private int _port = 5000;

        public IWebHost Host { get; private set; }

        public IServer SetPort(int port)
        {
            _port = 5000;

            return this;
        }

        public Task Run()
        {
            Host =
                new WebHostBuilder()
                    .UseKestrel(op => op.ListenAnyIP(_port))
                    .Configure(app =>
                    {
                        app.Run(http =>
                        {
                            var req = http.Request;
                            var resp = http.Response;

                            var path = req.Path;

                            _handlers.TryGetValue(path.Value.ToLower(), out HttpHandler handler);
                            if (handler != null)
                            {
                                try
                                {
                                    var result = handler.Handler(req, resp);
                                    return resp.WriteAsync(result);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);

                                    resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    return resp.WriteAsync("InternalServerError");
                                }
                            }

                            resp.StatusCode = (int)HttpStatusCode.NotFound;
                            return resp.WriteAsync("NotFound");
                        });
                    })
                    .Build();
            var task = Host.StartAsync();

            Console.WriteLine("AServer listen http requests now .");

            return task;
        }

        public IServer AddHandler(HttpHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (string.IsNullOrEmpty(handler.Path))
            {
                throw new ArgumentNullException("handler.Path");
            }
            if (handler.Handler == null)
            {
                throw new ArgumentNullException("handler.Handler");
            }

            if (!_handlers.TryAdd(handler.Path.ToLower(), handler))
            {
                throw new Exception($"request path:{handler.Path} only can be set 1 handler");
            }

            return this;
        }

        public Task Stop()
        {
            return Host?.StopAsync();
        }
    }
}
