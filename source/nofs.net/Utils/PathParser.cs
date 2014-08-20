using System.IO;

namespace Nofs.Net.Utils
{
    public class PathParser
    {
        public PathParser(string path)
        {
            Folder = Path.GetDirectoryName(path);
            FileName = Path.GetFileName(path);

            if (string.IsNullOrEmpty(Folder))
            {
                Folder = @"/";
            }

            Folder = Folder.Replace("\\", "/");
        }

        public string Folder
        {
            private set;
            get;
        }

        public string FileName
        {
            private set;
            get;
        }
    }
}
