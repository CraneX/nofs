using System;
using Nofs.Net.Common.Interfaces.Domain;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public interface IFileCacheManager
    {
        void Flush(Guid objectID);
        IFileCache GetFileCache(IFileObject file);
        void Deallocate(IFileCache cache);
        void DeallocateIfNotDirty(IFileCache cache);
    }
}
