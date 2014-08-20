using System;


namespace Nofs.Net.Fuse
{
    public interface IFileDataHandler
    {
        //FuseException
        int open(string path, int flags, IFuseOpenSetter openSetter);

        //FuseException
        int release(string path, object fh, int flags);

        //FuseException
        int read(string path, object fh, byte[] buf, long offset);

        //FuseException
        int write(string path, object fh, Boolean isWritePage, byte[] buf, long offset);

        //FuseException
        int flush(string path, object fileHandle);

        //FuseException
        int truncate(string path, long length);

        //Exception
        void CleanUp();

        //FuseException
        void Sync();
    }
}
