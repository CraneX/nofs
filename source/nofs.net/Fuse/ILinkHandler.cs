using System;

namespace Nofs.Net.Fuse
{
    public interface ILinkHandler
    {
        int readlink(string path, char[] link);
        int symlink(string linkTarget, string linkPath);
    }
}
