using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Db4objects.Db4o;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;
using Db4objects.Db4o.Config;

namespace Nofs.Net.nofs.Db4o
{
    public class DomainObjectContainer : IDomainObjectContainer
    {
        private Type _type;
        private IObjectContainer _db;
        private IKeyCache _keyCache;
        private IAttributeAccessor _accessor;
        private IStatMapper _statMapper;
        private IFileObjectFactory _fileObjectFactory;
        private IFileCacheManager _fileCacheManager;
        private IDomainObjectContainerManager _manager;
        private bool _init = false;

        public DomainObjectContainer(
                IStatMapper statMapper,
                IKeyCache keyCache,
                IFileObjectFactory fileObjectFactory,
                IDomainObjectContainerManager manager,
                IAttributeAccessor accessor,
                IObjectContainer db,
                Type type)
        {
            _statMapper = statMapper;
            _keyCache = keyCache;
            _fileObjectFactory = fileObjectFactory;
            _manager = manager;
            _accessor = accessor;
            _db = db;
            _type = type;
        }

        private void Init()
        {
            if (!_init)
            {
                _init = true;
                foreach (object obj in _db.Query(_type.GetType()))
                {
                    SetAccessor(obj);
                }
            }
        }

        public void SetFileCacheManager(IFileCacheManager cacheManager)
        {
            _fileCacheManager = cacheManager;
        }

        public IEnumerable<object> GetAllInstances()
        {
            foreach (object o in _db.Query().Execute())
            {
                yield return o;
            }
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
           // Init();

            foreach (T t in _db.Query<T>(_type))
            {
                yield return t;
            }
        }

        public bool IsPersistent<T>(T sender)
        {
            return null != _db.Query<T>(delegate(T t)
                    {
                        return t.Equals(sender);
                    }
                );
        }

        public void MakePersistent<T>(T sender)
        {
            if (!IsPersistent(sender))
            {
                _keyCache.Add(new KeyIdentifier(sender));

                StoreToDb(sender);
            }
        }

        public IKeyIdentifier GetIdentifier<T>(T sender)
        {
            return _keyCache.GetByReference(sender);
        }

        private Guid GetIdentifierId<T>(T sender)
        {
            IKeyIdentifier ki = GetIdentifier(sender);
            return ki == null ? Guid.Empty : ki.Id;
        }

        public T GetObject<T>(IKeyIdentifier key)
        {
            return (T)key.Reference;
        }

        public IWeakReference<T> GetWeakReference<T>(Guid id)
        {
            return new WeakReference<T>(id, _keyCache, _accessor, this, _manager);
        }

        public void ObjectRenamed<T>(T sender, string oldName, string newName)
        {
            if (_fileCacheManager != null)
            {
                _fileCacheManager.Flush(_keyCache.GetByReference(sender).Id);
            }

            if (_statMapper != null)
            {
                _statMapper.Rename(_keyCache.GetByReference(sender).Id, oldName, newName);
            }

            StoreToDb(sender);
        }

        public void Remove<T>(T sender)
        {
            if (_statMapper != null)
            {
                _statMapper.Delete(GetIdentifier(sender).Id);
            }

            if (_keyCache != null)
            {
                _keyCache.Remove(sender);
            }

            _db.Delete(sender);
        }

        public int GetObjectCountForTesting()
        {
            return _db.Query(_type).Count;
        }

        public IEnumerable<T> GetByExample<T>(object example)  
        {
            foreach (object result in _db.Query<T>())
            {
                T obj = (T)result;
                yield return obj;
            }
        }

        public object GetByExample(object example)
        {
            foreach (object obj in _db.QueryByExample(example))
            {
                if (obj != example)
                {
                    return obj;
                }
            }
            return null;
        }

        public T NewPersistentInstance<T>()
        {
            return (T)NewPersistentInstance();
        }

        public object NewPersistentInstance()
        {
            object sender;
            try
            {
                sender = Activator.CreateInstance(_type);
            }
            catch (Exception ie)
            {
                throw new System.Exception("Failed to create instance of class type = " + _type.Name
                    + "\r\nException"
                    + ie.ToString()
                    );
            }
            IKeyIdentifier keyId = new KeyIdentifier(sender);
            if (sender is IObjectWithID)
            {
                ((IObjectWithID)sender).Id = keyId.Id;
            }
            SetAccessor(sender);
            StoreToDb(sender);
            if (_keyCache != null)
            {
                _keyCache.Add(new KeyIdentifier(sender));
            }
            return sender;
        }

        public void ObjectChanged<T>(T sender)
        {
            if (_statMapper != null
                && !_statMapper.HasStat(_keyCache.GetByReference(sender).Id))
            {
                CreateAndSaveStatObjects(sender);
            }
            StoreToDb(sender);
        }


        public void UpdateStatObjects<T>(T sender, string oldName, string newName)
        {
            Guid id = _keyCache.GetByReference(sender).Id;
            if (_statMapper != null)
            {
                IFileObjectStat actualStat = _statMapper.Load(id, oldName);
                actualStat.ParentName = newName;
                _statMapper.Save(actualStat);
            }
        }

        private MethodInfo FindMethod<T>(T sender, string name)
        {
            foreach (MethodInfo method in sender.GetType().GetMethods())
            {
                if (method.Name.CompareTo(name) == 0 ||
                   (method.Name.StartsWith("get") && method.Name.Substring(3).CompareTo(name) == 0))
                {
                    return method;
                }
            }
            throw new System.Exception("could not find method named '" + name + "'");
        }

        public void CreateAndSaveStatObjects<T>(T sender)
        {
            Dictionary<string, IFileObjectStat> stats = new Dictionary<string, IFileObjectStat>();
            IFileObjectStat parentFileStat = _fileObjectFactory.BuildStat(sender);
            if ((parentFileStat.Mode & FuseFtypeConstants.TYPE_DIR) > 0)
            {
                IEnumerable<string> childNames = _fileObjectFactory.GetChildNames(sender);
                if (_accessor.IsFSRoot(sender))
                {
                    string objectName = _accessor.GetNameFromObject(sender);
                    stats.Add(objectName, parentFileStat);
                }

                if (childNames.Count() == 0)
                {
                    string objectName = _accessor.GetNameFromObject(sender);
                    stats.Add(objectName, parentFileStat);
                }
                else
                {
                    foreach (string childName in childNames)
                    {
                        MethodInfo method = FindMethod<T>(sender, childName);
                        IFileObjectStat childStat = _fileObjectFactory.BuildStat(sender, method);
                        stats.Add(childName, childStat);
                    }
                }
            }
            else if ((parentFileStat.Mode & FuseFtypeConstants.TYPE_FILE) > 0)
            {
                string objectName = _accessor.GetNameFromObject(sender);
                stats.Add(objectName, parentFileStat);
            }

            foreach (KeyValuePair<string, IFileObjectStat> pair in stats)
            {
                _statMapper.Save(pair.Value);
            }

            if (!_statMapper.HasStat(_keyCache.GetByReference(sender).Id, parentFileStat.ParentName))
            {
                //throw new Exception("stat wasn't saved!");
                _statMapper.Save(parentFileStat);
            }
        }

        public T NewTransientInstance<T>()
        {
            return (T)NewTransientInstance();
        }

        public object NewTransientInstance()
        {
            return Activator.CreateInstance(_type);
        }

        private void SetAccessor(object sender)
        {
            if (_accessor != null)
            {
                _accessor.SetContainerIfAttributeExists(sender, this);
                _accessor.SetContainerManagerIfAttributeExists(sender, _manager);
            }
        }

        public void StoreToDb(object sender)
        {
            try
            {
                if (_db != null)
                {
                    _db.Store(sender);
                    _db.Commit();
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
