using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agile.AServer.MockServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //read mock.josn
            var mockjsonFile = AppDomain.CurrentDomain.BaseDirectory + "mock.json";
            if (File.Exists(mockjsonFile))
            {
                var mockString = File.ReadAllText(mockjsonFile);
                if (!string.IsNullOrEmpty(mockString))
                {
                    try
                    {
                        var server = new Server();

                        var dynamicMockObj = JsonConvert.DeserializeObject<dynamic>(mockString);
                        string host = dynamicMockObj.host;

                        //解析监听ip port
                        string ip = "";
                        int port = 5000;
                        if (!string.IsNullOrEmpty(host))
                        {
                            var hostArr = host.Split(':');
                            if (hostArr.Length == 2)
                            {
                                ip = hostArr[0];
                                int.TryParse(hostArr[1], out port);
                            }
                            else
                            {
                                ip = hostArr[0];
                            }
                        }
                        server
                            .SetIP(ip)
                            .SetPort(port);
                        //解析apis
                        foreach (dynamic apiDesc in dynamicMockObj.apis)
                        {
                            string method = apiDesc.method;
                            string url = apiDesc.url;
                            int statuCode = apiDesc.response.statusCode;
                            var headerKvs = new List<KeyValuePair<string, string>>();
                            var dictHeader = apiDesc.response.headers as JObject;
                            if (dictHeader != null)
                            {
                                
                                foreach (var property in dictHeader.Properties())
                                {
                                    var value = dictHeader.GetValue(property.Name).ToString();
                                    headerKvs.Add(new KeyValuePair<string, string>(property.Name, value));
                                }
                            }
                            string result = apiDesc.response.result.ToString();

                            var handler = new HttpHandler();
                            handler.Method = method;
                            handler.Path = url;
                            handler.Handler = (request, response) => response.Write(result, (HttpStatusCode)statuCode, headerKvs);

                            server.AddHandler(handler);

                            Console.WriteLine($"add handler {method} {url}");
                        }
                        //run
                        server.Run();
                        Console.WriteLine("mock server is running now !");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("load mock.json fail !");
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    Console.WriteLine("mock.json is empty !");
                }
            }
            else
            {
                Console.WriteLine("no mock.json file !");
            }

            Console.WriteLine("input bye to exit !");
            var readLine = Console.ReadLine();
            while (readLine != null && readLine.Equals("bye",StringComparison.CurrentCultureIgnoreCase))
            {
                readLine = Console.ReadLine();
            }
        }

    }
}
