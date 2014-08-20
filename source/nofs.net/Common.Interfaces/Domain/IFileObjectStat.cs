using System;
using System.Collections.Generic;

namespace Nofs.Net.Common.Interfaces.Domain
{
    public interface IFileObjectStat : IDomainObject
    {
        int Mode
        {
            get;
            set;
        }
        int NLink
        {
            get;
            set;
        }
        int UID
        {
            get;
            set;
        }
        int GID
        {
            get;
            set;
        }
        int RDev
        {
            get;
            set;
        }
        long FileSize
        {
            get;
            set;
        }
        long Blocks
        {
            get;
            set;
        }
        int ATime
        {
            get;
            set;
        }
        int MTime
        {
            get;
            set;
        }
        int CTime
        {
            get;
            set;
        }

        bool Equals(IFileObjectStat stat);

        Guid ParentID
        {
            get;
        }

        string ParentName
        {
            get;
            set;
        }
        
        IFileObjectStat Clone(); 

        IExtendedAttribute GetXAttr(string name); 
        void AddXAttr(IExtendedAttribute attr);
        bool RemoveXAttr(IExtendedAttribute attr); 
        IEnumerable<IExtendedAttribute> GetAllXAttr();
    }

}
