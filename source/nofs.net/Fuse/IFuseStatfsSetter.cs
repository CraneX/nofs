using System;

namespace Nofs.Net.Fuse
{
    public interface IFuseStatfsSetter
    {
        void set(int blockSize, int blocks, int blocksFree, int blocksAvail, int files, int filesFree, int namelen);
    }
}
