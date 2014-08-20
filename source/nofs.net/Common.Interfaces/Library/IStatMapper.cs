using System;
using Nofs.Net.Common.Interfaces.Domain;

namespace Nofs.Net.Common.Interfaces.Library
{
    public interface IStatMapper
    {
        bool HasStat(Guid id);
        bool HasStat(Guid id, string name);
        void Rename(Guid id, string oldName, string newName);
        void Save(IFileObjectStat stat);
        IFileObjectStat Load(Guid id, string name);
        void Delete(Guid id);
        void CleanUp(bool cleanupManager);
        void DumpStatTables();
    }
}
