using System;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Domain.Impl;

namespace Nofs.Net.Fuse.Impl
{
    public class ExtendedAttributeHandler : IExtendedAttributeHandler
    {
        private PathTranslator _lookup;
        private LockManager _lock;
        private LogManager _logger;
        private IStatMapper _statMapper;

        public ExtendedAttributeHandler(PathTranslator lookup, LockManager lockManager, IStatMapper statMapper, LogManager logger)
        {
            _lookup = lookup;
            _lock = lockManager;
            _logger = logger;
            _statMapper = statMapper;
        }

        public int getxattr(string path, string name, byte[] dst) 
        {
            int stat = 0;
            _logger.LogInfo("getxattr(" + path + "," + name + ")");
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
                    IExtendedAttribute attr = target.GetStat().GetXAttr(name);
                    if (attr == null)
                    {
                        stat = FuseErrno.ENOATTR;
                    }
                    else
                    {
                        Array.Copy(attr.Value, dst, Math.Min(attr.Value.Length, dst.Length));
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("getxattr(" + path + "," + name + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        public int getxattrsize(string path, string name, IFuseSizeSetter sizeSetter)
        {
            int stat = 0;
            _logger.LogInfo("getxattrsize(" + path + "," + name + ")");
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
                    IExtendedAttribute attr = target.GetStat().GetXAttr(name);
                    if (attr == null)
                    {
                        stat = FuseErrno.ENOATTR;
                    }
                    else
                    {
                        sizeSetter.setSize(attr.Value.Length);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("getxattrsize(" + path + "," + name + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        public int listxattr(string path, IXattrLister lister)  
        {
            int stat = 0;
            _logger.LogInfo("listxattr(" + path + ")");
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
                    foreach (IExtendedAttribute attr in target.GetStat().GetAllXAttr())
                    {
                        _logger.LogInfo("-->" + attr.Name);
                        lister.add(attr.Name);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("listxattr(" + path + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        public int removexattr(string path, string name)  
        {
            int stat = 0;
            _logger.LogInfo("removexattr(" + path + "," + name + ")");
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
                    IExtendedAttribute attr = target.GetStat().GetXAttr(name);
                    if (attr == null)
                    {
                        stat = FuseErrno.ENOATTR;
                    }
                    else
                    {
                        target.GetStat().RemoveXAttr(attr);
                        _statMapper.Save(target.GetStat());
                    }
                }
            }
            catch (Exception e)
            {
                throw new FuseException("removexattr(" + path + "," + name + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

        public int setxattr(string path, string name, byte[] value, int flags)// throws FuseException 
        {
            int stat = 0;
            _logger.LogInfo("setxattr(" + path + "," + name + ")");
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
                    IExtendedAttribute attr;
                    if ((int)XattrSupportType.XATTR_CREATE == flags)
                    {
                        attr = new ExtendedAttribute(target.GetStat().Id);
                        attr.Name = name;
                        target.GetStat().AddXAttr(attr);
                    }
                    else
                    {
                        attr = target.GetStat().GetXAttr(name);
                    }
                    attr.Value = value;
                    _statMapper.Save(target.GetStat());
                }
            }
            catch (Exception e)
            {
                throw new FuseException("setxattr(" + path + "," + name + "," + flags + ")", e);
            }
            finally
            {
                _lock.UnLock();
            }
            return stat;
        }

    }

}
