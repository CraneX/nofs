using System;
using Nofs.Net.Common.Interfaces.Cache;

namespace Nofs.Net.Common.Interfaces.Library
{
    public interface IDomainObjectContainerManager
    {
        /**
         * Provides access to other NOFS containers for an object type.
         * 
         * @param type			The object type of the container
         * @return				The container for the object type
         * @throws Exception
         */
        IDomainObjectContainer GetContainer(Type type);
        IDomainObjectContainer GetContainer(string className);

        void CleanUp();

        void SetFileCacheManager(IFileCacheManager fileCacheManager);

        int GetObjectCountForTesting();
    }
}
