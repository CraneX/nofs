using System;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;

namespace Nofs.Net.nofs.Db4o
{
    internal class WeakReference<T> : IWeakReference<T>
    {
        private Guid _id;
        private IKeyCache _keyCache;
        private IAttributeAccessor _accessor;
        private IDomainObjectContainer _container;
        private IDomainObjectContainerManager _manager;

        public WeakReference(
                Guid id,
                IKeyCache keyCache,
                IAttributeAccessor accessor,
                IDomainObjectContainer container,
                IDomainObjectContainerManager manager)
        {
            _id = id;
            _keyCache = keyCache;
            _accessor = accessor;
            _container = container;
            _manager = manager;
        }


        public T Get()
        {
            IKeyIdentifier keyId = _keyCache.GetByID(_id);
            T reference = (T)keyId.Reference;
            if (_accessor != null && reference != null)
            {
                _accessor.SetContainerIfAttributeExists((object)reference, (IDomainObjectContainer)_container);
                _accessor.SetContainerManagerIfAttributeExists(reference, _manager);
            }
            return reference;
        }
    }
}
