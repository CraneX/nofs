using System;

namespace Nofs.Net.Exceptions
{
    public class NoFSPathInvalidException : Exception
    {
        public NoFSPathInvalidException(string path)
            : base("Path '" + path + "' is not valid")
        {
        }
    }
}
