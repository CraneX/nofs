using System;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Utils;


namespace Nofs.Net.Fuse.Impl
{

    public class DirHandler : IDirHandler
    {
        private PathTranslator _lookup;
        private LockManager _lock;
        private LogManager _logger;
        private DomainObjectCollectionHelper _helper;

        public DirHandler(PathTranslator lookup, DomainObjectCollectionHelper helper, LockManager lockManager, LogManager logger)
        {
            _lookup = lookup;
            _helper = helper;
            _lock = lockManager;
            _logger = logger;
        }

        
        public int getdir(string path, IFuseDirFiller filler)
        {
            int stat = 0;
            _logger.LogInfo("getdir(" + path + ")");
            try
            {
                _lock.Lock();
                IFileObject targetFolder = _lookup.TranslatePath(path);
                if (targetFolder == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else if (targetFolder.GetGenerationType() == GenerationType.DOMAIN_FOLDER)
                {
                    foreach (IFileObject child in _lookup.GetPathChildren(path))
                    {
                        filler.add(child.FileName, child.Id.GetHashCode(), StatHandler.GetMode(child));
                    }
                }
                else
                {
                    _logger.LogError("getdir(" + path + ") is not a folder");
                    stat = FuseErrno.ENOTDIR;
                }
            }
            catch (Exception e)
            {
                throw new FuseException("getdir(" + path + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        
        public int mkdir(string path, int mode)  
        {
            int stat = 0;
            _logger.LogInfo("mknod(" + path + "," + mode + ")");
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(path);
                if (target != null)
                {
                    stat = FuseErrno.EEXIST;
                }
                else
                {
                    target = _lookup.TranslatePathParent(path);
                    if (target == null)
                    {
                        stat = FuseErrno.ENOENT;
                    }
                    else if (target.GetGenerationType() != GenerationType.DOMAIN_FOLDER)
                    {
                        stat = FuseErrno.ENOTSUPP;
                    }
                    else if (!target.CanMkdir())
                    {
                        stat = FuseErrno.EACCES;
                    }
                    else
                    {
                        stat = mkdir(PathUtil.GetChildName(path), target, mode);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("mknod(" + path + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        private int mkdir(string childName, IFileObject parent, int mode)  
        {
            if (parent.HasMethod())
            {
                _helper.AddChildObject(parent.Content, parent.GetMethod(), childName, MarkerTypes.FolderObject);
            }
            else
            {
                _helper.AddChildObject(parent.Content, childName, MarkerTypes.FolderObject);
            }
            return 0;
        }

        
        public int rmdir(string path)  
        {
            int stat = 0;
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
                    IFileObject targetParent = _lookup.TranslatePathParent(path);
                    if (target.GetGenerationType() != GenerationType.DOMAIN_FOLDER)
                    {
                        stat = FuseErrno.EBADF;
                    }
                    else if (targetParent.GetGenerationType() != GenerationType.DOMAIN_FOLDER)
                    {
                        stat = FuseErrno.ENOTSUPP;
                    }
                    else if (!targetParent.CanRmdir())
                    {
                        stat = FuseErrno.EACCES;
                    }
                    else
                    {
                        if (targetParent.HasMethod())
                        {
                            _helper.RemoveChildObject(targetParent.Content, targetParent.GetMethod(), target.GetValue());
                        }
                        else
                        {
                            _helper.RemoveChildObject(targetParent.Content, target.GetValue());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("rmdir(" + path + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

    }
}
