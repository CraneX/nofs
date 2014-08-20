using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Common.Interfaces.Library;
using System.Reflection;
using Nofs.Net.AnnotationDriver;

namespace Nofs.Net.Fuse.Impl
{
    public class AttributeAccessor : IAttributeAccessor
    {
        public Type GetInnerCollectionType(MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public Type GetInnerCollectionType(object sender)
        {
            throw new NotImplementedException();
        }

        public bool IsExecutable(MethodInfo method)
        {
            foreach (object o in method.GetCustomAttributes(true))
            {
                if (IsExecutableAttribute(o as Attribute))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsExecutableAttribute(Attribute attr)
        {
            return attr == null ? false : attr.GetType() == typeof(ExecutableAttribute);
        }

        public bool isFSRoot(MethodInfo method)
        {
            throw new NotImplementedException();
        }
        public bool IsFSRoot(object sender)
        {
            throw new NotImplementedException();
        }

        public bool IsFolderObject(MethodInfo method)
        {
            foreach (var item in method.GetCustomAttributes(true))
            {
                if (IsFolderObjectAttribute(item as Attribute))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDomainObject(MethodInfo method)
        {
            foreach (var item in method.GetCustomAttributes(true))
            {
                if (IsDomainObjectAttribute(item as Attribute))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsFolderObjectAttribute(Attribute attr)
        {
            return attr == null ? false : attr.GetType() == typeof(FolderObjectAttribute);
        }

        public bool IsFolderObject(PropertyInfo property)
        {
            foreach (var item in property.GetCustomAttributes(true))
            {
                if (IsFolderObjectAttribute(item as Attribute))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDomainObject(PropertyInfo property)
        {
            foreach (var item in property.GetCustomAttributes(true))
            {
                if (IsDomainObjectAttribute(item as Attribute))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsDomainObjectAttribute(Attribute attr)
        {
            return attr == null ? false : attr.GetType() == typeof(DomainObjectAttribute);
        }

        public bool IsFolderObject(object obj)
        {
            throw new NotImplementedException();
        }

        public bool IsFolderObject(Type type, bool strict)
        {
            throw new NotImplementedException();
        }

        public bool IsFolderObject(object obj, bool strict)
        {
            throw new NotImplementedException();
        }

        public bool IsDomainObject(object obj)
        {
            throw new NotImplementedException();
        }

        public Type FindObjectTypeToCreateForMknod(object obj)
        {
            throw new NotImplementedException();
        }

        public Type FindObjectTypeToCreateForMkDir(object obj)
        {
            throw new NotImplementedException();
        }

        public Type FindObjectTypeToCreateForMknod(object obj, MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public Type FindObjectTypeToCreateForMkDir(object obj, MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public IFolderObjectProperties GetFolderObject(object obj)
        {
            throw new NotImplementedException();
        }

        public IFolderObjectProperties GetFolderObject(MethodInfo method)
        {
            throw new NotImplementedException();
        }


        public IDomainObjectProperties GetDomainObject(object obj)
        {
            throw new NotImplementedException();
        }

        public IDomainObjectProperties GetDomainObject(MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public void SetContainerIfAttributeExists(object obj, IDomainObjectContainer container)
        {
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                foreach (object o in info.GetCustomAttributes(true))
                {
                    if (IsNeedsContainerAttribute(o as Attribute))
                    {
                        MethodInfo method = info.GetSetMethod();
                        if (method != null)
                        {
                            method.Invoke(obj, new object[] { container });
                            return;
                        }
                    }
                }
            }

            //search from method
            foreach (MethodInfo info in obj.GetType().GetMethods())
            {
                if (info.ReturnType == typeof(void))
                {
                    foreach (object o in info.GetCustomAttributes(true))
                    {
                        if (IsNeedsContainerAttribute(o as Attribute))
                        {
                            info.Invoke(obj, new object[] { container });
                            return;
                        }
                    }
                }
            }
        }

        private static bool IsNeedsContainerAttribute(Attribute attr)
        {
            if (attr != null)
            {
                return (attr.GetType() == typeof(NeedsContainerAttribute));
            }
            return false;
        }

        private static bool IsNeedsContainerManagerAttribute(Attribute attr)
        {
            if (attr != null)
            {
                return (attr.GetType() == typeof(NeedsContainerManagerAttribute));
            }
            return false;
        }

        public void SetContainerManagerIfAttributeExists(object obj, IDomainObjectContainerManager manager)
        {
            //search from properties first
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                foreach (var o in info.GetCustomAttributes(true))
                {
                    if (IsNeedsContainerManagerAttribute(o as Attribute))
                    {
                        MethodInfo method = info.GetSetMethod();
                        if (method != null)
                        {
                            method.Invoke(obj, new object[] { manager });
                            return;
                        }
                    }
                }
            }

            //search from method
            foreach (MethodInfo info in obj.GetType().GetMethods())
            {
                if (info.ReturnType == typeof(void))
                {
                    foreach (object o in info.GetCustomAttributes(true))
                    {
                        if (IsNeedsContainerManagerAttribute(o as Attribute))
                        {
                            info.Invoke(obj, new object[] { manager });
                            return;
                        }
                    }
                }
            }
        }

        public string GetNameFromObject(object obj)
        {
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                foreach (var item in info.GetCustomAttributes(true))
                {
                    if (IsProvidesNameAttribute(item as Attribute))
                    {
                        MethodInfo method = info.GetGetMethod();
                        object o = method.Invoke(obj, null);
                        return o == null ? obj.GetHashCode().ToString() : o.ToString();
                    }
                }

            }
            return string.Empty;
        }

        private bool IsProvidesNameAttribute(Attribute attr)
        {
            return attr == null ? false : attr.GetType() == typeof(ProvidesNameAttribute);
        }

        public bool CanSetNameForObject(object obj)
        {
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                foreach (var item in info.GetCustomAttributes(true))
                {
                    if (IsProvidesNameAttribute(item as Attribute))
                    {
                        return info.GetSetMethod() != null;
                    }
                }

            }
            return false;
        }

        public void SetNameForObject(object obj, string name)
        {
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                foreach (var item in info.GetCustomAttributes(true))
                {
                    if (IsProvidesNameAttribute(item as Attribute))
                    {
                        MethodInfo method = info.GetSetMethod();
                        method.Invoke(obj, new object[] { name });
                    }
                }

            }
        }

        public bool HasAnyCustomMetadataManagement(object obj)
        {
            throw new NotImplementedException();
        }

        public IGetterSetterPair GetLastAccessTimePair(object obj)
        {
            throw new NotImplementedException();
        }

        public IGetterSetterPair GetLastModifiedTimePair(object obj)
        {
            throw new NotImplementedException();
        }

        public IGetterSetterPair GetCreateTimePair(object obj)
        {
            throw new NotImplementedException();
        }

        public IGetterSetterPair GetModePair(object obj)
        {
            throw new NotImplementedException();
        }
        public IGetterSetterPair GetUIDPair(object obj)
        {
            throw new NotImplementedException();
        }

        public IGetterSetterPair GetGIDPair(object obj)
        {
            throw new NotImplementedException();
        }

        public bool IsMethodCanWrite(MethodInfo method)
        {
            foreach (object o in method.GetCustomAttributes(true))
            {
                if (o is FolderObjectAttribute)
                {
                    FolderObjectAttribute attr = o as FolderObjectAttribute;
                    return attr.CanAdd() || attr.CanRemove();
                }

                return true;

            }
            return false;
        }
    }

}
