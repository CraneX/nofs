using System;

namespace Nofs.Net.Common.Interfaces.Library
{

    /**
     * Classes that implement this interface provide their own structured data to the file system.
     * Classes that do not implement this interface are managed by NOFS automatically.
     */
    public interface IProvidesUnstructuredData
    {
        /**
         * Provides the size of the file on the file system.
         * @return	The size of the file in bytes.
         */
        long DataSize();

        /**
         * Tells NOFS if it can read ahead, write behind, and if data from reads can be cached in memory.
         * If your object can change its state without calling ObjectChanged() on a container, then this method
         * should always return false.
         * 
         * @return true if NOFS can cache data, false if not
         */
        bool Cacheable();

        /**
         * Provides read access to an object
         * 
         * @param buffer	The byte buffer to fill. The available size of the buffer may be less or more than the length parameter.
         * @param offset	The offset into the object's data that needs to be read
         * @param length	The number of bytes that have been requested. Fewer than length can be read, but not more.
         * @throws Exception 
         */
        void Read(byte[] buffer, long offset, long length);

        /**
         * Provides write access to an object
         * 
         * @param buffer	The byte buffer to write from. The available size of the buffer may be less or more than the length parameter.
         * @param offset	The offset into the object's data that needs to be written
         * @param length	The length of data that must be written.
         * @throws Exception 
         */
        void Write(byte[] buffer, long offset, long length);

        /**
         * Provides access to change the data size of an object (increase or reduce). It is okay to do nothing in this method 
         * at the cost of compatibility with some applications. 
         * 
         * @param length	The new size of the object's data
         */
        void Truncate(long length);
    }

}
