using System;

namespace Nofs.Net.Exceptions
{
    public class NoFSDuplicateNameException : Exception
    {
        public NoFSDuplicateNameException(string path, string name)
            : base("Duplicate file object name found at path '" + path + "' for name '" + name + "'")
        {
        }
    }
}
