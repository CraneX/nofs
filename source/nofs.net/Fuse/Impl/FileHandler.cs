using System;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Utils;

namespace Nofs.Net.Fuse.Impl
{

    public class FileHandler : IFileHandler
    {
        private PathTranslator _lookup;
        private LockManager _lock;
        private LogManager _logger;
        private DomainObjectCollectionHelper _helper;

        public FileHandler(PathTranslator lookup, DomainObjectCollectionHelper helper, LockManager lockManager, LogManager logger)
        {
            _lookup = lookup;
            _lock = lockManager;
            _logger = logger;
            _helper = helper;
        }

        
        public int mknod(string path, int mode, int rdev)// throws FuseException
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
                    else if (!target.CanMknod())
                    {
                        stat = FuseErrno.EACCES;
                    }
                    else
                    {
                        stat = mknod(PathUtil.GetChildName(path), target, mode);
                        if (target.GetValue() is IListensToEvents)
                        {
                            ((IListensToEvents)target.GetValue()).Created();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("mknod(" + path + "," + mode + "," + rdev + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        private int mknod(string childName, IFileObject parent, int mode)// throws Exception
        {
            if (parent.HasMethod())
            {
                _helper.AddChildObject(parent.Content, parent.GetMethod(), childName, MarkerTypes.DomainObject);
            }
            else
            {
                _helper.AddChildObject(parent.Content, childName, MarkerTypes.DomainObject);
            }
            return 0;
        }

        
        public int unlink(string path) 
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
                    if (targetParent.GetGenerationType() != GenerationType.DOMAIN_FOLDER)
                    {
                        stat = FuseErrno.ENOTSUPP;
                    }
                    else if (!targetParent.CanDeleteChildren())
                    {
                        stat = FuseErrno.EACCES;
                    }
                    else
                    {
                        if (target.GetValue() is IListensToEvents)
                        {
                            ((IListensToEvents)target).Deleting();
                        }
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
                throw new FuseException("ulink(" + path + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        
        public int rename(string from, string to)// throws FuseException 
        {
            int stat = 0;
            try
            {
                _lock.Lock();
                IFileObject target = _lookup.TranslatePath(from);
                IFileObject fromParent = _lookup.TranslatePathParent(from);
                IFileObject toParent = _lookup.TranslatePathParent(to);
                if (target == null || fromParent == null || toParent == null)
                {
                    stat = FuseErrno.ENOENT;
                }
                else if (!target.CanSetName() || !toParent.CanMknod() || !toParent.IsChildTypeCompatible(target))
                {
                    stat = FuseErrno.EACCES;
                }
                else
                {
                    _helper.MoveChildObject(target.GetValue(), fromParent.GetValue(), toParent.GetValue(), PathUtil.GetChildName(to));
                }
            }
            catch (Exception e)
            {
                throw new FuseException("rename(" + from + "," + to + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }
    }
}
