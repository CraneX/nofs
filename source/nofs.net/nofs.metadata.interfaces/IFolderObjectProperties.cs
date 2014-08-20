using System;

namespace Nofs.Net.nofs.metadata.interfaces
{
    public interface IFolderObjectProperties
    {
        bool CanAdd();
        bool CanRemove();
        string CanAddMethod();
        string CanRemoveMethod();
        string ChildTypeFilterMethod();
    }
}
