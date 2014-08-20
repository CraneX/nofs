using System;
using Nofs.Net.Common.Interfaces.Library;
using System.Reflection;

namespace Nofs.Net.nofs.metadata.interfaces
{
    public interface IAttributeAccessor
    {
        Type GetInnerCollectionType(MethodInfo method);
        Type GetInnerCollectionType(object sender);
        bool IsExecutable(MethodInfo method);
        bool isFSRoot(MethodInfo method);
        bool IsFSRoot(object sender);
        bool IsFolderObject(MethodInfo method);
        bool IsDomainObject(MethodInfo method);
        bool IsFolderObject(PropertyInfo property);
        bool IsDomainObject(PropertyInfo property);
        bool IsFolderObject(object obj);
        bool IsDomainObject(object obj);
        bool IsFolderObject(Type type, bool strict);
        bool IsFolderObject(object obj, bool strict);
        Type FindObjectTypeToCreateForMknod(object obj);
        Type FindObjectTypeToCreateForMkDir(object obj);
        Type FindObjectTypeToCreateForMknod(object obj, MethodInfo method);
        Type FindObjectTypeToCreateForMkDir(object obj, MethodInfo method);
        IFolderObjectProperties GetFolderObject(object obj);
        IFolderObjectProperties GetFolderObject(MethodInfo method);
        IDomainObjectProperties GetDomainObject(object obj);
        IDomainObjectProperties GetDomainObject(MethodInfo method);
        void SetContainerIfAttributeExists(object obj, IDomainObjectContainer container);
        void SetContainerManagerIfAttributeExists(object obj, IDomainObjectContainerManager manager);
        string GetNameFromObject(object obj);
        bool CanSetNameForObject(object obj);
        void SetNameForObject(object obj, string name);

        bool HasAnyCustomMetadataManagement(object obj);
        IGetterSetterPair GetLastAccessTimePair(object obj);
        IGetterSetterPair GetLastModifiedTimePair(object obj);
        IGetterSetterPair GetCreateTimePair(object obj);
        IGetterSetterPair GetModePair(object obj);
        IGetterSetterPair GetUIDPair(object obj);
        IGetterSetterPair GetGIDPair(object obj);

        bool IsMethodCanWrite(MethodInfo method);
    }
}
