using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.nofs.Db4o;
using Nofs.Net.Common.Interfaces.Domain;
using Db4objects.Db4o;
using Nofs.Net.nofs.metadata.interfaces;
using Db4objects.Db4o.Config;
using Nofs.Net.Fuse.Impl;

namespace nofs.fuse.test
{
    class TestFixture :IDisposable
    {
        public const string DB_PATH = "myTestdb.db";

        private IStatMapper statMapper = null;
        private IKeyCache keyCache = null;
        private IFileObjectFactory fileObjectFactory = null;
        private DomainObjectContainerManager manager = null;
        private IAttributeAccessor accessor = null;
        private IObjectContainer db;

        public TestFixture()
        {
            System.IO.File.Delete(DB_PATH);
        }

        public IDomainObjectContainerManager Manager
        {
            get
            {
                if (manager == null)
                {
                    db = Db4oEmbedded.OpenFile(ConfigureDb4oForReplication(), DB_PATH);
                    manager = new DomainObjectContainerManager(
                          db,
                          statMapper,
                          keyCache,
                          fileObjectFactory,
                          Accessor
                          );
                }
                return manager;
            }
        }


        private IAttributeAccessor Accessor
        {
            get
            {
                if (accessor == null)
                {
                    accessor = new AttributeAccessor(); 
                }
                return accessor;
            }
        }
        

        protected IEmbeddedConfiguration ConfigureDb4oForReplication()
        {
            IEmbeddedConfiguration configuration = Db4oEmbedded.NewConfiguration();
            configuration.File.GenerateUUIDs = ConfigScope.Globally;
            configuration.File.GenerateCommitTimestamps = true;

            return configuration;
        }

        public IDomainObjectContainer GetContainer(Type type)
        {
              return new DomainObjectContainer(
                    statMapper,
                    keyCache,
                    fileObjectFactory,
                    Manager,
                    accessor,
                    db,
                    type
                );
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Close();
                db.Dispose();
            }
        }
             
    }
}
