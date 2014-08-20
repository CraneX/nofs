using System;

namespace Nofs.Net.Common.Interfaces.Library
{
    /**
    * Provides access for NOFS to uniquely identify domain objects. 
    * This interface allows NOFS to provide weak references to other objects in the system
    */
    public interface IObjectWithID
    {
        Guid Id
        {
            get;
            set;
        }

    }
}
