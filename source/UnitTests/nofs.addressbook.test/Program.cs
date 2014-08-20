using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace nofs.test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> runnerArgs = new List<string>();
            runnerArgs.Add(Assembly.GetExecutingAssembly().Location);

            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                case PlatformID.Win32S:
                    runnerArgs.Add("/framework=net-3.5");
                    break;
                default:
                    break;
            }

#if (DEBUG)
            string scope = ConfigurationManager.AppSettings["TestScope"];
            if (!string.IsNullOrEmpty(scope))
            {
                runnerArgs.Add(scope);
            }
#endif
            NUnit.ConsoleRunner.Runner.Main(runnerArgs.ToArray());

#if (DEBUG)
            Console.Out.WriteLine("\nPress Any key to exit.");
            Console.ReadKey();
#endif
        }
    }
}
