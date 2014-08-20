using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nofs.Net.Common.Interfaces.Library
{
    public interface IListensToEvents
    {

        /**
         * Provides notification that the file has been closed by the host operating system. The file should be in a consistent state.
         */
        void Closed();

        /**
         * Provides notification that the file has been opened by the host operating system. The file should be in a consistent state.
         * @throws Exception 
         */
        void Opened();

        /**
         * Provides notification that the file is about to be deleted.
         */
        void Deleting();

        /**
         * Provides notification that the file has just been created
         */
        void Created();
    }

}
