using System;


namespace Nofs.Net.Fuse
{
    public interface IFileHandler
    {
        //FuseException
        int rename(string from, string to);

        //FuseException
        int unlink(string path);

        //FuseException
        int mknod(string path, int mode, int rdev);
    }
}
