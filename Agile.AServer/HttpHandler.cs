using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Agile.AServer
{
    public class HttpHandler
    {
        public string Path { get; set; }

        public Func<HttpRequest, HttpResponse, string> Handler { get; set; }
    }
}
