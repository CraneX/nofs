using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nofs.Net.Fuse
{
    public interface IFuseObjectTree
    {
        bool HasRootClass { get; }
        string TempFileNamePrefix { get; }
        IEnumerable<string> GetFiles(string path);
        IEnumerable<string> GetFolders(string directory);
        bool IsFolder(string path);
        bool IsExcuteFile(string file);
        bool IsFile(string path);
        void ExecuteFile(string path, params string[] paramsValue);
        byte[] GetFileData(string path);
        int GetFileLength(string path);
        bool RemoveFolder(string path);
        bool RemoveFile(string path);
        void ClearCache();
    }
}
