using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Agile.AServer.utils
{
    public class PathUtil
    {
        public static dynamic MatchPathParams(string path, string pathPattern)
        {
            IsMatch(path, pathPattern, out dynamic pathParams);

            return pathParams;
        }

        private static bool IsMatch(string path, string pathPattern,out dynamic pathParams)
        {
            if (path == null || pathPattern == null)
            {
                throw new ArgumentNullException();
            }

            pathParams = new ExpandoObject();

            if (path.Equals(pathPattern, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            var pathArray = path.TrimEnd('/').Split('/');
            var patternArray = pathPattern.TrimEnd('/').Split('/');

            if (pathArray.Length != patternArray.Length)
            {
                return false;
            }

            var isMatch = true;
            var paramsDict = new Dictionary<string,string>();
            for (int i = 0; i < pathArray.Length; i++)
            {
                var pathNode = pathArray[i];
                var patternNode = patternArray[i];

                if (pathNode.Equals(patternNode, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                if (patternNode.StartsWith(":"))
                {
                    var paramName = patternNode.Substring(1, patternNode.Length - 1);
                    if (!paramsDict.ContainsKey(paramName))
                    {
                        paramsDict.Add(paramName, pathNode);
                    }
                    continue;
                }

                isMatch = false;
            }

            pathParams = paramsDict.ToDynamic();
            return isMatch;
        }

        public static bool IsMatch(string path, string pathPattern)
        {
           return IsMatch(path, pathPattern,out dynamic pathParams);
        }
    }
}
