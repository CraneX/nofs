using System;
using System.Collections.Generic;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public interface IRepresentationBuilder
    {
        void PopulateWith(string data);
        string TranslateToString();
        IFolderReference GetRoot();
        IFolderReference AddFolder(IFolderReference folder, string name);
        List<IFolderReference> GetChildren(IFolderReference folder);
        void SetFolderValue(IFolderReference folder, object value);
        string GetFolderValue(IFolderReference folder);
        IFolderReference FindChildByName(IFolderReference parent, string name);
    }
}
