using System;
using System.Reflection;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Nofs.Net.Fuse.Impl
{
    public class DomainObjectCollectionHelper
    {
        private IDomainObjectContainerManager _containerManager;
        private IAttributeAccessor _accessor;

        public DomainObjectCollectionHelper(IDomainObjectContainerManager containerManager, IAttributeAccessor accessor)
        {
            _containerManager = containerManager;
            _accessor = accessor;
        }

        
        public void RemoveChildObject(object parent, MethodInfo method, object child) 
        {
            //incoming assumption is that this operation can succeed
            object result = method.Invoke(parent, (object[])null);
                
            RemoveChildObject(result, _accessor.GetInnerCollectionType(method), child);
        }

        
        public void RemoveChildObject(object obj, object child) 
        {
            //incoming assumption is that this operation can succeed
            RemoveChildObject(obj, _accessor.GetInnerCollectionType(obj), child);
        }

        
        private void RemoveChildObject(object objList, Type objType, object child) 
        {
            IDomainObjectContainer container = _containerManager.GetContainer(objType);
            if (objList is IList)
            {
                IList<object> list = objList as IList<object>;

                if (!list.Remove(child))
                {
                    throw new System.Exception("Child was not in the list!");
                }
                if (container.IsPersistent(child))
                {
                    container.Remove(child);
                }
            }
        }

        
        public void AddChildObject(object obj, MethodInfo method, string name, MarkerTypes inodeType) 
        {
            //incoming assumption is that this operation can succeed
            //AddChildObject((List)method.invoke(obj, (object[])null), _accessor.GetInnerCollectionType(method), name);
            AddChildObject(
                    (IList)method.Invoke(obj, (object[])null),
                    MarkerTypes.FolderObject == inodeType
                        ? _accessor.FindObjectTypeToCreateForMkDir(obj, method)
                        : _accessor.FindObjectTypeToCreateForMknod(obj, method),
                    name);
        }

        
        public void AddChildObject(object obj, string name, MarkerTypes inodeType)
        {
            //incoming assumption is that this operation can succeed
            //AddChildObject((List)obj, _accessor.GetInnerCollectionType(obj), name);
            AddChildObject(
                    (IList)obj,
                    MarkerTypes.FolderObject == inodeType
                        ? _accessor.FindObjectTypeToCreateForMkDir(obj)
                        : _accessor.FindObjectTypeToCreateForMknod(obj),
                    name);
        }

        
        private void AddChildObject(IList objList, Type objType, string name)
        {
            IDomainObjectContainer container = _containerManager.GetContainer(objType);
            object newObj = container.NewPersistentInstance();
            _accessor.SetNameForObject(newObj, name);
            container.CreateAndSaveStatObjects(newObj);
            objList.Add(newObj);
        }

        
        public void MoveChildObject(object objToMove, object sourceCollection, object destCollection, string newName) 
        {
            //incoming assumption is that this operation can succeed
            IList source = (IList)sourceCollection;
            IList dest = (IList)destCollection;
            if (!source.Contains(objToMove))
            {
                throw new System.Exception("Child was not in the list!");
            }
            else
            {
                source.Remove(objToMove);
            }
            dest.Add(objToMove);
            string oldName = _accessor.GetNameFromObject(objToMove);
            IDomainObjectContainer container = _containerManager.GetContainer(objToMove.GetType());
            container.UpdateStatObjects(objToMove, oldName, newName);
            _accessor.SetNameForObject(objToMove, newName);
        }
    }

}
