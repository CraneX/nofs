using Nofs.Net.Common.Interfaces.Domain;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public interface IFileCache
    {
        void Write(byte[] buffer, long offset);
        void Read(byte[] buffer, long offset);
        void Truncate(long length);
        void Commit();
        int GetFileSize();
        IFileObject File();
        bool IsDirty();
    }
}
