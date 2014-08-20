
namespace Nofs.Net.Fuse
{
    public interface IDirHandler
    {
        int getdir(string path, IFuseDirFiller filler);

	    int mkdir(string path, int mode);
            
	    int rmdir(string path);
    }
}
