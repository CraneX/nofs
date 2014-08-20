using System;

namespace Nofs.Net.Fuse
{
    public interface IXattrLister
    {
        void add(string xattrName);
    }
}
