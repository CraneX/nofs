using System;
using Nofs.Net.Common.Interfaces;
using log4net;

namespace Nofs.Net.Fuse.Impl
{
    public class LogManager : ILogManager
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(LogManager));

        public void LogDebug(string msg)
        {
            log.Debug(msg);
        }

        public void LogError(string msg)
        {
            log.Error(msg);
        }

        public void LogInfo(string msg)
        {
            log.Info(msg);

        }

        public static ILog GetLogger(Type type)
        {
            return log4net.LogManager.GetLogger(type);
        }

        public static void LogInfo(ILog log, string msg)
        {
            log.Info(msg);
#if (DEBUG)
            Console.WriteLine(msg);
#endif
        }

    }

}
