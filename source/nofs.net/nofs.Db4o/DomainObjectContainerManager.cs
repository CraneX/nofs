using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;

namespace Nofs.Net.nofs.Db4o
{
    public class DomainObjectContainerManager : IDomainObjectContainerManager, IDisposable
    {
        private Dictionary<Type, IDomainObjectContainer> _containers;
        private IObjectContainer _db;
        private IKeyCache _keyCache;
        private IAttributeAccessor _accessor;
        private IStatMapper _statMapper;
        private IFileObjectFactory _fileObjectFactory;
        private IFileCacheManager _fileCacheManager;

        private DomainObjectContainerManager()
        {
            _containers = new Dictionary<Type, IDomainObjectContainer>();
        }

        public DomainObjectContainerManager
            (
                IObjectContainer db,
                IStatMapper statMapper,
                IKeyCache keyCache,
                IFileObjectFactory fileObjectFactory,
                IAttributeAccessor accessor)
            : this()
        {
            if (db == null)
            {
                throw new System.Exception("db is null");
            }
            _db = db;
            _statMapper = statMapper;
            _keyCache = keyCache;
            _accessor = accessor;
            _fileObjectFactory = fileObjectFactory;
        }

        public void SetFileCacheManager(IFileCacheManager cacheManager)
        {
            _fileCacheManager = cacheManager;
        }

        public int GetObjectCountForTesting()
        {
            int count = 0;

            foreach (IDomainObjectContainer container in _containers.Values)
            {
                count += container.GetObjectCountForTesting();
            }
            return count;
        }

        public void CleanUp()
        {
            Dispose();
        }

        public void Dispose()
        {
            _db.Close();

            if (_statMapper != null)
            {
                _statMapper.CleanUp(true);
            }
        }

        /// <summary>
        /// Call GetContianer by className, only search from map, 
        /// if can not find, throw exception
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public IDomainObjectContainer GetContainer(string className)
        {
            IDomainObjectContainer container;
            if (!_containers.TryGetValue(Type.GetType(className), out container))
            {
                throw new System.Exception("could not find type: " + className);
            }
            return container;
        }

        //
        /// <summary>
        /// Call GetContianer by Type , if can not find
        /// add container to manager;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IDomainObjectContainer GetContainer(Type type)
        {
            IDomainObjectContainer container;
            lock (_containers)
            {
                if (!_containers.TryGetValue(type, out container))
                {
                    container = new DomainObjectContainer(_statMapper, _keyCache, _fileObjectFactory, this, _accessor, _db, type);
                    container.SetFileCacheManager(_fileCacheManager);
                    _containers.Add(type, container);
                }
            }

            return container;
        }

    }
}
