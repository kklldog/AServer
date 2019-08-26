using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Agile.AServer.utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Agile.AServer
{
    public interface IServer
    {
        IServer SetIP(string ip);
        IServer SetPort(int port);
        IWebHost Host { get; }

        IServer AddHandler(HttpHandler handler);

        IServer EnableCors(CorsOption option);

        IServer SetLinstenUrls(params string[] url);

        Task Run();

        Task Stop();
    }

    public class Server : IServer
    {
        private static object _lockObj = new object();
        private readonly List<HttpHandler> _handlers = new List<HttpHandler>();
        private readonly ConcurrentDictionary<string, HttpHandler> _handlersCache = new ConcurrentDictionary<string, HttpHandler>();
        private string[] _urls = new string[] { };
        private int _port = 5000;
        private string _ip = "localhost";
        private CorsOption _corsOption;

        public IWebHost Host { get; private set; }

        public IServer SetPort(int port)
        {
            _port = port;

            return this;
        }

        public IServer SetIP(string ip)
        {
            if (!string.IsNullOrEmpty(ip))
            {
                _ip = ip;
            }

            return this;
        }

        public IServer EnableCors(CorsOption option)
        {
            _corsOption = option;

            return this;
        }



        public Task Run()
        {
            string url = String.Format("http://{0}:{1}", _ip, _port);
            var urls = new List<string>();
            if (!_urls.Any())
            {
                urls.Add(url);
            }
            else
            {
                urls = _urls.ToList();
            }

            Host =
                 new WebHostBuilder()
                    .UseUrls(urls.ToArray())
                    .UseKestrel()
                    .Configure(app =>
                    {
                        app.Run(http =>
                        {

                            return Task.Run(() =>
                            {
                                var req = http.Request;
                                var resp = http.Response;
                                var method = http.Request.Method;
                                var path = req.Path;

                                var cacheKey = $"Request:{method}-{path}";
                                ConsoleUtil.DebugConsole(cacheKey);

                                //cors
                                var corsResult = CorsHandler.Handler(_corsOption, http);
                                if (corsResult != null)
                                {
                                    return corsResult;
                                }

                                _handlersCache.TryGetValue(cacheKey, out HttpHandler handler);
                                if (handler == null)
                                {
                                    handler = _handlers.FirstOrDefault(
                                        h => h.Method.Equals(method,StringComparison.OrdinalIgnoreCase) && PathUtil.IsMatch(path, h.Path));
                                    if (handler != null)
                                    {
                                        _handlersCache.TryAdd(cacheKey, handler);
                                    }
                                }

                                if (handler != null)
                                {
                                    try
                                    {
                                        return handler.Handler(new Request(req, handler.Path), new Response(resp));
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                        resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        Console.WriteLine(
                                            $"Response:{resp.StatusCode} {HttpStatusCode.InternalServerError}");

                                        return resp.WriteAsync("InternalServerError");
                                    }
                                }

                                resp.StatusCode = (int)HttpStatusCode.NotFound;
                                Console.WriteLine($"Response:{resp.StatusCode} {HttpStatusCode.NotFound}");

                                return resp.WriteAsync("NotFound");
                            });
                        });
                    })
                    .Build();
            var task = Host.StartAsync();

            urls.ForEach(u =>
            {
                Console.WriteLine($"AServer listen on {u} .");
            });

            return task;
        }

        public IServer AddHandler(HttpHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (string.IsNullOrEmpty(handler.Method))
            {
                throw new ArgumentNullException("handler.Method");
            }
            if (string.IsNullOrEmpty(handler.Path))
            {
                throw new ArgumentNullException("handler.Path");
            }
            if (handler.Handler == null)
            {
                throw new ArgumentNullException("handler.Handler");
            }

            if (_handlers.Any(h => h.Path.Equals(handler.Path, StringComparison.CurrentCultureIgnoreCase) &&
                                   h.Method == handler.Method))
            {
                throw new Exception($"{handler.Method} {handler.Path} only can be set 1 handler");
            }
            else
            {
                lock (_lockObj)
                {
                    _handlers.Add(handler);
                }
            }

            return this;
        }



        public Task Stop()
        {
            return Host?.StopAsync();
        }

        /// <summary>
        /// 如果设置了listen urls则ip，port不起作用。
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public IServer SetLinstenUrls(params string[] urls)
        {
            this._urls = urls ?? throw new ArgumentNullException("urls");
            return this;
        }
    }
}
