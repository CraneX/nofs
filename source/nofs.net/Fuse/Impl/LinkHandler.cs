using System;

namespace Nofs.Net.Fuse.Impl
{
    public class LinkHandler : ILinkHandler
    {
        public LinkHandler()
        {
        }

        
        public int readlink(String path, char[] link) 
        {
            return FuseErrno.ENOTSUPP;
        }

        
        public int symlink(String linkTarget, String linkPath)  
        {
            return FuseErrno.ENOTSUPP;
        }
    }
}
