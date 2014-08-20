using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nofs.Net.Fuse
{

    public interface IFuseOpenSetter
    {
        /**
        * Callback for filehandle API
        * <p/>
        * @param fh the filehandle to return from <code>open()<code> method.
        */
        void setFh(object fh);

        /**
         * Sets/gets the direct_io FUSE option for this opened file
         */
        bool isDirectIO();

        void setDirectIO(bool directIO);

        /**
         * Sets/gets keep_cache FUSE option for this opened file
         */
        bool isKeepCache();

        void setKeepCache(bool keepCache);
    }

}
