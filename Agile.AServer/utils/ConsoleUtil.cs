using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.AServer.utils
{
    class ConsoleUtil
    {
        public static void WriteToConsole(string txt)
        {
#if DEBUG
            Console.WriteLine(txt);
#endif
        }
    }
}
