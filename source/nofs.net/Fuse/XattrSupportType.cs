
namespace Nofs.Net.Fuse
{
    public enum XattrSupportType
    {
        XATTR_CREATE = 0x1, /* set value, fail if attr already exists */

        XATTR_REPLACE = 0x2/* set value, fail if attr does not exist */
    }
}
