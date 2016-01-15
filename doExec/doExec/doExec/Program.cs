using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace doExec
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            var exeName = args[0];
            var str = string.Join(" ", args, 1, args.Length - 1);
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName, str);
            var process = Process.Start(startInfo);
            process.Close();
        }
    }
}
