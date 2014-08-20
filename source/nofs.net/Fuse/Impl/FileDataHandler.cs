using System;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;

namespace Nofs.Net.Fuse.Impl
{
    public class FileDataHandler : IFileDataHandler
    {
        private IFileCacheManager _cacheManager;
        private PathTranslator _lookup;
        private LockManager _lock;
        private LogManager _logger;

        public FileDataHandler(
                IFileCacheManager cacheManager,
                PathTranslator lookup,
                LockManager lockManager,
                LogManager logger)
        {
            _cacheManager = cacheManager;
            _lookup = lookup;
            _lock = lockManager;
            _logger = logger;
        }


        public void CleanUp()  
        {
        }

        
        public int open(string path, int flags, IFuseOpenSetter openSetter) 
        {
            _logger.LogInfo("open(" + path + ", " + flags + ")");
            int stat = 0;
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(path);
                if (target == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else if (target.GetGenerationType() == GenerationType.DOMAIN_FOLDER)
                {
                    stat = FuseErrno.EBADF;
                }
                else
                {
                    openSetter.setFh(target);
                    if (target.GetGenerationType() == GenerationType.DATA_FILE
                        && target.GetValue() is IListensToEvents)
                    {
                        ((IListensToEvents)target.GetValue()).Opened();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("open(" + path + "," + flags + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        
        public int release(string path, object fh, int flags) 
        {
            _logger.LogInfo("release(" + path + ")");
            int stat = 0;
            try
            {
                _lock.Lock();
                IFileObject target = (IFileObject)fh;
                IFileCache cache = _cacheManager.GetFileCache(target);
                cache.Commit();
                _cacheManager.Deallocate(cache);
                if (target.GetGenerationType() == GenerationType.DATA_FILE && target.GetValue() is IListensToEvents)
                {
                    ((IListensToEvents)target.GetValue()).Closed();
                }
            }
            catch (NoFSSerializationException)
            {
                stat = FuseErrno.EDOM;
            }
            catch (Exception e)
            {
                throw new FuseException("release(" + path + "," + flags + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        
        public void Sync() 
        {
        }

        
        public int flush(string path, object fileHandle)
        {
            return 0;
        }

        
        public int read(string path, object fh, byte[] buf, long offset) 
        {
            _logger.LogInfo("read(" + path + "," + offset + ")");
            int stat = 0;
            try
            {
                _lock.Lock();
                IFileObject target = (IFileObject)fh;
                IFileCache cache = _cacheManager.GetFileCache(target);
                int originalPosition = 0;//buf.position();
                cache.Read(buf, offset);
                _logger.LogInfo("--read " + (buf.Length - originalPosition) + " bytes");
            }
            catch (Exception e)
            {
                throw new FuseException("read(" + path + "," + offset + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        
        public int truncate(string path, long length)// throws FuseException 
        {
            _logger.LogInfo("truncate(" + path + "," + length + ")");
            int stat = 0;
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(path);
                if (target == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else if (target.GetGenerationType() == GenerationType.DOMAIN_FOLDER)
                {
                    stat = FuseErrno.EBADF;
                }
                else if (target.GetValue() is IProvidesUnstructuredData)
                {
                    ((IProvidesUnstructuredData)target.GetValue()).Truncate(length);
                }
                else
                {
                    stat = FuseErrno.ENOTSUPP;
                }
            }
            catch (Exception e)
            {
                throw new FuseException("truncate(" + path + "," + length + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        
        public int write(string path, object fh, bool isWritePage, byte[] buf, long offset) 
        {
            _logger.LogInfo("write(" + path + "," + offset + ")");
            int stat = 0;
            try
            {
                _lock.Lock();
                IFileObject target = (IFileObject)fh;
                IFileCache cache = _cacheManager.GetFileCache(target);
                cache.Write(buf, offset);
            }
            catch (Exception e)
            {
                throw new FuseException("write(" + path + "," + offset + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

    }

}
