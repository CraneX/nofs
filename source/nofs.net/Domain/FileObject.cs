using System;
using System.Reflection;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;
using System.Collections.Generic;

namespace Nofs.Net.Domain.Impl
{
    public class FileObject : BaseDomainObject, IFileObject
    {
        private IAttributeAccessor _accessor;
        private IFileObjectStat _statInternal;
        private IFileObjectStat _realStat;
        private GenerationType _generation;
        private MethodInfo _method;
        private StatLazyLoader _statLoader;

        public FileObject(
              string folder,
              string fileName,
              object sender)
            :base()
        {
            Folder = folder;
            FileName = fileName;
            Content = sender;
        }

        public FileObject(string folder, object sender, IAttributeAccessor accessor, GenerationType generation)
            : this(folder, sender, null, accessor, generation)
        {
        }

        public FileObject(string folder, object sender, MethodInfo method, IAttributeAccessor accessor, GenerationType generation)
        {
            Folder = folder;
            Content = sender;

            _accessor = accessor;
            _generation = generation;
            _method = method;
            _statLoader = null;
            _statInternal = null;
            _realStat = null;
        }

        public void SetStatLazyLoader(StatLazyLoader statLoader)
        {
            _statLoader = statLoader;
            _statInternal = null;
            _realStat = null;
        }

        private static bool RunMethod(object sender, string methodName) 
        {
            foreach (MethodInfo method in sender.GetType().GetMethods())
            {
                if (method.Name.CompareTo(methodName) == 0 &&
                   method.ReturnType == typeof(bool) &&
                   method.GetParameters().Length == 0)
                {
                    return (bool)method.Invoke(sender, (object[])null);
                }
            }
            throw new System.Exception("could not find method: " + methodName);
        }

        public string Folder
        {
            private set;
            get;
        }

        public string FileName
        {
            set;
            get;
        }

        public object Content
        {
            private set;
            get;
        }

        public bool CanMknod()  
        {
            return CanAdd() && (!InnerCollectionTypeIsFolderType(true) && InnerCollectionTypeIsFileType());
        }

        private bool CanAdd()  
        {
            if (_generation == GenerationType.DOMAIN_FOLDER)
            {
                IFolderObjectProperties fObj;
                bool addableObject = false;
                if (HasMethod())
                {
                    fObj = _accessor.GetFolderObject(GetMethod());
                    addableObject = typeof(List<object>).GetType().IsAssignableFrom(GetMethod().ReturnType);
                }
                else
                {
                    fObj = _accessor.GetFolderObject(Content);
                    addableObject = typeof(List<object>).GetType().IsAssignableFrom(Content.GetType());
                }
                if (addableObject && fObj.CanAdd())
                {
                    if (fObj.CanAddMethod().Length > 0)
                    {
                        return RunMethod(Content, fObj.CanAddMethod());
                    }
                    return true;
                }
            }
            return false;
        }

        private bool InnerCollectionTypeIsFileType() 
        {
            Type innerType;
            if (HasMethod())
            {
                innerType = _accessor.GetInnerCollectionType(GetMethod());
            }
            else
            {
                innerType = _accessor.GetInnerCollectionType(Content);
            }
            return _accessor.IsDomainObject(innerType);
        }

        private bool InnerCollectionTypeIsFolderType(bool strict)  
        {
            Type innerType;
            if (HasMethod())
            {
                innerType = _accessor.GetInnerCollectionType(GetMethod());
            }
            else
            {
                innerType = _accessor.GetInnerCollectionType(Content);
            }
            return _accessor.IsFolderObject(innerType, strict);
        }

        public bool CanMkdir()  
        {
            if (CanAdd() && GetGenerationType() == GenerationType.DOMAIN_FOLDER)
            {
                return InnerCollectionTypeIsFolderType(false);
            }
            return false;
        }

        private bool CanRemove()  
        {
            if (_generation == GenerationType.DOMAIN_FOLDER)
            {
                IFolderObjectProperties fObj;
                bool removableObject = false;
                if (HasMethod())
                {
                    fObj = _accessor.GetFolderObject(GetMethod());
                    removableObject = typeof(List<object>).GetType().IsAssignableFrom(GetMethod().ReturnType);
                }
                else
                {
                    fObj = _accessor.GetFolderObject(Content);
                    removableObject = typeof(List<object>).GetType().IsAssignableFrom(Content.GetType());
                }
                if (removableObject && fObj.CanRemove())
                {
                    if (fObj.CanRemoveMethod().Length > 0)
                    {
                        return RunMethod(Content, fObj.CanRemoveMethod());
                    }
                    return true;
                }
                return removableObject && fObj.CanRemove();
            }
            return false;
        }

        public bool CanWrite()  
        {
            if (_generation == GenerationType.DATA_FILE)
            {
                IDomainObjectProperties dObj;
                if (HasMethod())
                {
                    dObj = _accessor.GetDomainObject(GetMethod());
                }
                else
                {
                    dObj = _accessor.GetDomainObject(Content);
                }
                return dObj.CanWrite();
            }
            return true;
        }

        public bool CanDeleteChildren() 
        {
            return CanRemove() && !InnerCollectionTypeIsFolderType(false);
        }

        public bool CanRmdir()  
        {
            if (CanRemove() && GetGenerationType() == GenerationType.DOMAIN_FOLDER)
            {
                return InnerCollectionTypeIsFolderType(false);
            }
            return false;
        }

        public GenerationType GetGenerationType()
        {
            return _generation;
        }

        public string GetName() 
        {
            return HasMethod() ? _accessor.GetNameFromObject(GetMethod()) : _accessor.GetNameFromObject(Content);
        }

        public bool CanSetName()  
        {
            return _accessor.CanSetNameForObject(GetValue());
        }

        public void SetName(string value) 
        {
            _accessor.SetNameForObject(GetValue(), value);
        }

        public bool IsChildTypeCompatible(IFileObject possibleChild)  
        {
            Type myType = HasMethod()
                    ? _accessor.GetInnerCollectionType(GetMethod())
                    : _accessor.GetInnerCollectionType(Content);
            Type theirType = possibleChild.GetValue().GetType();
            return myType.IsAssignableFrom(theirType) && theirType.IsAssignableFrom(myType);
        }

        private IFileObjectStat GetStatInternal()  
        {
            if (_statLoader != null)
            {
                _statInternal = _statLoader.GetStat();
                _statLoader = null;
            }
            return _statInternal;
        }

        public IFileObjectStat GetStat()  
        {
            if (_realStat == null)
            {
                if (_accessor.HasAnyCustomMetadataManagement(Content))
                {
                    _realStat = new CustomStat(GetStatInternal(), _accessor, Content);
                }
                else
                {
                    _realStat = GetStatInternal();
                }
            }
            return _realStat;
        }


        public MethodInfo GetMethod()
        {
            return _method;
        }

        public bool HasMethod()
        {
            return _method != null;
        }

        public object GetValue() 
        {
            return HasMethod() ? GetMethod().Invoke(Content, (object[])null) : Content;
        }

        public bool SupportsDirectIO()  
        {
            return (GetGenerationType() == GenerationType.DATA_FILE)
                    && (GetValue() is IProvidesUnstructuredData);
        }

        public IProvidesUnstructuredData GetReadWriteInterface()  
        {
            return (IProvidesUnstructuredData)GetValue();
        }

       
    }

}
