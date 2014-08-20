using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dokan;
using System.Diagnostics;

namespace Nofs.Net.Fuse.Impl
{
    public class DokanFileSystem : DokanOperations
    {
        private const string RootPath = @"\";
        private const string ExcuteExeFileName = ".vbs";
        private log4net.ILog log = LogManager.GetLogger(typeof(DokanFileSystem));
        private BaseFileSystem baseFileSystem;
        //private Dictionary<Guid, string> FileNameCache = new Dictionary<Guid, string>();

        public DokanFileSystem(IFuseObjectTree tree)
        {
            baseFileSystem = new BaseFileSystem(tree);
        }

        public int Cleanup(string filename, DokanFileInfo info)
        {
            LogInfo("Cleanup: " + filename + BuildDokanFileInfoInfo(info));

            return DokanNet.DOKAN_SUCCESS;
        }

        public int CloseFile(string filename, DokanFileInfo info)
        {
            LogInfo("CloseFile: " + filename + BuildDokanFileInfoInfo(info));

            return DokanNet.DOKAN_SUCCESS;
        }

        public int CreateDirectory(string filename, DokanFileInfo info)
        {
            LogInfo("CreateDirectory: " + filename + BuildDokanFileInfoInfo(info));

            return DokanNet.DOKAN_ERROR;
        }

        public int CreateFile(
            string filename,
            System.IO.FileAccess access,
            System.IO.FileShare share,
            System.IO.FileMode mode,
            System.IO.FileOptions options,
            DokanFileInfo info)
        {
            LogInfo("CreateFile: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_SUCCESS;
        }

        public int DeleteDirectory(string filename, DokanFileInfo info)
        {
            LogInfo("DeleteDirectory: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_ERROR;
        }

        public int DeleteFile(string filename, DokanFileInfo info)
        {
            LogInfo("DeleteFile: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_ERROR;
        }

        public int FlushFileBuffers(
           string filename,
           DokanFileInfo info)
        {
            LogInfo("FlushFileBuffers: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_ERROR;
        }

        public int FindFiles(
            string filename,
            System.Collections.ArrayList files,
            DokanFileInfo info)
        {
            LogInfo("FindFiles: " + filename + BuildDokanFileInfoInfo(info));

            filename = ReplaceSlash(filename);

            foreach (string name in baseFileSystem.GetFolders(filename))
            {
                FileInformation finfo = new FileInformation();
                finfo.FileName = name.Substring(1);
                finfo.Attributes = System.IO.FileAttributes.Directory;
                finfo.LastAccessTime = DateTime.Now;
                finfo.LastWriteTime = DateTime.Now;
                finfo.CreationTime = DateTime.Now;
                if (files != null)
                {
                    files.Add(finfo);
                }
                //FileNameCache.Add(Guid.NewGuid(), finfo.FileName);
            }

            foreach (string name in baseFileSystem.GetFiles(filename))
            {
                string path = BuildFileName(filename, name);
                FileInformation finfo = new FileInformation();
                finfo.FileName = name;
                if (baseFileSystem.IsExcuteFile(path))
                {
                    finfo.FileName += ExcuteExeFileName;
                }
                finfo.Attributes = System.IO.FileAttributes.Normal;
                finfo.LastAccessTime = DateTime.Now;
                finfo.LastWriteTime = DateTime.Now;
                finfo.CreationTime = DateTime.Now;
                finfo.Length = baseFileSystem.GetFileLength(path);
                if (files != null)
                {
                    files.Add(finfo);
                }

                //FileNameCache.Add(Guid.NewGuid(), finfo.FileName);
            }
            return DokanNet.DOKAN_SUCCESS;
        }

        public int GetFileInformation(
            string filename,
            FileInformation fileinfo,
            DokanFileInfo info)
        {
            LogInfo("GetFileInformation: " + filename + BuildFileInformation(fileinfo) + BuildDokanFileInfoInfo(info));

            filename = ReplaceSlash(filename);

            if (baseFileSystem.IsFolder(filename))
            {
                fileinfo.Attributes = System.IO.FileAttributes.Directory;
                fileinfo.LastAccessTime = DateTime.Now;
                fileinfo.LastWriteTime = DateTime.Now;
                fileinfo.CreationTime = DateTime.Now;
                return DokanNet.DOKAN_SUCCESS;
            }

            else if (baseFileSystem.IsValidFiles(filename))
            {
                fileinfo.Attributes = System.IO.FileAttributes.Normal;
                fileinfo.LastAccessTime = DateTime.Now;
                fileinfo.LastWriteTime = DateTime.Now;
                fileinfo.CreationTime = DateTime.Now;
                fileinfo.Length = baseFileSystem.GetFileLength(filename);
                return DokanNet.DOKAN_SUCCESS;
            }


            return DokanNet.DOKAN_ERROR;
        }

        public int LockFile(
            string filename,
            long offset,
            long length,
            DokanFileInfo info)
        {
            LogInfo("LockFile: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_SUCCESS;
        }

        public int MoveFile(
            string filename,
            string newname,
            bool replace,
            DokanFileInfo info)
        {
            LogInfo("MoveFile: " + filename + " newFileName: " + newname);
            return DokanNet.DOKAN_ERROR;
        }

        public int OpenDirectory(string filename, DokanFileInfo info)
        {
            LogInfo("OpenDirectory: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_SUCCESS;
        }

        public int ReadFile(
            string filename,
            byte[] buffer,
            ref uint readBytes,
            long offset,
            DokanFileInfo info)
        {
            LogInfo("ReadFile: " + filename + BuildDokanFileInfoInfo(info));

            filename = ReplaceSlash(filename);

            if (baseFileSystem.IsValidFiles(filename))
            {
                int size = buffer.Length;

                byte[] source = baseFileSystem.GetFileData(filename);
                if (offset < (long)source.Length)
                {
                    if (offset + (long)size > (long)source.Length)
                    {
                        size = (int)((long)source.Length - offset);
                    }
                    Buffer.BlockCopy(source, (int)offset, buffer, 0, size);
                }
                else
                {
                    size = 0;
                }

                readBytes = (uint)size;

                return DokanNet.DOKAN_SUCCESS;
            }
            return DokanNet.DOKAN_ERROR;
        }

        public int SetEndOfFile(string filename, long length, DokanFileInfo info)
        {
            LogInfo("SetEndOfFile: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_ERROR;
        }

        public int SetAllocationSize(string filename, long length, DokanFileInfo info)
        {
            LogInfo("SetAllocationSize: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_ERROR;
        }

        public int SetFileAttributes(
            string filename,
            System.IO.FileAttributes attr,
            DokanFileInfo info)
        {
            LogInfo("SetFileAttributes: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_ERROR;
        }

        public int SetFileTime(
            string filename,
            DateTime ctime,
            DateTime atime,
            DateTime mtime,
            DokanFileInfo info)
        {
            LogInfo("SetFileTime: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_ERROR;
        }

        public int UnlockFile(string filename, long offset, long length, DokanFileInfo info)
        {
            LogInfo("UnlockFile: " + filename + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_SUCCESS;
        }

        public int Unmount(DokanFileInfo info)
        {
            LogInfo("Unmount: " + BuildDokanFileInfoInfo(info));
            return DokanNet.DOKAN_SUCCESS;
        }

        public int GetDiskFreeSpace(
           ref ulong freeBytesAvailable,
           ref ulong totalBytes,
           ref ulong totalFreeBytes,
           DokanFileInfo info)
        {
            LogInfo("GetDiskFreeSpace: " + BuildDokanFileInfoInfo(info));

            freeBytesAvailable = 512 * 1024 * 1024;
            totalBytes = 1024 * 1024 * 1024;
            totalFreeBytes = 512 * 1024 * 1024;
            return DokanNet.DOKAN_SUCCESS;
        }

        public int WriteFile(
            string filename,
            byte[] buffer,
            ref uint writtenBytes,
            long offset,
            DokanFileInfo info)
        {
            string s = UTF8Encoding.UTF8.GetString(buffer);

            LogInfo("WriteFile: " + BuildDokanFileInfoInfo(info) + " writtenBytes: " + writtenBytes.ToString() + " offset:" + offset.ToString());

            writtenBytes = (uint)baseFileSystem.WriteFileBuffer(filename, buffer, offset);

            return DokanNet.DOKAN_SUCCESS;
        }

        private static string ReplaceSlash(string path)
        {
            return path.Replace(RootPath, ClassLoader.RootFolder).Replace(ExcuteExeFileName, string.Empty);
        }

        private static string BuildFileName(string path, string name)
        {
            if (!path.EndsWith("/"))
            {
                path += "/";
            }
            return path + name;
        }

        #region Log Info

        private void LogInfo(string message)
        {
            Trace.TraceInformation(message);

            log.Info(message);
        }

        private string BuildFileInformation(FileInformation info)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nFileAttributes:\t" + info.Attributes.ToString());
            sb.Append(" CreationTime:\t" + info.CreationTime.ToString());
            sb.Append(" FileName:\t" + info.FileName);
            sb.Append(" LastAccessTime:\t" + info.LastAccessTime.ToString());
            sb.Append(" LastWriteTime:\t" + info.LastWriteTime.ToString());
            sb.Append(" Length:\t" + info.Length.ToString());
            return sb.ToString();
        }

        private string BuildDokanFileInfoInfo(DokanFileInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nContext:\t" + (info.Context == null ? "null" : info.Context.ToString()));
            sb.Append(" DeleteOnClose:\t" + info.DeleteOnClose.ToString());
            sb.Append(" DokanContext:\t" + info.DokanContext.ToString());
            sb.Append(" InfoId:\t" + info.InfoId.ToString());
            sb.Append(" IsDirectory:\t" + info.IsDirectory.ToString());
            sb.Append(" Nocache:\t" + info.PagingIo.ToString());
            sb.Append(" ProcessId:\t" + info.ProcessId.ToString());
            sb.Append(" SynchronousIo:\t" + info.SynchronousIo.ToString());
            sb.Append(" WriteToEndOfFile:\t" + info.WriteToEndOfFile.ToString());
            return sb.ToString();
        }

        #endregion

    }

}
