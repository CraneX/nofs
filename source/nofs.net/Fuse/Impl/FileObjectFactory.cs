using System;
using System.Collections.Generic;
using System.Reflection;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Domain.Impl;
using System.Collections;

namespace Nofs.Net.Fuse.Impl
{

    public class FileObjectFactory : IFileObjectFactory
    {
        private IAttributeAccessor _accessor;
        private IStatMapper _statMapper;
        private IKeyCache _keyCache;

        public FileObjectFactory(IAttributeAccessor accessor, IStatMapper statMapper, IKeyCache keyCache)
        {
            _accessor = accessor;
            _statMapper = statMapper;
            _keyCache = keyCache;
        }


        public IFileObjectStat BuildStat(object obj)
        {
            if (obj is MethodInfo)
            {
                throw new System.Exception("can't build from method");
            }
            if (!_accessor.IsDomainObject(obj) && !_accessor.IsFolderObject(obj))
            {
                throw new System.Exception(obj.GetType().Name + " is not a domain object");
            }
            int typ;
            if (_accessor.IsFolderObject(obj))
            {
                typ = FuseFtypeConstants.TYPE_DIR;
            }
            else
            {
                typ = FuseFtypeConstants.TYPE_FILE;
            }
            FileObjectStat stat = new FileObjectStat(_keyCache.GetByReference(obj).Id, _accessor.GetNameFromObject(obj));
            stat.Mode = (typ | 0755);
            return stat;
        }


        public IFileObjectStat BuildStat(object obj, MethodInfo method)
        {
            if (!_accessor.IsDomainObject(method) && !_accessor.IsFolderObject(method) &&
               !_accessor.IsExecutable(method))
            {
                throw new Exception("not a domain object");
            }
            int typ;
            if (_accessor.IsExecutable(method))
            {
                typ = FuseFtypeConstants.TYPE_FILE;
            }
            else
            {
                typ = FuseFtypeConstants.TYPE_DIR;
            }
            FileObjectStat stat = new FileObjectStat(_keyCache.GetByReference(obj).Id, TranslateMethodName(method.Name));
            stat.Mode = (typ | 0755);
            return stat;
        }


        public IFileObject BuildFileObject(string parentPath, object obj)
        {
            if (obj is MethodInfo)
            {
                throw new Exception("can't build from method");
            }
            if (!_accessor.IsDomainObject(obj) && !_accessor.IsFolderObject(obj))
            {
                throw new Exception("not a domain object");
            }
            GenerationType generation;
            if (_accessor.IsFolderObject(obj))
            {
                generation = GenerationType.DOMAIN_FOLDER;
            }
            else
            {
                generation = GenerationType.DATA_FILE;
            }
            FileObject file = new FileObject(parentPath, obj, _accessor, generation);
            return SetToKeyCache(obj, file);
        }


        public IFileObject BuildFileObject(string parentPath, object obj, MethodInfo method)// throws Exception 
        {
            if (!_accessor.IsDomainObject(method) && !_accessor.IsFolderObject(method) &&
               !_accessor.IsExecutable(method))
            {
                throw new Exception("not a domain object");
            }
            GenerationType generation;
            if (_accessor.IsExecutable(method))
            {
                generation = GenerationType.EXECUTABLE;
            }
            else
            {
                generation = GenerationType.DOMAIN_FOLDER;
            }
            FileObject file = new FileObject(parentPath, obj, method, _accessor, generation);
            return SetToKeyCache(obj, file);
        }

        private FileObject SetToKeyCache(object obj, FileObject file)
        {
            file.Id = _keyCache.GetByReference(obj).Id;
            file.SetStatLazyLoader(new StatLazyLoader(_statMapper, file));
            return file;
        }

        private static string TranslateMethodName(string name)
        {
            if (name.StartsWith("get") && name.Length > 3)
            {
                name = name.Substring(3);
            }
            return name;
        }


        public IEnumerable<string> GetChildNames(object methodObj, MethodInfo parentMethod) //throws IllegalArgumentException, IllegalAccessException, InvocationTargetException, Exception 
        {
            if (_accessor.IsFolderObject(parentMethod))
            {
                return GetChildNames(parentMethod.Invoke(methodObj, (object[])null));
            }
            else if (parentMethod.ReturnType.IsAssignableFrom(typeof(IList)))
            {
                List<string> names = new List<string>();
                List<object> objects = (List<object>)parentMethod.Invoke(methodObj, (object[])null);
                foreach (object obj in objects)
                {
                    names.Add(_accessor.GetNameFromObject(obj));
                }
                return names;
            }
            return new List<string>();
        }

        public IEnumerable<string> GetChildNames(object parent)
        {
            List<string> names = new List<string>();
            if (parent is IList)
            {
                foreach (object childObj in (IList)parent)
                {
                    if (_accessor.IsDomainObject(childObj) ||
                       _accessor.IsFolderObject(childObj))
                    {
                        names.Add(_accessor.GetNameFromObject(childObj));
                    }
                }
            }
            else
            {
                foreach (MethodInfo method in parent.GetType().GetMethods())
                {
                    string methodName = TranslateMethodName(method.Name);
                    if (_accessor.IsDomainObject(method) ||
                       _accessor.IsFolderObject(method) ||
                       _accessor.IsExecutable(method))
                    {
                        names.Add(methodName);
                    }
                }
            }
            return names;
        }

        public object GetChildWithName(object methodObj, MethodInfo parentMethod, string name)
        {
            object child = null;
            if (parentMethod.ReturnType.IsAssignableFrom(typeof(IList)))
            {
                IList objects = (IList)parentMethod.Invoke(methodObj, (object[])null);
                foreach (object obj in objects)
                {
                    if (_accessor.GetNameFromObject(obj).CompareTo(name) == 0)
                    {
                        child = obj;
                        break;
                    }
                }
            }
            return child;
        }


        public object GetChildWithName(object parent, string name)
        {
            object child = null;
            foreach (MethodInfo method in parent.GetType().GetMethods())
            {
                if (TranslateMethodName(method.Name).CompareTo(name) == 0)
                {
                    if (method.ReturnType == typeof(void)
                        || _accessor.IsDomainObject(method))
                    {
                        child = method;
                    }
                    else
                    {
                        child = method.Invoke(parent, (object[])null);
                    }
                    break;
                }
            }
            if (child == null && typeof(IList).IsAssignableFrom(parent.GetType()))
            {
                foreach (object obj in (IList)parent)
                {
                    if (_accessor.GetNameFromObject(obj).CompareTo(name) == 0)
                    {
                        child = obj;
                        break;
                    }
                }
            }
            return child;
        }
    }

}
