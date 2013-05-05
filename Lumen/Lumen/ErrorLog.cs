using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lumen
{
    public static class ErrorLog
    {
        private const string LogFileName = "log.txt";

        public static void Log(string logMessage)
        {
            using (var w = File.AppendText(LogFileName)) {
                w.Write(Environment.NewLine + "Log ({0} {1}): ", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
                w.WriteLine();
                w.WriteLine("{0}", logMessage);
                w.WriteLine();
                w.WriteLine("\t-----");
            }
        }
    }
}
