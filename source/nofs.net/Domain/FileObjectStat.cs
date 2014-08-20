using System;
using Nofs.Net.Common.Interfaces.Domain;
using System.Collections.Generic;

namespace Nofs.Net.Domain.Impl
{
    public class FileObjectStat : BaseDomainObject, IFileObjectStat
    {
        private Guid _parentID;
        private string _name;
        private int _mode;
        private int _nlink;
        private int _uid;
        private int _gid;
        private int _rdev;
        private long _fileSize;
        private long _blocks;
        private int _atime;
        private int _mtime;
        private int _ctime;

        public FileObjectStat(Guid parentID, string name)
            : this(parentID, name, FuseFtypeConstants.TYPE_FILE | 0755, 0, 0, 0, 0, 0, 0, 0, 0, 0)
        {

        }

        public FileObjectStat(Guid parentID, string name, int mode, int nlink, int uid, int gid, int rdev, long fileSize, long blocks, int atime, int mtime, int ctime)
        {
            _name = name;
            _parentID = parentID;
            _mode = mode;
            _nlink = nlink;
            _uid = uid;
            _gid = gid;
            _rdev = rdev;
            _fileSize = fileSize;
            _blocks = blocks;
            _atime = atime;
            _mtime = mtime;
            _ctime = ctime;
        }

        public IFileObjectStat Clone()
        {
            FileObjectStat stat = new FileObjectStat(_parentID, _name, _mode, _nlink, _uid, _gid, _rdev, _fileSize, _blocks, _atime, _mtime, _ctime);
            stat.Id = Id;
            stat.IsNew = IsNew;
            return stat;
        }


        public string ParentName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public Guid ParentID
        {
            get
            {
                return _parentID;
            }
        }

        public bool Equals(IFileObjectStat stat)
        {
            return Id.Equals(stat.Id);
        }


        public int ATime
        {
            get
            {
            return _atime;
            }
            set
            {
                 _atime = value;
            }
        }


        public long Blocks
        {
            get
            {
                return _blocks;
            }
            set
            {
                _blocks = value;
            }
        }


        public int CTime
        {
            get
            {
                return _ctime;
            }
            set
            {
                _ctime = value;
            }
        }


        public long FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
            }
        }


        public int GID
        {
            get
            {
                return _gid;
            }
            set
            {
                _gid = value;
            }
        }


        public int MTime
        {
            get
            {
                return _mtime;
            }
            set
            {
                _mtime = value;
            }
        }


        public int Mode
        {
            get
            {
                return _mode;
            }
            set 
            {
                _mode = value;
            }

        }


        public int NLink
        {
            get
            {
                return _nlink;
            }
            set
            {
                _nlink = value;
            }
        }


        public int RDev
        {
            get
            {
                return _rdev;
            }
            set
            {
                _rdev = value;
            }
        }


        public int UID
        {
            get
            {
                return _uid;
            }
            set
            {
                _uid = value;
            }
        }

        private List<IExtendedAttribute> _xattr = new List<IExtendedAttribute>();

        public void AddXAttr(IExtendedAttribute attr)
        {
            _xattr.Add(attr);
        }

        public IEnumerable<IExtendedAttribute> GetAllXAttr()
        {
            return _xattr;
        }

        public IExtendedAttribute GetXAttr(string name)
        {
            foreach (IExtendedAttribute attr in _xattr)
            {
                if (attr.Name.CompareTo(name) == 0)
                {
                    return attr;
                }
            }
            return null;
        }

        public bool RemoveXAttr(IExtendedAttribute attr)
        {
            if (!_xattr.Remove(attr))
            {
                bool success = false;
                foreach (IExtendedAttribute actual in _xattr)
                {
                    if (actual.Id.CompareTo(attr.Id) == 0)
                    {
                        success = _xattr.Remove(actual);
                        break;
                    }
                }
                return success;
            }
            return false;
        }
    }

}
