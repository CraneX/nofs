using System;

namespace Nofs.Net.Fuse
{
    public interface IStatHandler
    {
        int chmod(string path, int mode);
        int chown(string path, int uid, int gid);
        int getattr(string path, IFuseGetattrSetter attr);
        int utime(string path, int atime, int mtime);
    }

}
