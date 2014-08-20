using System;

namespace Nofs.Net.Common.Interfaces.Domain
{
    public interface IExtendedAttribute : IDomainObject
    {
        string Name
        {
            get;
            set;
        }
        
        byte[] Value
        {
            get;
            set;
        }
        
    }
}
