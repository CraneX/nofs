using System;
using System.Collections.Generic;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.nofs.metadata.interfaces;

namespace Nofs.Net.Domain
{
    public class CustomStat : IFileObjectStat
    {
        private IFileObjectStat _base;
        private IAttributeAccessor _accessor;
        private object _obj;
        private IGetterSetterPair _atime;
        private IGetterSetterPair _mtime;
        private IGetterSetterPair _ctime;
        private IGetterSetterPair _mode;
        private IGetterSetterPair _uid;
        private IGetterSetterPair _gid;

        public CustomStat(IFileObjectStat baseObject, IAttributeAccessor accessor, object obj)
        {
            _base = baseObject;
            _accessor = accessor;
            _obj = obj;
        }

        public IFileObjectStat Inner()
        {
            return _base;
        }

        private IGetterSetterPair GetATime() 
        {
            if (_atime == null)
            {
                _atime = _accessor.GetLastAccessTimePair(_obj);
            }
            return _atime;
        }

        private IGetterSetterPair GetMTime() 
        {
            if (_mtime == null)
            {
                _mtime = _accessor.GetLastModifiedTimePair(_obj);
            }
            return _mtime;
        }

        private IGetterSetterPair GetCTime() 
        {
            if (_ctime == null)
            {
                _ctime = _accessor.GetCreateTimePair(_obj);
            }
            return _ctime;
        }

        private IGetterSetterPair GetMode() 
        {
            if (_mode == null)
            {
                _mode = _accessor.GetModePair(_obj);
            }
            return _mode;
        }

        private IGetterSetterPair GetUID() 
        {
            if (_uid == null)
            {
                _uid = _accessor.GetUIDPair(_obj);
            }
            return _uid;
        }

        private IGetterSetterPair GetGID() 
        {
            if (_gid == null)
            {
                _gid = _accessor.GetGIDPair(_obj);
            }
            return _gid;
        }

        public IFileObjectStat Clone() 
        {
            return new CustomStat(_base.Clone(), _accessor, _obj);
        }

        public bool Equals(IFileObjectStat stat)
        {
            return ParentID.Equals(stat.ParentID);
        }

        public Guid ParentID
        {
            get
            {
                return _base.ParentID;
            }
        }

        public string ParentName
        {
            get
            {
                return _base.ParentName;
            }
            set
            {
                _base.ParentName = value;
            }
        }

        public int ATime 
        {
            get
            {
                return GetATime().GetterExists()
                        ? (int)((DateTime)GetATime().Getter()).Ticks
                        : _base.ATime;

            }
            set
            {
                if (GetATime().SetterExists())
                {
                    GetATime().Setter(new DateTime(value));
                }
                _base.ATime = value;
            }
        }

        public long Blocks
        {
            get
            {
                return _base.Blocks;
            }
            set
            {
                _base.Blocks = value;
            }
        }

        public int CTime 
        {
            get
            {
                return GetCTime().GetterExists()
                    ? (int)((DateTime)GetCTime().Getter()).Ticks
                    : _base.CTime;
            }
            set
            {
                if (GetCTime().SetterExists())
                {
                    GetCTime().Setter(new DateTime(value));
                }
                _base.CTime = (value);
            }
        }

        public long FileSize
        {
            get
            {
                return _base.FileSize;
            }
            set
            {
                _base.FileSize = value;
            }
        }

        public int GID 
        {
            get
            {
                return GetGID().GetterExists()
                    ? ((int)GetGID().Getter())
                    : _base.GID;
            }
            set
            {
                if (GetGID().SetterExists())
                {
                    GetGID().Setter(value);
                }
                _base.GID = value;
            }
        }

        public int MTime
        {
            get
            {
                return GetMTime().GetterExists()
                    ? (int)((DateTime)GetMTime().Getter()).Ticks
                    : _base.MTime;
            }
            set
            {
                if (GetMTime().SetterExists())
                {
                    GetMTime().Setter(new DateTime(value));
                }
                _base.MTime = value;
            }
        }

        public int Mode 
        {
            get
            {
                return GetMode().GetterExists()
                    ? ((int)GetMode().Getter())
                    : _base.Mode;
            }
            set
            {
                if (GetMode().SetterExists())
                {
                    GetMode().Setter(value);
                }
                _base.Mode = value;
            }
        }

        public int NLink
        {
            get
            {
                return _base.NLink;
            }
            set
            {
                _base.NLink = value;
            }
        }

        public int RDev
        {
            get
            {
                return _base.RDev;
            }
            set
            {
                _base.RDev = value;
            }
        }

        public int UID
        {
            get
            {
                return GetUID().GetterExists()
                    ? ((int)GetUID().Getter())
                    : _base.UID;
            }
            set
            {
                if (GetUID().SetterExists())
                {
                    GetUID().Setter(value);
                }
                _base.UID = value;
            }
        }


        public Guid Id
        {
            get
            {
                return _base.Id;
            }
            set
            {
                _base.Id = value;
            }
        }

        public bool IsNew
        {
            get
            {
                return _base.IsNew;
            }
            set
            {
                _base.IsNew = value;
            }
        }

        public void SetIsNotNew() 
        {
            _base.SetIsNotNew();
        }

        public void AddXAttr(IExtendedAttribute attr)
        {
            _base.AddXAttr(attr);
        }

        public IEnumerable<IExtendedAttribute> GetAllXAttr()
        {
            return _base.GetAllXAttr();
        }

        public IExtendedAttribute GetXAttr(string name) 
        {
            return _base.GetXAttr(name);
        }

        public bool RemoveXAttr(IExtendedAttribute attr) 
        {
            return _base.RemoveXAttr(attr);
        }
    }

}
