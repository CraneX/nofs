using System;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public class NoFSSerializationException : Exception
    {
        public NoFSSerializationException(Exception inner)
            : base("NoFSSerializationException", inner)
        {
        }
    }
}
