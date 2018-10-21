# AServer
AServer是基于asp.net core Kestrel封装的一个超迷你http服务器。它可以集成进你的Core程序里，用来快速的响应http请求，而不需要集成整个asp.net core mvc。
## 依赖
Microsoft.AspNetCore.Server.Kestrel 2.1.3  
Newtonsoft.Json 11.0.2
## 安装  
Nuget:PM> Install-Package Agile.AServer  
## 使用  
step 1：  
```var server = new Agile.AServer.Server();//实例化一个server```  
step 2:
```
server.AddHandler(new HttpHandler()
                    {
                        Method = "GET",
                        Path = "/api/user",
                        Handler = (req, resp) =>
                        {
                            return resp.Write("['kklldog','agile']");
                        }
                    });
 //通过AddHandler方法添加一个对请求的响应
```
step 3:  
```
server.Run();
```
