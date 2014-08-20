using System;

namespace Nofs.Net.Common.Interfaces
{
    public interface ILogManager
    {
        void LogDebug(string msg);

        void LogError(string msg);

        void LogInfo(string msg);

    }
}
