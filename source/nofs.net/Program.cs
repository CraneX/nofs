using System;
using NConsoler;

namespace Nofs.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            Consolery.Run(typeof(Program), args);
        }

        [Action]
        public static void Run(
            [Required] string libraryName,
            [Required] string mountPoint
            )
        {
            using (var nofs = new Nofs4Net(mountPoint))
            {
                nofs.Start(libraryName);
            }
        }
    }
}
