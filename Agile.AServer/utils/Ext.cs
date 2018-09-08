using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Agile.AServer.utils
{
    public static class Ext
    {
        public static dynamic ToDynamic(this IDictionary dictionary)
        {
            IDictionary<string,object> dy = new ExpandoObject();
            foreach (object key in dictionary.Keys)
            {
                var val = dictionary[key];
                var dyPropName = key.ToString();
                if (!dy.ContainsKey(dyPropName))
                {
                    dy.Add(dyPropName,val);
                }
            }

            return (ExpandoObject) dy;
        }
    }
}
