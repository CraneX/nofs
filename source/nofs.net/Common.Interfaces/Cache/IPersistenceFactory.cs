using System;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public interface IPersistenceFactory
    {
        IStatMapper CreateStatMapper(
                string objectStore, 
                string metdataStore,
                Type fileObjectStatClass, 
                ILogManager logManager);

        IDomainObjectContainerManager CreateContainerManager
                (
                string objectStore, 
                string metdataStore, 
                ILogManager logManager,
                IStatMapper statMapper, 
                IKeyCache keyCache, 
                IFileObjectFactory fileObjectFactory,
                IAttributeAccessor attributeAccessor);  

        IKeyCache CreateKeyCache(string objectStore, string metdataStore, ILogManager logManager);
    }

}
