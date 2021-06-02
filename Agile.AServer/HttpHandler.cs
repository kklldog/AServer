using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Agile.AServer.utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Agile.AServer
{
    public class Request
    {
        private string _pathPattern;
        public Request(HttpRequest request, string pathPattern)
        {
            HttpRequest = request;
            _pathPattern = pathPattern;
        }

        public HttpRequest HttpRequest { get; }

        private dynamic _params;
        public dynamic Params => _params ?? (_params = PathUtil.MatchPathParams(HttpRequest.Path, _pathPattern));

        private dynamic _query;
        public dynamic Query => _query ?? (_query = DynamicQuery());

        private dynamic _header;
        public dynamic Header => _header ?? (_header = DynamicHeader());

        public Stream BodyStream => HttpRequest.Body;

        private string _bodyContent;

        public async Task<string> ReadBodyContent()
        {
            if (!string.IsNullOrEmpty(_bodyContent))
            {
                return _bodyContent;
            }

            using (var readStream = new StreamReader(BodyStream, Encoding.UTF8))
            {
                _bodyContent = await readStream.ReadToEndAsync();
            }

            return _bodyContent;
        }

        public async Task<T> Body<T>()
        {
            var body = await ReadBodyContent();
            if (string.IsNullOrEmpty(body))
            {
                return default(T);
            }

            T obj = JsonConvert.DeserializeObject<T>(body);

            return obj;
        }
        private dynamic DynamicHeader()
        {
            if (HttpRequest == null)
            {
                throw new ArgumentNullException();
            }

            var dict = new Dictionary<string, string>();
            foreach (var keyValuePair in HttpRequest.Headers)
            {
                if (!dict.ContainsKey(keyValuePair.Key))
                {
                    dict.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return dict.ToDynamic();
        }

        private dynamic DynamicQuery()
        {
            if (HttpRequest == null)
            {
                throw new ArgumentNullException();
            }
            var dict = new Dictionary<string, string>();
            foreach (var keyValuePair in HttpRequest.Query)
            {
                if (!dict.ContainsKey(keyValuePair.Key))
                {
                    dict.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return dict.ToDynamic();
        }
    }

    public class Response
    {
        public Response(HttpResponse response)
        {
            HttpResponse = response;
        }

        public HttpResponse HttpResponse { get; }

        public void AddHeader(string key, string value)
        {
            HttpResponse.Headers.Add(key, value);
        }

        public Task Write(string responseContent, HttpStatusCode statusCode, List<KeyValuePair<string, string>> headers)
        {
            HttpResponse.StatusCode = (int)statusCode;
            headers?.ForEach(h =>
            {
                if (HttpResponse.Headers.Any(h1 => h1.Key == h.Key))
                {
                    var value = HttpResponse.Headers[h.Key];
                    value += "; " + h.Value;
                    HttpResponse.Headers[h.Key] = value;
                }
                else
                {
                    HttpResponse.Headers.Add(h.Key, h.Value);
                }
            });

            ConsoleUtil.DebugConsole($"Response:{HttpResponse.StatusCode} {statusCode}");

            return HttpResponse.WriteAsync(responseContent);
        }


        public Task Write(string responseContent, string contentType = "text/html")
        {
            return Write(responseContent, HttpStatusCode.OK, new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Content-Type",contentType)
            });
        }

        public Task WriteJson(string responseContent)
        {
            return Write(responseContent, HttpStatusCode.OK, new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Content-Type","application/json")
            });
        }
    }

    public class HttpHandler
    {
        public string Method { get; set; }
        public string Path { get; set; }

        public Func<Request, Response, Task> Handler { get; set; }

    }
}
