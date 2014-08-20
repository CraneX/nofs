using System;

namespace Nofs.Net.Common.Interfaces.Library
{
    public interface IWeakReference<T>
    {
        /**
         * Loads an object from the NOFS cache or database. There is no guarantee
         * that subsequent accesses on this method will or will not produce reference equality.
         * 
         * @return				a reference to the loaded object
         * @throws Exception	An exception can be thrown if there is a fault in the NOFS database
         */
        T Get();
    }

}
