using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Exceptions;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Utils;

namespace Nofs.Net.Fuse.Impl
{
    /// <summary>
    /// PathTranslator
    /// </summary>
    public class PathTranslator
    {
        private INoFSClassLoader _loader;
        private IDomainObjectContainerManager _manager;
        private IFileObjectFactory _fileObjectFactory;
        private object _root;
        private static string _separatorChar = "" + System.IO.Path.DirectorySeparatorChar;

        public PathTranslator(INoFSClassLoader loader, IDomainObjectContainerManager manager, IFileObjectFactory factory)
        {
            _root = null;
            _loader = loader;
            _manager = manager;
            _fileObjectFactory = factory;
        }

        
        public void CreateRootInFileSystem(LogManager logger) 
        {
            IEnumerable<Type> rootClasses = _loader.LoadClassesWithAnnotation(MarkerTypes.RootFolderObject);
            logger.LogInfo("Found " + rootClasses.ToString() + " root classes");
            if (rootClasses.Count() == 0)
            {
                throw new System.Exception("could not find any root classes. cannot create file system.");
            }
            Type firstClass = rootClasses.FirstOrDefault<Type>();
            IDomainObjectContainer container =_manager.GetContainer(firstClass);
            object sender = container.NewPersistentInstance();
            container.CreateAndSaveStatObjects(sender);
            logger.LogInfo("Created file system root.");
        }

        
        private object GetRootObject() 
        {
            if (_root == null)
            {
                IEnumerable<Type> rootClasses = _loader.LoadClassesWithAnnotation(MarkerTypes.RootFolderObject);
                IList<object> rootObjects = new List<object>();
                foreach (Type rootClass in rootClasses)
                {
                    IDomainObjectContainer container = (IDomainObjectContainer)_manager.GetContainer(rootClass);
                    rootObjects.Add(container.GetAllInstances());
                }
                if (rootObjects.Count != 1)
                {
                    throw new System.Exception("found " + rootObjects.Count() + " root file system objects");
                }
                _root = rootObjects.FirstOrDefault();
            }
            if (_root == null)
            {
                throw new System.Exception("null root!");
            }
            return _root;
        }

        public bool FileSystemHasARoot()
        {
            try
            {
                GetRootObject();
            }
            catch (System.Exception)
            {
            }
            return _root != null;
        }

        private static List<string> SplitPath(string path)
        {
            List<string> parts = new List<string>();
            string pattern = _separatorChar.CompareTo("\\") == 0 ? "\\\\" : _separatorChar;
            foreach (string part in path.Split(pattern.ToCharArray()))
            {
                if (part.Length > 0)
                {
                    parts.Add(part);
                }
            }
            return parts;
        }

        public IFileObject TranslatePathParent(string path) 
        {
            string parentPath = PathUtil.GetParentName(path);
            return TranslatePath(parentPath);
        }

        public IFileObject TranslatePath(string path) 
        {
            object current = GetRootObject();
            object methodObj = null;
            
            foreach (string name in SplitPath(path))
            {
                object newObj;
                if (methodObj == null)
                {
                    newObj = _fileObjectFactory.GetChildWithName(current, name);
                }
                else
                {
                    newObj = _fileObjectFactory.GetChildWithName(methodObj, (MethodInfo)current, name);
                }
                if (newObj is MethodInfo)
                {
                    methodObj = current;
                }
                else
                {
                    methodObj = null;
                }
                current = newObj;
                if (current == null)
                {
                    break;
                }
            }
            if (current == null)
            {
                return null;
            }
            else
            {
                if (methodObj == null)
                {
                    return _fileObjectFactory.BuildFileObject(PathUtil.GetParentName(path), current);
                }
                else
                {
                    return _fileObjectFactory.BuildFileObject(PathUtil.GetParentName(path), methodObj, (MethodInfo)current);
                }
            }
        }

        private static void ThrowIfDuplicateNameFound(string path, List<string> names)
        {
            List<string> seen = new List<string>();
            foreach (string name1 in names)
            {
                foreach (string name2 in seen)
                {
                    if (name2.CompareTo(name1) == 0)
                    {
                        throw new NoFSDuplicateNameException(path, name1);
                    }
                }
                seen.Add(name1);
            }
        }

        public IEnumerable<IFileObject> GetPathChildren(string path) 
        {
            object current = GetRootObject();
            object methodObj = null;
            foreach (string name in SplitPath(path))
            {
                object newObj;
                if (methodObj == null)
                {
                    newObj = _fileObjectFactory.GetChildWithName(current, name);
                }
                else
                {
                    newObj = _fileObjectFactory.GetChildWithName(methodObj, (MethodInfo)current, name);
                }
                if (newObj is MethodInfo)
                {
                    methodObj = current;
                }
                else
                {
                    methodObj = null;
                }
                current = newObj;
                if (current == null)
                {
                    throw new NoFSPathInvalidException(path);
                }
            }
            if (path.CompareTo(_separatorChar) != 0)
            {
                IFileObject file;
                if (methodObj != null)
                {
                    file = _fileObjectFactory.BuildFileObject(PathUtil.GetParentName(path), methodObj, (MethodInfo)current);
                }
                else
                {
                    file = _fileObjectFactory.BuildFileObject(PathUtil.GetParentName(path), current);
                }
                if (file.GetGenerationType() != GenerationType.DOMAIN_FOLDER)
                {
                    throw new NoFSPathIsNotAFolderException(path);
                }
            }

            List<IFileObject> objects = new List<IFileObject>();
            List<string> childNames = new List<string>();
            if (methodObj == null)
            {
                childNames.AddRange(_fileObjectFactory.GetChildNames(current));
            }
            else
            {
                childNames.AddRange(_fileObjectFactory.GetChildNames(methodObj, (MethodInfo)current));
            }
            ThrowIfDuplicateNameFound(path, childNames);
            foreach (string name in childNames)
            {
                object childObject;
                if (methodObj == null)
                {
                    childObject = _fileObjectFactory.GetChildWithName(current, name);
                }
                else
                {
                    childObject = _fileObjectFactory.GetChildWithName(methodObj, (MethodInfo)current, name);
                }
                if (childObject == null)
                {
                    throw new System.Exception("child with name '" + name + "' could not be found");
                }
                IFileObject childFile;
                if (childObject is MethodInfo)
                {
                    childFile = _fileObjectFactory.BuildFileObject(PathUtil.GetParentName(path), current, (MethodInfo)childObject);
                }
                else
                {
                    childFile = _fileObjectFactory.BuildFileObject(PathUtil.GetParentName(path), childObject);
                }
                objects.Add(childFile);
            }
            return objects;
        }
    }

}
