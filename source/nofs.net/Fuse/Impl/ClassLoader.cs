using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Nofs.Net.AnnotationDriver;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Domain.Impl;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Utils;

namespace Nofs.Net.Fuse.Impl
{
    public class ClassLoader : IFuseObjectTree, IDisposable
    {
        public const string RootFolder = @"/";
        private Assembly Loader;
        private object RootInstance;
        private Dictionary<string, MethodInfo> MethodNameMapping = new Dictionary<string, MethodInfo>();
        private Dictionary<string, List<string>> ExecutableFiles = new Dictionary<string, List<string>>();
        private Dictionary<string, List<FileObject>> DomainObjectMapping = new Dictionary<string, List<FileObject>>();
        private Dictionary<string, object> InstanceMapping = new Dictionary<string, object>();
        private List<string> FolderCache;
        private List<string> FilesCache;
        private IDomainObjectContainerManager DomainObjectContainerManager;
        private IAttributeAccessor Accessor;

        public ClassLoader(string fileName,
            IDomainObjectContainerManager domainObjectContainerManager,
            IAttributeAccessor accessor
            )
        {
            Accessor = accessor;
            DomainObjectContainerManager = domainObjectContainerManager;
            if (string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
            {
                fileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + fileName;
            }

            Loader = Assembly.LoadFile(fileName);
            RootClass = FindRootClass(Loader, DomainObjectContainerManager);
            if (RootClass == null)
            {
                throw new FuseException("Can not find root class!");
            }
            else
            {
                RootInstance = GetClassInstance(RootClass, RootFolder);
                if (Accessor != null)
                {
                    Accessor.SetContainerManagerIfAttributeExists(RootInstance, domainObjectContainerManager);
                    Accessor.SetContainerIfAttributeExists(RootInstance, domainObjectContainerManager.GetContainer(RootClass));
                }

                FolderCache = new List<string>(GetFolders(RootClass));
                FilesCache = new List<string>(GetFiles(RootClass, RootFolder));
            }
        }


        public void Dispose()
        {
            if (DomainObjectContainerManager != null)
            {
                DomainObjectContainerManager.CleanUp();
            }
        }

        #region interface

        public bool HasRootClass
        {
            get
            {
                return RootClass != null;
            }
        }

        public string TempFileNamePrefix
        {
            get
            {
                return @"..";
            }
        }

        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<string> GetFolders(string path)
        {
            if (path == RootFolder)
            {
                return FolderCache.Where<string>(item => item != RootFolder);
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public IEnumerable<string> GetFiles(string path)
        {
            List<string> names;
            if (path == RootFolder)
            {
                // return GetFiles(RootClass, path);
                names = FilesCache;
            }
            else
            {
                names = new List<string>();

                if (MethodNameMapping.ContainsKey(path))
                {
                    PathParser parse = new PathParser(path);

                    MethodInfo method = FindMethodInfo(parse.Folder, parse.FileName);
                    if (method != null)
                    {
                        List<FileObject> list = null;
                        if (!DomainObjectMapping.TryGetValue(path, out list))
                        {
                            list = new List<FileObject>();
                            DomainObjectMapping.Add(path, list);
                        }

                        if (IsCollectionType(method.ReturnType.FullName))
                        {
                            object v = method.Invoke(RootInstance, null);
                            foreach (var item in (System.Collections.IEnumerable)v)
                            {
                                string fileName = Accessor.GetNameFromObject(item);
                                names.Add(fileName);

                                if (list.Find(itr => itr.FileName == fileName) == null)
                                {
                                    list.Add(new FileObject(parse.Folder, fileName, item));
                                }
                            }
                        }
                    }
                }
                else if (DomainObjectMapping.ContainsKey(path))
                {
                    foreach (var item in DomainObjectMapping[path])
                    {
                        names.Add(item.FileName);
                    }
                }
            }

            return names.Where(item => true);
        }

        public bool IsFolder(string path)
        {
            return FolderCache.Contains(path);
        }

        public bool RemoveFolder(string path)
        {
            return FolderCache.Remove(path);
        }

        public bool IsFile(string path)
        {
            PathParser parse = new PathParser(path);
            return FindDomainObject(parse.Folder, parse.FileName) != null;
        }

        public bool RemoveFile(string path)
        {
            throw new NotImplementedException();
        }

        public bool IsExcuteFile(string path)
        {
            PathParser parse = new PathParser(path);
            return IsInCache(parse.Folder, parse.FileName, ExecutableFiles);
        }

        public void ExecuteFile(string path, params string[] paramsValue)
        {
            PathParser parse = new PathParser(path);
            ExecuteFile(parse.Folder, parse.FileName, paramsValue);
        }

        public byte[] GetFileData(string path)
        {
            PathParser parse = new PathParser(path);
            string data = GetFileContent(parse.Folder, parse.FileName);
            return new UTF8Encoding(true, true).GetBytes(data);
        }

        public int GetFileLength(string path)
        {
            PathParser parse = new PathParser(path);
            if (string.IsNullOrEmpty(parse.FileName))
            {
                return 0;
            }
            else
            {
                string data = GetFileContent(parse.Folder, parse.FileName);
                return string.IsNullOrEmpty(data) ? 0 : data.Length;
            }
        }

        #endregion

        public Type RootClass
        {
            get;
            private set;
        }

        private object GetClassInstance(Type classType, string path)
        {
            object instance = null;
            try
            {
                instance = Activator.CreateInstance(classType);
                if (!InstanceMapping.ContainsKey(path))
                {
                    InstanceMapping.Add(path, instance);
                }
                if (RootInstance == null && classType == RootClass)
                {
                    RootInstance = instance;
                }
            }
            catch
            {
                throw;
            }

            return instance;
        }

        private Type FindRootClass(Assembly loader, IDomainObjectContainerManager domainObjectContainerManager)
        {
            foreach (Type item in Loader.GetTypes())
            {
                if (!item.IsInterface)
                {
                    foreach (var attr in item.GetCustomAttributes(true))
                    {
                        if (attr.GetType() == typeof(RootFolderObjectAttribute))
                        {
                            return item;
                        }
                    }
                }
            }
            return null;
        }

        #region Get Folder

        private static bool IsCollectionType(string fullTypeName)
        {
            return fullTypeName.Contains("[[") && fullTypeName.Contains("]]");
        }

        private string ParseFunctionName(MethodInfo method)
        {
            string name = method.Name;
            //get_ is from Property get method
            if (name.StartsWith("get_", StringComparison.OrdinalIgnoreCase))
            {
                name = RootFolder + name.Substring(4);
            }
            else if (name.StartsWith("get", StringComparison.OrdinalIgnoreCase))
            {
                name = RootFolder + name.Substring(3);
            }
            else
            {
                name = RootFolder + name;
            }

            if (!MethodNameMapping.ContainsKey(name))
            {
                MethodNameMapping.Add(name, method);
            }

            return name;
        }

        private MethodInfo GetOriginMethodName(string name)
        {
            return MethodNameMapping[name];
        }

        public IEnumerable<string> GetFolders(Type rootClass)
        {
            if (FolderCache != null)
            {
                foreach (string s in FolderCache)
                {
                    yield return s;
                }
            }
            else if (rootClass != null)
            {
                yield return RootFolder;//

                foreach (PropertyInfo info in rootClass.GetProperties())
                {
                    if (Accessor.IsFolderObject(info))
                    {
                        yield return ParseFunctionName(info.GetGetMethod());
                    }
                }

                foreach (MethodInfo info in rootClass.GetMethods())
                {
                    if (Accessor.IsFolderObject(info))
                    {
                        yield return ParseFunctionName(info);
                    }
                }
            }
        }

        #endregion

        #region GetFiles

        private void AddToCache(string folder, string file, IDictionary<string, List<string>> map)
        {
            if (!map.ContainsKey(folder))
            {
                map.Add(folder, new List<string>());
            }

            if (!map[folder].Contains(file))
            {
                map[folder].Add(file);
            }
        }

        private void AddToCache(
            string folder,
            string fileName,
            object sender,
            IDictionary<string, List<FileObject>> map)
        {
            if (!map.ContainsKey(folder))
            {
                map.Add(folder, new List<FileObject>());
            }

            FileObject o = map[folder].Find(item => item.FileName == fileName);
            if (o == null)
            {
                map[folder].Add(new FileObject(folder, fileName, sender));
            }
        }

        private bool IsInCache(string folder, string file, IDictionary<string, List<string>> map)
        {
            return map.ContainsKey(folder) &&
                (map[folder].Contains(folder + file) || map[folder].Contains(file));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootClass"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public IEnumerable<string> GetFiles(Type rootClass, string folder)
        {
            if (rootClass != null)
            {
                if (string.IsNullOrEmpty(folder))
                {
                    folder = RootFolder;
                }

                foreach (PropertyInfo info in rootClass.GetProperties())
                {
                    if (Accessor.IsDomainObject(info))
                    {
                        MethodInfo method = info.GetGetMethod();
                        if (method != null)
                        {
                            object obj = method.Invoke(RootInstance, new object[] { });
                            AddToCache(folder, info.Name, obj, DomainObjectMapping);
                        }

                        yield return info.Name;
                    }
                }

                foreach (MethodInfo info in rootClass.GetMethods())
                {
                    if (Accessor.IsExecutable(info))
                    {
                        string name = ParseFunctionName(info);
                        AddToCache(folder, name, ExecutableFiles);
                        if (name.StartsWith("/"))
                        {
                            yield return name.Substring(1);
                        }
                        else
                        {
                            yield return name;
                        }
                    }
                }
            }
        }

        #endregion

        #region Get FileContents

        private MethodInfo FindMethodInfo(string folder, string fileName)
        {
            MethodInfo method;
            if ( MethodNameMapping.TryGetValue(fileName, out method)
                || MethodNameMapping.TryGetValue(folder + fileName, out method))
            {
                //DO NOTHING HERE
            }

            return method;
        }

        public string GetFileContent(string folder, string fileName)
        {
            if (IsInCache(folder, fileName, ExecutableFiles))
            {
                //is excute files
                MethodInfo method = FindMethodInfo(folder, fileName);
                if (method != null)
                {
                    return GenerateExcutableFileData(method);
                }
            }
            else
            {
                FileObject o = FindDomainObject(folder, fileName);
                if (o == null)
                {
                    throw new FuseException("Can not find this domain object!");
                }
                return StreamUtil.SerializeToString(o.Content);
            }

            return string.Empty;
        }

        private FileObject FindDomainObject(string folder, string fileName)
        {
            if (DomainObjectMapping.ContainsKey(folder))
            {
                return DomainObjectMapping[folder].Find(item => item.FileName == fileName);
            }
            else
            {
                return null;
            }
        }

        private string GenerateExcutableFileData(MethodInfo method)
        {
            StringBuilder sb = new StringBuilder();

            string excuteFileName = TempFileNamePrefix + method.Name;

            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                case PlatformID.Win32S:
                    {
                        sb.AppendLine("Const ForWriting = 2");
                        sb.AppendLine("Set objArgs = wscript.Arguments");
                        sb.AppendLine("Set objFSO = CreateObject(\"Scripting.FileSystemObject\")");
                        sb.AppendLine("s=\"\"");
                        sb.AppendFormat(
                            CultureInfo.InvariantCulture,
                            "Set objFile = objFSO.CreateTextFile(\"{0}\", ForWriting)\r\n",
                            excuteFileName
                            );
                        sb.AppendLine("for each arg in objArgs");
                        sb.AppendLine("\t s = s + arg + \" \" ");
                        sb.AppendLine("next");
                        sb.AppendLine("objFile.Write s");
                        sb.AppendLine("objFile.Close");
                        sb.AppendLine("Set objFile = Nothing");
                        sb.AppendLine("Set objFSO = Nothing");
                    }
                    break;
                default:
                    {
                        sb.Append("echo");
                        int index = 0;
                        foreach (ParameterInfo info in method.GetParameters())
                        {
                            if (!info.IsOut)
                            {
                                sb.Append(" $" + (++index).ToString());
                            }
                        }
                        sb.Append(" > " + excuteFileName);
                    }
                    break;        
            }


            return sb.ToString();

        }

        #endregion

        #region Execute File

        public object ExecuteFile(string folder, string file, params string[] args)
        {
            if (IsInCache(folder, file, ExecutableFiles))
            {
                object instance;
                if (InstanceMapping.TryGetValue(folder, out instance))
                {
                    //is excute files
                    MethodInfo method = FindMethodInfo(folder, file);
                    if (method != null)
                    {
                        try
                        {
                            object[] paramters = ConvertToArguments(method, args);
                            if (paramters != null)
                            {
                                return method.Invoke(instance, paramters);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
            else
            {
                throw new FuseException("Can not find this method");
            }
            return null;
        }

        private object[] ConvertToArguments(MethodInfo method, string[] args)
        {
            List<object> list = new List<object>();
            int i = 0;
            foreach (ParameterInfo info in method.GetParameters())
            {
                if (i >= args.Length)
                {
                    Console.Error.WriteLine("\nError: Missing paramter for " + info.Name);
                    return null;
                }

                string typeName = info.ParameterType.Name.ToUpperInvariant();

                switch (typeName)
                {
                    case "BOOL":
                    case "BOOLEAN":
                        list.Add(Boolean.Parse(args[i]));
                        break;
                    case "BYTE":
                        list.Add(Byte.Parse(args[i]));
                        break;
                    case "CHAR":
                        list.Add(Char.Parse(args[i]));
                        break;
                    case "DATETIME":
                        list.Add(DateTime.Parse(args[i]));
                        break;
                    case "DATETIMEOFFSET":
                        list.Add(DateTimeOffset.Parse(args[i]));
                        break;
                    case "DECIMAL":
                        list.Add(Decimal.Parse(args[i]));
                        break;
                    case "DOUBLE":
                        list.Add(Double.Parse(args[i]));
                        break;
                    case "FLOAT":
                        list.Add(float.Parse(args[i]));
                        break;
                    case "INT16":
                    case "SHORT":
                        list.Add(short.Parse(args[i]));
                        break;
                    case "INT32":
                    case "INT":
                        list.Add(int.Parse(args[i]));
                        break;
                    case "INT64":
                    case "LONG":
                        list.Add(long.Parse(args[i]));
                        break;
                    case "OBJECT":
                        list.Add(args[i] as object);
                        break;
                    case "SBYTE":
                        list.Add(SByte.Parse(args[i]));
                        break;
                    case "STRING":
                        list.Add(args[i]);
                        break;
                    case "TIMESPAN":
                        list.Add(TimeSpan.Parse(args[i]));
                        break;
                    case "UINT16":
                    case "USHORT":
                        list.Add(ushort.Parse(args[i]));
                        break;
                    case "UINT32":
                    case "UINT":
                        list.Add(uint.Parse(args[i]));
                        break;
                    case "UINT64":
                    case "ULONG":
                        list.Add(ulong.Parse(args[i]));
                        break;
                    case "GUID":
                        list.Add(new Guid(args[i]));
                        break;
                    default:
                        PathParser parse = new PathParser(args[i]);
                        FileObject obj = SearchDomainObject(parse.Folder, parse.FileName, info.ParameterType);
                        if (obj == null)
                        {
                            IDomainObjectContainer container = DomainObjectContainerManager.GetContainer(info.ParameterType);
                            //only can call NewTransientInstance
                            object instance = container.NewTransientInstance();
                            if (Accessor.CanSetNameForObject(instance))
                            {
                                Accessor.SetNameForObject(instance, parse.FileName);
                                list.Add(container.GetByExample(instance));
                            }
                        }
                        else
                        {
                            list.Add(obj == null ? null : obj.Content);
                        }
                        break;
                }
                //
                ++i;
            }
            return list.ToArray();
        }

        private FileObject SearchDomainObject(string folder, string fileName, Type type)
        {
            if (string.IsNullOrEmpty(folder))
            {
                foreach (KeyValuePair<string, List<FileObject>> pair in DomainObjectMapping)
                {
                    if (pair.Value != null)
                    {
                        FileObject o = pair.Value.Find(
                            item => (item.FileName == fileName) &&
                                (item.Content != null && item.Content.GetType() == type));

                        if (o != null)
                        {
                            return o;
                        }
                    }
                }
            }
            else
            {
                return FindDomainObject(folder, fileName);
            }
            return null;
        }

        #endregion

        public void ClearCache()
        {
            DomainObjectMapping.Clear();
        }

    }
}
