using System;
using System.Reflection;
using System.Collections.Generic;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Domain.Impl;
using Nofs.Net.Cache.Impl;

namespace Nofs.Net.Fuse.Impl
{
    public class NoFSFuseDriver : INoFSFuseDriver
    {
        private IStatHandler _stat;
        private IFsyncHandler _fsync;
        private IDirHandler _dir;
        private IFileHandler _file;
        private IFileDataHandler _data;
        private ILinkHandler _link;
        private IStatMapper _statMapper;
        private IExtendedAttributeHandler _xattr;

        private IDomainObjectContainerManager _containerManager;
        private PathTranslator _translator;
        private LogManager _logger;

        public IDomainObjectContainerManager GetDBForTesting()
        {
            return _containerManager;
        }

        public NoFSFuseDriver(
                string domainFile, 
                string objectStore,
                string metaLocation, 
                string metadataFactoryClassName,
                string persistenceFactoryClassName) 
            : this(domainFile, objectStore, metaLocation,
                    (IMetadataFactory)Activator.CreateInstance(Type.GetType(metadataFactoryClassName)),
                    (IPersistenceFactory)Activator.CreateInstance(Type.GetType(persistenceFactoryClassName)))
        {
           
        }

        public NoFSFuseDriver(
                string domainFile, 
                string objectStore,
                string metaLocation, 
                IMetadataFactory factory, 
                IPersistenceFactory persistenceFactory)
            : this(
                factory.CreateClassLoader(domainFile),
                objectStore, 
                metaLocation, 
                factory, 
            persistenceFactory)
        {
        }

        public NoFSFuseDriver(
                INoFSClassLoader classLoader, 
                string objectStore,
                string metaLocation, 
                IMetadataFactory factory, 
                IPersistenceFactory persistenceFactory)  
        {
            IAttributeAccessor attributeAccessor = factory.CreateAttributeAccessor();
            LockManager lockManager = new LockManager();
            LogManager logManager = new LogManager();

            IKeyCache keyCache = persistenceFactory.CreateKeyCache(objectStore, metaLocation, logManager);
            _statMapper = persistenceFactory.CreateStatMapper(objectStore, metaLocation, typeof(FileObjectStat).GetType(), logManager);
            IFileObjectFactory fileObjectFactory = new FileObjectFactory(attributeAccessor, _statMapper, keyCache);
           
            //
            IDomainObjectContainerManager containerManager = persistenceFactory.CreateContainerManager
                    (
                    objectStore, 
                    metaLocation, 
                    logManager, 
                    _statMapper, 
                    keyCache, 
                    fileObjectFactory, 
                    attributeAccessor);

            PathTranslator pathTranslator = new PathTranslator
                        (
                        classLoader,
                        containerManager, 
                        fileObjectFactory
                        );
            IFileCacheManager fileCacheManager = new FileCacheManager
                    (
                    containerManager  , 
                    logManager, 
                    factory.CreateMethodFilter()
                    );

            containerManager.SetFileCacheManager(fileCacheManager);
            DomainObjectCollectionHelper collectionHelper = new DomainObjectCollectionHelper(
                (IDomainObjectContainerManager)containerManager, 
                attributeAccessor);

            _stat = new StatHandler(fileCacheManager, pathTranslator, lockManager, _statMapper, logManager);
            _xattr = new ExtendedAttributeHandler(pathTranslator, lockManager, _statMapper, logManager);
            _fsync = new FsyncHandler();
            _dir = new DirHandler(pathTranslator, collectionHelper, lockManager, logManager);
            _file = new FileHandler(pathTranslator, collectionHelper, lockManager, logManager);
            _data = new FileDataHandler(fileCacheManager, pathTranslator, lockManager, logManager);
            _containerManager = containerManager;
            _translator = pathTranslator;
            _logger = logManager;
            _link = new LinkHandler();
        }

        public void DumpStatTables()  
        {
            _statMapper.DumpStatTables();
        }

        
        public IEnumerable<object> GetFSObjectsByType(string className)  
        {
            var container = _containerManager.GetContainer(Type.GetType(className));
            foreach (object obj in container.GetAllInstances())
            {
               yield return obj;
            }
        }

        public void Init()  
        {
            if (!_translator.FileSystemHasARoot())
            {
                _logger.LogInfo("No root found in file system. Attempting to create one...");
                _translator.CreateRootInFileSystem(_logger);
            }
            else
            {
                _logger.LogInfo("File system root object found.");
            }
        }

        public void CleanUp()  
        {
            if (_containerManager != null)
            {
                _containerManager.CleanUp();
            }
        }

        
        public int chmod(string path, int mode) 
        {
            return _stat.chmod(path, mode);
        }

        
        public int chown(string path, int uid, int gid)
        {
            return _stat.chown(path, uid, gid);
        }

        
        public int flush(string path, object fileHandle) 
        {
            return _data.flush(path, fileHandle);
        }

        
        public int fsync(string path, object fileHandle, bool isDataSync) 
        {
            return _fsync.fsync(path, fileHandle, isDataSync);
        }

        public int getattr(string path, IFuseGetattrSetter attr) 
        {
            return _stat.getattr(path, attr);
        }

        
        public int getdir(string path, IFuseDirFiller filler) 
        {
            return _dir.getdir(path, filler);
        }

        
        public int link(string fromPath, string toPath) 
        {
            return FuseErrno.ENOTSUPP;
        }

        
        public int mkdir(string path, int mode) 
        {
            return _dir.mkdir(path, mode);
        }

        
        public int mknod(string path, int mode, int rdev) 
        {
            return _file.mknod(path, mode, rdev);
        }

        
        public int open(string path, int flags, IFuseOpenSetter openSetter) 
        {
            return _data.open(path, flags, openSetter);
        }

        
        public int read(string path, object fh, byte[] buf, long offset) 
        {
            return _data.read(path, fh, buf, offset);
        }

        
        public int readlink(string path, char[] link)
        {
            return _link.readlink(path, link);
        }

        
        public int release(string path, object fh, int flags) 
        {
            return _data.release(path, fh, flags);
        }

        
        public int rename(string from, string to)
        {
            return _file.rename(from, to);
        }

        
        public int rmdir(string path) 
        {
            return _dir.rmdir(path);
        }

        
        public int statfs(IFuseStatfsSetter statfsSetter)
        {
            int stat = 0;
            try
            {
                statfsSetter.set(2048, 
                                1024 * 1024 * 1024, 
                                1024 * 1024 * 1024,
                                1024 * 1024 * 1024,
                                1000,
                                1000 + 1024 * 1024,
                                2048);
            }
            catch (System.Exception e)
            {
                throw new FuseException("statfs()", e);
            }
            return stat;
        }

        
        public int symlink(string linkTarget, string linkPath)
        {
            return _link.symlink(linkTarget, linkPath);
        }

        
        public int truncate(string path, long length) 
        {
            return _data.truncate(path, length);
        }

        
        public int unlink(string path)
        {
            return _file.unlink(path);
        }

        
        public int utime(string path, int atime, int mtime) 
        {
            return _stat.utime(path, atime, mtime);
        }

        
        public int write(string path, object fh, bool isWritePage, byte[] buf, long offset)
        {
            return _data.write(path, fh, isWritePage, buf, offset);
        }

        public int getxattr(string path, string name, byte[] dst)
        {
            return _xattr.getxattr(path, name, dst);
        }

        public int getxattrsize(string path, string name, IFuseSizeSetter sizeSetter) 
        {
            return _xattr.getxattrsize(path, name, sizeSetter);
        }

        public int listxattr(string path, IXattrLister lister)
        {
            return _xattr.listxattr(path, lister);
        }

        public int removexattr(string path, string name) 
        {
            return _xattr.removexattr(path, name);
        }

        public int setxattr(string path, string name, byte[] value, int flags) 
        {
            return _xattr.setxattr(path, name, value, flags);
        }

    }

}
