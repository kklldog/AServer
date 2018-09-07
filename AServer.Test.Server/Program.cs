using System;
using System.Threading.Tasks;
using Agile.AServer;

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
                        Path = "/api/user",
                        Handler = (req, resp) =>
                        {
                            return "kklldog";
                        }
                    })
                    .AddHandler(new HttpHandler()
                    {
                        Path= "/api/ex",
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
