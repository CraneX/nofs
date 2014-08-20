using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nofs.Net.Utils
{
    internal sealed class PathUtil
    {
        private static string _separatorChar = "" + Path.PathSeparator;

        private PathUtil()
        {
        }

        public static List<string> SplitPath(string path)
        {
            List<string> segments = new List<string>();
            StringTokenizer parts = new StringTokenizer(path, _separatorChar);
            while (parts.hasMoreTokens())
            {
                segments.Add(parts.nextToken());
            }
            return segments;
        }

        public static string PathAfter(string path, int level)
        {
            StringBuilder newPath = new StringBuilder();
            List<string> segments = SplitPath(path);
            for (int i = level + 1; i < segments.Count; i++)
            {
                newPath.Append(_separatorChar);
                newPath.Append(segments[i]);
            }
            return newPath.ToString();
        }

        public static string GetParentName(string path)
        {
            //string[] parts = path.split("\\/");
            string[] parts = path.Split(("\\" + _separatorChar).ToCharArray());
            string parent = "";
            if (parts.Length == 2)
            {
                parent = _separatorChar;
            }
            else
            {
                for (int i = 1; i < parts.Length - 1; i++)
                {
                    parent += _separatorChar + parts[i];
                }
            }
            return parent;
        }

        public static string GetChildName(string path)
        {
            string[] parts = path.Split(("\\" + _separatorChar).ToCharArray());
            return parts.Length == 0 ? _separatorChar : parts[parts.Length - 1];
        }
    }

}
