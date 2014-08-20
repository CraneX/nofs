using System;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public interface IKeyCache
    {
        int CacheSize();
        IKeyIdentifier GetByID(Guid id);
        IKeyIdentifier GetByReference(object reference);
        void Add(IKeyIdentifier key);
        void Remove(IKeyIdentifier key);
        void Remove(object reference);
    }
}
