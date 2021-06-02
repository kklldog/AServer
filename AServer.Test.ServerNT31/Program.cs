using System;
using System.Threading.Tasks;

namespace AServer.Test.ServerNT31
{
    class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var server = new Agile.AServer.Server();

            server.AddHandler(new Agile.AServer.HttpHandler() { 
                Method = "GET",
                Path = "/api/doget",
                Handler = (req, resp) => {
                    return resp.Write("doget");
                }
            });

            server.ShowLog = true;
            server.SetIP("127.0.0.1").SetPort(6000);
            server.Run();

            await Task.Delay(-1);
        }
    }
}
