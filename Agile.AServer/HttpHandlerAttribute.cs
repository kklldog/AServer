using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.AServer
{
    public class HttpHandlerAttribute:Attribute
    {
        public HttpHandlerAttribute(string path, string method)
        {
            Path = path;
            Method = method;
        }

        public string Method { get; private set; }

        public string Path { get; private set; }


    }
}
