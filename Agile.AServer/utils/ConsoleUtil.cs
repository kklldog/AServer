using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.AServer.utils
{
    class ConsoleUtil
    {
        /// <summary>
        /// 写到debug控制台
        /// </summary>
        /// <param name="txt"></param>
        public static void DebugConsole(string txt)
        {
#if DEBUG
            Console.WriteLine(txt);
#endif
        }
    }
}
