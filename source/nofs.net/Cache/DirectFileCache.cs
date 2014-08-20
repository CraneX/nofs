using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;

namespace Nofs.Net.Cache.Impl
{
    public class DirectFileCache : IFileCache
    {
        private IFileObject _file;

        public DirectFileCache(IFileObject file)
        {
            _file = file;
        }

        public void Commit()
        {
        }

        public IFileObject File()
        {
            return _file;
        }

        public bool IsDirty()
        {
            return false;
        }

        public int GetFileSize()
        {
            return (int)_file.GetReadWriteInterface().DataSize();
        }

        public void Read(byte[] buffer, long offset) 
        {
            int readMax = buffer.Length; //buffer.limit() - buffer.position();
            _file.GetReadWriteInterface().Read(buffer, offset, readMax);
        }

        public void Truncate(long length)
        {
            _file.GetReadWriteInterface().Truncate(length);
        }

        public void Write(byte[] buffer, long offset)
        {
            int writeMax = buffer.Length; // buffer.limit() - buffer.position();
            _file.GetReadWriteInterface().Write(buffer, offset, writeMax);
        }
    }

}
