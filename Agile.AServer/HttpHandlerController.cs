﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.AServer
{
    public class HttpHandlerController
    {
    }

    internal class ControllerManager
    {
        public static void Load<T>(IServer server) where T : HttpHandlerController
        {
            var methods = typeof(T).GetMethods();
            foreach (var methodInfo in methods)
            {
                var attr = methodInfo.GetCustomAttribute(typeof(HttpHandlerAttribute));
                if (attr is HttpHandlerAttribute && CheckHandlerParam(methodInfo.GetParameters()) && methodInfo.ReturnType == typeof(Task))
                {
                    var httpHandlerAttr = attr as HttpHandlerAttribute;
                    //找出具有httphandler attribute的方法
                    var handler = new HttpHandler
                    {
                        Method = httpHandlerAttr.Method,
                        Path = httpHandlerAttr.Path,
                        Handler = (request, response) =>
                        {
                            object[] parameters = new object[2];
                            parameters[0] = request;
                            parameters[1] = response;
                            var controllerInstance = Activator.CreateInstance(typeof(T));

                            var task = methodInfo.Invoke(controllerInstance, parameters);

                            return task as Task;
                        }
                    };

                    server.AddHandler(handler);
                }
            }
        }


        private static bool CheckHandlerParam(ParameterInfo[] pInfo)
        {
            if (pInfo.Length != 2) return false;
            if (pInfo[0].ParameterType != typeof(Request)) return false;
            if (pInfo[1].ParameterType != typeof(Response)) return false;
            return true;
        }
    }
}
