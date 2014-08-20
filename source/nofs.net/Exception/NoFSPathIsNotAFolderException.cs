using System;

namespace Nofs.Net.Exceptions
{
    public class NoFSPathIsNotAFolderException : Exception
    {
        public NoFSPathIsNotAFolderException(string path)
            : base("Path '" + path + "' is not a folder")
        {
        }
    }
}
