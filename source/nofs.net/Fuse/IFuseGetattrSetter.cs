
namespace Nofs.Net.Fuse
{
    public interface IFuseGetattrSetter
    {
        void set(long inode, int mode, int nlink, int uid, int gid, int rdev, long size, long blocks, int atime, int mtime, int ctime);
    }
}
