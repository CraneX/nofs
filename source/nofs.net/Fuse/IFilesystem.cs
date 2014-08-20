using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nofs.Net.Fuse
{
    public interface Filesystem3
    {
        int getattr(string path, IFuseGetattrSetter getattrSetter);

        int readlink(string path, string link);

        int getdir(string path, IFuseDirFiller dirFiller);

        int mknod(string path, int mode, int rdev);

        int mkdir(string path, int mode);

        int unlink(string path);

        int rmdir(string path);

        int symlink(string from, string to);

        int rename(string from, string to);

        int link(string from, string to);

        int chmod(string path, int mode);

        int chown(string path, int uid, int gid);

        int truncate(string path, long size);

        int utime(string path, int atime, int mtime);

        int statfs(IFuseStatfsSetter statfsSetter);

        // if open returns a filehandle by calling IFuseOpenSetter.setFh() method, it will be passed to every method that supports 'fh' argument
        int open(string path, int flags, IFuseOpenSetter openSetter);

        // fh is filehandle passed from open
        int read(string path, object fh, byte[] buf, long offset);

        // fh is filehandle passed from open,
        // isWritepage indicates that write was caused by a writepage
        int write(string path, object fh, bool isWritepage, byte[] buf, long offset);

        // called on every filehandle close, fh is filehandle passed from open
        int flush(string path, object fh);

        // called when last filehandle is closed, fh is filehandle passed from open
        int release(string path, object fh, int flags);

        // Synchronize file contents, fh is filehandle passed from open,
        // isDatasync indicates that only the user data should be flushed, not the meta data
        int fsync(string path, object fh, bool isDatasync);
    }
}
