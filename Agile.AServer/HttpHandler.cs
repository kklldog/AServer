using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.AServer.utils;
using Microsoft.AspNetCore.Http;

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

        public Stream Body => HttpRequest.Body;

        private string _bodyContent;
        public string BodyContent
        {
            get
            {
                if (!string.IsNullOrEmpty(_bodyContent))
                {
                    return _bodyContent;
                }

                using (var readStream = new StreamReader(Body, Encoding.UTF8))
                {
                    _bodyContent = readStream.ReadToEnd();
                }

                return _bodyContent;
            }
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

        public Task Write(string responseContent, string contentType = "text/html")
        {
            HttpResponse.Headers.Add("Content-Type", contentType);
            return HttpResponse.WriteAsync(responseContent);
        }

        public Task WriteJson(string responseContent)
        {
            HttpResponse.Headers.Add("Content-Type", "application/json");
            return HttpResponse.WriteAsync(responseContent);
        }
    }

    public class HttpHandler
    {
        public string Method { get; set; }
        public string Path { get; set; }

        public Func<Request, Response, Task> Handler { get; set; }

    }
}
