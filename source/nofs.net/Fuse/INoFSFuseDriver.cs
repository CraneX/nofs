using System.Collections.Generic;

namespace Nofs.Net.Fuse
{
    public interface INoFSFuseDriver
    {
        void CleanUp();
        IEnumerable<object> GetFSObjectsByType(string className);
        void DumpStatTables();
    }
}
