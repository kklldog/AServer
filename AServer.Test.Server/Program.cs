using System;
using System.Threading.Tasks;
using Agile.AServer;
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
                        Method = "GET",
                        Path = "/api/ex",
                        Handler = (req, resp) =>
                        {
                            throw new Exception("ex");
                        }
                    });
                await server.Run();
            });

            Console.Read();
        }
    }
}
