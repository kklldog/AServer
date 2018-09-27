using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Agile.AServer
{
    public class CorsOption
    {
        public string AccessControlAllowOrigins { get; set; } = "";

        public bool AccessControlAllowCredentials { get; set; }

        public string AccessControlAllowMethods { get; set; } = "";

        public string AccessControlAllowHeaders { get; set; } = "";

        public string AccessControlExposeHeaders { get; set; } = "";

        public int? AccessControlMaxAge { get; set; }

        public bool IsOriginAllow(string origin)
        {
            if (AccessControlAllowOrigins == "*")
            {
                return true;
            }

            return AccessControlAllowOrigins.Split(',').Contains(origin);
        }

        public bool IsMethodAllow(string method)
        {
            if (AccessControlAllowMethods == "*")
            {
                return true;
            }

            return AccessControlAllowMethods.Split(',').Contains(method);
        }

        public bool IsHeaderAllow(string header)
        {
            if (AccessControlAllowHeaders == "*")
            {
                return true;
            }

            var headers = header?.Split(',');
            var allowHeaders = AccessControlAllowHeaders.Split(',');
            var headerContain = true;
            if (headers == null)
            {
                headerContain = false;
            }
            else
            {
                foreach (var accessHeader in headers)
                {
                    if (!allowHeaders.Contains(accessHeader))
                    {
                        headerContain = false;
                        break;
                    }
                }
            }

            return headerContain;
        }
    }

    public class CorsHandler
    {
        public static Task Handler(CorsOption corsOption,HttpContext http)
        {
            var req = http.Request;
            var resp = http.Response;
            var method = http.Request.Method;
            if (req.Headers.TryGetValue("Origin", out StringValues origin))
            {
                //未设corsOption直接拒绝cors请求
                if (corsOption == null)
                {
                    //直接返回，虽然status=200，但是没有相关access-control-allow-xxx，这个请求会在浏览器端失败。
                    return resp.WriteAsync("");
                }
                //检查origin
                var originVal = origin.FirstOrDefault();
                if (!corsOption.IsOriginAllow(originVal))
                {
                    return resp.WriteAsync("");
                }

                //Access-Control-Allow-Origin必须
                resp.Headers.Add("Access-Control-Allow-Origin",
                    corsOption.AccessControlAllowOrigins == "*" ? "*" : originVal);
                if (corsOption.AccessControlAllowCredentials)
                {
                    //Access-Control-Allow-Credentials可选
                    resp.Headers.Add("Access-Control-Allow-Credentials", "true");
                }
                if (!string.IsNullOrEmpty(corsOption.AccessControlExposeHeaders))
                {
                    //Access-Control-Expose-Headers可选
                    resp.Headers.Add("Access-Control-Expose-Headers",
                        corsOption.AccessControlExposeHeaders);
                }

                if (method == "OPTIONS")
                {
                    //非简单请求
                    if (req.Headers.TryGetValue("Access-Control-Request-Method",
                        out StringValues accessMethods))
                    {
                        var accessMethodVal = accessMethods.FirstOrDefault();
                        if (corsOption.IsMethodAllow(accessMethodVal))
                        {
                            //非简单请求Access-Control-Allow-Methods必须
                            resp.Headers.Add("Access-Control-Allow-Methods",
                                corsOption.AccessControlAllowMethods);
                        }
                    }

                    if (req.Headers.TryGetValue("Access-Control-Request-Headers", out StringValues accessHeaders))
                    {
                        var accessHeaderVal = accessHeaders.FirstOrDefault();
                        if (corsOption.IsHeaderAllow(accessHeaderVal))
                        {
                            //如果Access-Control-Request-Headers存在，则Access-Control-Allow-Headers是必须的
                            resp.Headers.Add("Access-Control-Allow-Headers", corsOption.AccessControlAllowHeaders);
                        }
                    }

                    if (corsOption.AccessControlMaxAge.HasValue)
                    {
                        //Access-Control-Max-Age可选
                        resp.Headers.Add("Access-Control-Max-Age", corsOption.AccessControlMaxAge.ToString());
                    }

                    //OPTIONS预检需要直接返回
                    return resp.WriteAsync("");
                }
            }

            return null;
        }
    }
}
