using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Agile.AServer;
using Agile.AServer.utils;
using Newtonsoft.Json;

namespace AServer.Test.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var server = new Agile.AServer.Server();
                server.EnableCors(new CorsOption()
                {
                    AccessControlAllowOrigins = "http://localhost:5000",
                    AccessControlAllowHeaders = "auth",
                    AccessControlAllowMethods = "GET,POST,PUT,DELETE"
                });
                server
                    .AddHandler(new HttpHandler()
                    {
                        Method = "GET",
                        Path = "/api/user",
                        Handler = (req, resp) =>
                        {
                            return resp.Write("['kklldog','agile']");
                        }
                    })
                    .AddHandler(new HttpHandler()
                    {
                        Method = "GET",
                        Path = "/api/user/:id",
                        Handler = (req, resp) =>
                        {
                            var id = req.Params.id;

                            return resp.Write($"userId:{id}");
                        }
                    })
                    .AddHandler(new HttpHandler()
                    {
                        Method = "GET",
                        Path = "/api/query",
                        Handler = (req, resp) =>
                        {
                            var name = req.Query.name;

                            var user = new
                            {
                                name = name,
                                id = "0001",
                            };

                            var json = JsonConvert.SerializeObject(user);
                            return resp.WriteJson(json);
                        }
                    })
                    .AddHandler(new HttpHandler()
                    {
                        Method = "POST",
                        Path = "/api/user",
                        Handler = (req, resp) =>
                        {
                            return resp.Write(req.BodyContent);
                        }
                    })
                    .AddHandler(new HttpHandler()
                    {
                        Method = "DELETE",
                        Path = "/api/user/:id",
                        Handler = (req, resp) =>
                        {
                            var headers = new List<KeyValuePair<string, string>>();
                            headers.Add(new KeyValuePair<string, string>("Content-Type", "charset=utf-8"));
                            return resp.Write($"user {req.Params.id} be deleted .", HttpStatusCode.OK, headers);
                        }
                    })
                    .AddHandler(new HttpHandler()
                    {
                        Method = "GET",
                        Path = "/api/ex",
                        Handler = (req, resp) =>
                        {
                            throw new Exception("ex");
                        }
                    })
                    .AddController<ApiController>()
                    ;
                await server.Run();
            });

            Console.Read();
        }
    }
}
