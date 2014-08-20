using System;

namespace Nofs.Net.nofs.metadata.interfaces
{
    public interface IMetadataFactory
    {
        IAttributeAccessor CreateAttributeAccessor(); 
        IMethodFilter CreateMethodFilter(); 
        INoFSClassLoader CreateClassLoader(string fileName);
    }

}
