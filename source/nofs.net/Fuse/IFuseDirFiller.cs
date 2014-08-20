using System;

namespace Nofs.Net.Fuse
{
    public interface IFuseDirFiller
    {
        void add(string name, long inode, int mode);
    }
}
