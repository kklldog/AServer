using System;
using System.Threading.Tasks;

namespace AServer.Test.ServerNT31
{
    class testClass
    {
       public string name { get; set; }
    }
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

            server.AddHandler(new Agile.AServer.HttpHandler()
            {
                Method = "POST",
                Path = "/api/dopost",
                Handler = async (req, resp) => {
                    var body = await req.ReadBodyContent();
                    var obj = await req.Body<testClass>();
                    await resp.Write(obj.name);
                }
            });

            server.SetLinstenUrls("http://localhost:8989");
            server.Run();

            await Task.Delay(-1);
        }
    }
}
