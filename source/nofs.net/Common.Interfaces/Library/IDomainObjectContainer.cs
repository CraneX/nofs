using System;
using System.Collections.Generic;
using Nofs.Net.Common.Interfaces.Cache;

namespace Nofs.Net.Common.Interfaces.Library
{

    /**
     * IDomainObjectContainer provides a persistence layer for NOFS objects. 
     *
     */
    public interface IDomainObjectContainer
    {
        /**
         * Queries if a object reference is persisted in the NOFS database
         * 
         * @param object 		A reference to an object that was returned by NewPersisentInstance or NewTransientInstance
         * @return				True if the object is persisted by NOFS
         * @throws Exception	Will fail if there are underlying failures in the NOFS database
         */
        bool IsPersistent<T>(T sender);

        /**
         * Changes and object's state from transient to persistent. Persistent objects can be saved in the NOFS database.
         * 
         * @param object        A reference to an object that is not persisted in the NOFS database
         * @throws Exception
         */
        void MakePersistent<T>(T sender);

        /**
         * Should be called on an object when a property that affects the object's name or file data is updated.
         * 
         * @param object        A reference to an object that has changed.
         * @throws Exception    An exception can be thrown if there is a fault in the NOFS database
         */
        void ObjectChanged<T>(T sender);

        /**
         * Should be called on an object when the name of the object on the file system is changed by a way other
         * than NOFS calling the setter method for a name (See the ProvidesName annotation)
         * 
         * @param object		The object that has changed its name
         * @param oldName		The name value before the change
         * @param newName		The name value after the change
         * @throws Exception	An exception can be thrown if the value for oldName is incorrect or if there is a fault in the NOFS database
         */
        void ObjectRenamed<T>(T sender, string oldName, string newName);

        /**
         * Should be called when an object is being removed from the file system. If the object is persisted
         * to the NOFS database, it will be removed.
         * 
         * @param object		The object to be removed.
         * @throws Exception    An exception can be thrown if there is a fault in the NOFS database
         */
        void Remove<T>(T sender);

        /**
         * Be warned, if there are many instances managed by NOFS, a call to GetAllInstances() can be expensive.
         * 
         * @return				Returns all instances managed by the current container.
         * @throws Exception	An exception can be thrown if there is a fault in the NOFS database
         */
        IEnumerable<T> GetAllInstances<T>();

        IEnumerable<object> GetAllInstances();
        /**
         * Query the NOFS database by example. Pass an object in with properties set that are similar to what
         * is being searched for. All non-default fields will be used to query the NOFS database.
         * 
         * @param example		The object to be used in the query
         * @return				a collection (size 0:N) of matching persisted objects
         * @throws Exception	An exception can be thrown if there is a fault in the NOFS database
         */
        IEnumerable<T> GetByExample<T>(object example);

        object GetByExample(object example);

        int GetObjectCountForTesting();

        /**
         * Return a weak reference to an object from the NOFS database. This is useful for file systems
         * with large object graphs or objects that contain large amounts of data.
         * 
         * @param id			The UUID for the object to be loaded. See the IObjectWithID interface.
         * @return				The weak reference for the object
         * @throws Exception	An exception can be thrown if there is a fault in the NOFS database
         */
        IWeakReference<T> GetWeakReference<T>(Guid id);

        /**
         * Creates a new instance of an object and persists it to the NOFS database. The class being created
         * must have a default constructor.
         * 
         * @return				A new instance of an object.
         * @throws Exception	An exception can be thrown if there is a fault in the NOFS database
         */
        object NewPersistentInstance();

        T NewPersistentInstance<T>();

        /**
         * Creates a new instance of an object using the default constructor.
         * 
         * @return				A new instance of an object.
         * @throws Exception	An exception can be thrown if there is a fault in the NOFS database
         */
        object NewTransientInstance();

        T NewTransientInstance<T>();

        void CreateAndSaveStatObjects<T>(T sender);

        void UpdateStatObjects<T>(T sender, string oldName, string newName);

        void SetFileCacheManager(IFileCacheManager cacheManager);

        void StoreToDb(object sender);

    }
}
