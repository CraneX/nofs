using System;
using System.Collections.Generic;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Fuse.Impl;
using Nofs.Net.nofs.metadata.interfaces;

namespace Nofs.Net.Cache.Impl
{
    public class FileCacheManager : IFileCacheManager
    {
        private IDomainObjectContainerManager _containerManager;
        private const int _blockSize = 1024;
        private Dictionary<Guid, IFileCache> _caches;
        private IMethodFilter _methodFilter;
        private LogManager _log;

        public FileCacheManager(IDomainObjectContainerManager containerManager, LogManager log, IMethodFilter methodFilter)
        {
            _containerManager = containerManager;
            _log = log;
            _caches = new Dictionary<Guid, IFileCache>();
            _methodFilter = methodFilter;
        }

        
        public void Flush(Guid objectID)
        {
            _caches.Remove(objectID);
        }

        public void Deallocate(IFileCache cache)
        {
            _caches.Remove(cache.File().Id);
        }

        
        public void DeallocateIfNotDirty(IFileCache cache)
        {
            if (cache.IsDirty())
            {
                Deallocate(cache);
            }
        }

        public void CommitToDB(IFileObject sender) 
        {
            Type cls = sender.GetValue().GetType();
            _containerManager.GetContainer(cls).ObjectChanged(sender.GetValue());
        }

        
        public IFileCache GetFileCache(IFileObject file) 
        {
            if (!_caches.ContainsKey(file.Id))
            {
                if (file.SupportsDirectIO())
                {
                    _caches.Add(file.Id, new DirectFileCache(file));
                }
                else
                {
                    _caches.Add(file.Id, new FileCache(_log, _methodFilter, this, file, _blockSize));
                }
            }
            return _caches[file.Id];
        }
    }
}
