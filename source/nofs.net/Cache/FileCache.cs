using System;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Fuse.Impl;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Utils;
using System.Text;

namespace Nofs.Net.Cache.Impl
{
    public class FileCache : IFileCache
    {
        private IFileObject _file;
        private ITranslatorStrategy _translator = null;
        private IMethodFilter _methodFilter;
        private MemoryBuffer _cache;
        private int _blockSize;
        private FileCacheManager _manager;
        private bool _dirty;
        private LogManager _log;

        public FileCache(
                LogManager log,
                IMethodFilter methodFilter,
                FileCacheManager manager,
                IFileObject file,
                int blockSize)
        {
            _log = log;
            _dirty = false;
            _manager = manager;
            _file = file;
            _blockSize = blockSize;
            _methodFilter = methodFilter;

        }

        public bool IsDirty()
        {
            return _dirty;
        }

        public IFileObject File()
        {
            return _file;
        }

        private ITranslatorStrategy Translator()  
        {
            if (_translator == null)
            {
                _translator = new TranslatorFactory(_methodFilter).CreateTranslator(_file);
            }
            return _translator;
        }

        private MemoryBuffer Cache() 
        {
            if (_cache == null)
            {
                _cache = new MemoryBuffer(_blockSize);
                string values = Translator().Serialize(_file);
                int readCount = 0;
                foreach (byte value in values)
                {
                    _cache.put(value);
                    readCount += 1;
                }
                _log.LogInfo("read " + readCount + " bytes into cache for file: " + File().FileName);
                _cache.setPosition(0);
            }
            return _cache;
        }

        
        public int GetFileSize()  
        {
            return Cache().getSize();
        }

        
        public void Commit()  
        {
            try
            {
                if (_cache != null && _dirty)
                {
                    StringBuilder buffer = new StringBuilder();
                    Cache().setPosition(0);
                    foreach (var value in Cache())
                    {
                        buffer.Append(value);
                    }
                    string data = buffer.ToString();
                    _log.LogInfo("commit " + data.Length + " bytes into " + File().FileName);
                    Translator().DeserializeInto(data, _file);
                    _manager.CommitToDB(_file);
                }
            }
            finally
            {
                _cache = null;
            }
        }


        public void Read(byte[] buffer, long offset)  
        {
            Cache().setPosition((int)offset);
            //int readMax = buffer.limit() - buffer.position();
            int readMax = buffer.Length;
            int bufferMax = Cache().getSize();
            int readCount = 0;
            for (int i = 0; i < readMax && i < bufferMax; i++)
            {
                readCount += 1;
                buffer[i] = _cache.get();
            }
            _log.LogInfo("read " + readCount + " bytes from cache");
        }

        
        public void Truncate(long length) 
        {
            _dirty = true;
            Cache().Trim((int)length);
        }

        
        public void Write(byte[] buffer, long offset)  
        {
            _dirty = true;
            Cache().setPosition((int)offset);
            //int writeMax = buffer.limit() - buffer.position();
            int writeMax = buffer.Length;
            int writeCount = 0;
            for (int i = 0; i < writeMax; i++)
            {
                writeCount += 1;
                Cache().put(buffer[i]);
            }
            _log.LogInfo("wrote " + writeCount + " bytes into cache");
        }

    }

}
