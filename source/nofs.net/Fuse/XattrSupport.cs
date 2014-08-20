using System;

namespace Nofs.Net.Fuse
{
    public interface IXattrSupport
    {
        // bits passed in 'flags' parameter to setxattr() method

        /**
         * This method can be called to query for the size of the extended attribute
         *
         * @param path the path to file or directory containing extended attribute
         * @param name the name of the extended attribute
         * @param sizeSetter a callback interface that should be used to set the attribute's size
         * @return 0 if Ok or errno when error
         * @throws FuseException an alternative to returning errno is to throw this exception with errno initialized
         */
        int getxattrsize(string path, string name, IFuseSizeSetter sizeSetter);

        /**
         * This method will be called to get the value of the extended attribute
         *
         * @param path the path to file or directory containing extended attribute
         * @param name the name of the extended attribute
         * @param dst a byte[] that should be filled with the value of the extended attribute
         * @return 0 if Ok or errno when error
         * @throws FuseException an alternative to returning errno is to throw this exception with errno initialized
         * @throws BufferOverflowException should be thrown to indicate that the given <code>dst</code> byte[]
         *         is not large enough to hold the attribute's value. After that <code>getxattr()</code> method will
         *         be called again with a larger buffer.
         */
        int getxattr(string path, string name, byte[] dst);

        /**
         * This method will be called to get the list of extended attribute names
         *
         * @param path the path to file or directory containing extended attributes
         * @param lister a callback interface that should be used to list the attribute names
         * @return 0 if Ok or errno when error
         * @throws FuseException an alternative to returning errno is to throw this exception with errno initialized
         */
        int listxattr(string path, IXattrLister lister);

        /**
         * This method will be called to set the value of an extended attribute
         *
         * @param path the path to file or directory containing extended attributes
         * @param name the name of the extended attribute
         * @param value the value of the extended attribute
         * @param flags parameter can be used to refine the semantics of the operation.<p>
         *        <code>XATTR_CREATE</code> specifies a pure create, which should fail with <code>Errno.EEXIST</code> if the named attribute exists already.<p>
         *        <code>XATTR_REPLACE</code> specifies a pure replace operation, which should fail with <code>Errno.ENOATTR</code> if the named attribute does not already exist.<p>
         *        By default (no flags), the  extended  attribute  will  be created if need be, or will simply replace the value if the attribute exists.
         * @return 0 if Ok or errno when error
         * @throws FuseException an alternative to returning errno is to throw this exception with errno initialized
         */
        int setxattr(string path, string name, byte[] value, int flags);

        /**
         * This method will be called to remove the extended attribute
         *
         * @param path the path to file or directory containing extended attributes
         * @param name the name of the extended attribute
         * @return 0 if Ok or errno when error
         * @throws FuseException an alternative to returning errno is to throw this exception with errno initialized
         */
        int removexattr(string path, string name);
    }
}
