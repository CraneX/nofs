using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Nofs.Net.Fuse.Impl
{
    internal class BaseFileSystem
    {
        private log4net.ILog log = LogManager.GetLogger(typeof(BaseFileSystem));
        private string TempFileNamePrefix;
        private IFuseObjectTree ObjectsTree;
        private Dictionary<string, List<byte>> MemoryBuffer;

        public BaseFileSystem(IFuseObjectTree tree)
        {
            ObjectsTree = tree;
            TempFileNamePrefix = tree.TempFileNamePrefix;
            MemoryBuffer = new Dictionary<string, List<byte>>();
        }

        public bool IsTempFile(string path)
        {
            return path.StartsWith("/" + TempFileNamePrefix, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsFolder(string path)
        {
            return !IsTempFile(path) && ObjectsTree.IsFolder(path);
        }

        public bool IsExcuteFile(string file)
        {
            return ObjectsTree.IsExcuteFile(file);
        }

        public bool IsValidFiles(string path)
        {
            return IsTempFile(path) || IsExcuteFile(path) || ObjectsTree.IsFile(path);
        }

        public int GetFileLength(string file)
        {
            if (IsTempFile(file))
            {
                return 0;
            }
            else
            {
                return ObjectsTree.GetFileLength(file);
            }
        }

        public byte[] GetFileData(string file)
        {
            return ObjectsTree.GetFileData(file);
        }

        public IEnumerable<string> GetFolders(string directory)
        {
            return ObjectsTree.GetFolders(directory);
        }

        public IEnumerable<string> GetFiles(string directory)
        {
            return ObjectsTree.GetFiles(directory);
        }

        public void ExecuteFile(string fileName, byte[] buffer)
        {
            try
            {
                string s = UTF8Encoding.UTF8.GetString(buffer).Trim();

                Trace.TraceInformation(s);
                //LogManager.LogInfo(log, "input arguments:" + s);

                ObjectsTree.ExecuteFile(fileName.Replace(TempFileNamePrefix, string.Empty), s.Split(' '));
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        #region Write File Buffer

        private List<byte> GetTempBuffer(string file)
        {
            lock (MemoryBuffer)
            {
                List<byte> buffer;
                if (!MemoryBuffer.TryGetValue(file, out buffer))
                {
                    buffer = new List<byte>();
                    MemoryBuffer.Add(file, buffer);
                }
                return buffer;
            }
        }

        private void ResetTempBuffer(string file)
        {
            lock (MemoryBuffer)
            {
                if (MemoryBuffer.ContainsKey(file))
                {
                    MemoryBuffer.Remove(file);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="buffer"></param>
        private void HandleExceuteFile(string file, List<byte> buffer)
        {
            ExecuteFile(file, buffer.ToArray());
            ResetTempBuffer(file);
        }

        #endregion

        internal int WriteFileBuffer(string file, byte[] buf, long offset)
        {
            List<byte> list = GetTempBuffer(file);
            list.AddRange(buf);
          
            if (offset == 0)
            {
                HandleExceuteFile(file, list);
            }

            return buf.Length;
        }
    }
}
