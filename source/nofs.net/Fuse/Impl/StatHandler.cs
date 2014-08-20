using System;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Domain;

namespace Nofs.Net.Fuse.Impl
{

    public class StatHandler : IStatHandler
    {
        private PathTranslator _lookup;
        private LockManager _lock;
        private LogManager _logger;
        private IFileCacheManager _cacheManager;
        private IStatMapper _statMapper;

        public StatHandler(IFileCacheManager cacheManager, PathTranslator lookup, LockManager lockManager, IStatMapper statMapper, LogManager logger)
        {
            _cacheManager = cacheManager;
            _lookup = lookup;
            _lock = lockManager;
            _logger = logger;
            _statMapper = statMapper;
        }


        public int chmod(string path, int mode)
        {
            int stat = 0;
            _logger.LogInfo("getattr(" + path + ")");
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(path);

                if (target == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else
                {
                    target.GetStat().Mode = (mode);
                    SaveStat(target.GetStat());
                }
            }
            catch (System.Exception e)
            {
                throw new FuseException("chmod(" + path + "," + mode + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }


        public int chown(string path, int uid, int gid)
        {
            int stat = 0;
            _logger.LogInfo("getattr(" + path + ")");
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(path);
                if (target == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else
                {
                    target.GetStat().UID = (uid);
                    target.GetStat().GID = (gid);
                    SaveStat(target.GetStat());
                }
            }
            catch (Exception e)
            {
                throw new FuseException("chown(" + path + "," + uid + "," + gid + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        public static int GetMode(IFileObject file)
        {
            int mode = 0;
            if (file.GetGenerationType() == GenerationType.DATA_FILE ||
               file.GetGenerationType() == GenerationType.EXECUTABLE)
            {
                mode = file.GetStat().Mode | FuseFtypeConstants.TYPE_FILE;
            }
            else if (file.GetGenerationType() == GenerationType.DOMAIN_FOLDER)
            {
                mode = file.GetStat().Mode | FuseFtypeConstants.TYPE_DIR;
            }
            else
            {
                throw new Exception("unknown generation type");
            }
            if (file.GetGenerationType() == GenerationType.EXECUTABLE || !file.CanWrite())
            {
                mode = mode & (~(FuseMode.S_IWUSR | FuseMode.S_IWOTH | FuseMode.S_IWGRP));
            }
            return mode;
        }

        private int GetFileSize(IFileObject target)
        {
            if (target.GetGenerationType() == GenerationType.DOMAIN_FOLDER)
            {
                return 0;
            }
            else
            {
                IFileCache cachedFile = _cacheManager.GetFileCache(target);
                int size = cachedFile.GetFileSize();
                _cacheManager.DeallocateIfNotDirty(cachedFile);
                return size;
            }
        }


        public int getattr(string path, IFuseGetattrSetter attr)
        {
            int stat = 0;
            _logger.LogInfo("getattr(" + path + ")");
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(path);
                if (target == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else
                {
                    IFileObjectStat statTmp = target.GetStat();
                    attr.set(
                            0, //inode
                            GetMode(target),
                            statTmp.NLink,
                            statTmp.UID,
                            statTmp.GID,
                            statTmp.RDev,
                        //statTmp.FileSize,
                            GetFileSize(target),
                            statTmp.Blocks,
                            statTmp.ATime,
                            statTmp.MTime,
                            statTmp.CTime);
                }
            }
            catch (Exception e)
            {
                throw new FuseException("getattr(" + path + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        private void SaveStat(IFileObjectStat stat)
        {
            if (stat is CustomStat)
            {
                _statMapper.Save(((CustomStat)stat).Inner());
            }
            else
            {
                _statMapper.Save((IFileObjectStat)stat);
            }
        }


        public int utime(string path, int atime, int mtime)
        {
            int stat = 0;
            _logger.LogInfo("getattr(" + path + ")");
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(path);
                if (target == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else
                {
                    target.GetStat().ATime = (atime);
                    target.GetStat().MTime = (mtime);
                    SaveStat(target.GetStat());
                }
            }
            catch (System.Exception e)
            {
                throw new FuseException("utime(" + path + "," + atime + "," + mtime + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

    }

}
