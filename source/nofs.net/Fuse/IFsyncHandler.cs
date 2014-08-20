using System;

namespace Nofs.Net.Fuse
{
    public interface IFsyncHandler
    {
        int fsync(string path, object fileHandle, Boolean isDataSync);
    }
}
