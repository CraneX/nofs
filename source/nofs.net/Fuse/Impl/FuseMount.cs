using System;
using System.Collections.Generic;
using System.Text;
using Mono.Fuse;
using Mono.Unix.Native;

namespace Nofs.Net.Fuse.Impl
{
    /// <summary>
    /// Mount Fuse FileSystem, Main Entry
    /// </summary>
    public class FuseMount : FileSystem
    {
        private log4net.ILog log = LogManager.GetLogger(typeof(FuseMount));
        private BaseFileSystem baseFileSystem;
        
        public FuseMount(IFuseObjectTree tree)
            : base()
        {
            baseFileSystem = new BaseFileSystem(tree);
        }

        public bool ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        FileSystem.ShowFuseHelp("Nofs.Net");
                        Console.Error.WriteLine("Nofs.Net options:");
                        Console.Error.WriteLine("    --data.im-in-memory    Add data.im file");
                        return false;
                    default:
                        base.MountPoint = args[i];
                        break;
                }
            }
            return true;
        }

        #region Fuse Event Handlder

        protected override Errno OnReadDirectory(
            string directory,
            OpenedPathInfo info,
            out IEnumerable<DirectoryEntry> names)
        {
            LogInfo("OnReadDirectory:" + directory);

            if (baseFileSystem.IsFolder(directory))
            {
                names = ListNames(directory);
                return 0;
            }
            else
            {
                names = null;
                return Errno.ENOENT;
            }
        }

        protected override Errno OnGetPathStatus(string path, out Stat stbuf)
        {
            LogInfo("OnGetPathStatus:" + path);

            stbuf = new Stat();

            if (baseFileSystem.IsFolder(path))
            {
                stbuf.st_mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");
                stbuf.st_nlink = 1;
                return 0;
            }
            else //file
            {
                if (baseFileSystem.IsValidFiles(path))
                {
                    if (baseFileSystem.IsExcuteFile(path))
                    {
                        stbuf.st_mode = FilePermissions.S_IFREG | NativeConvert.FromUnixPermissionString("-r-xr-xr-x");
                    }
                    else if (baseFileSystem.IsTempFile(path))
                    {
                        stbuf.st_mode = FilePermissions.S_IFREG | NativeConvert.FromUnixPermissionString("-rwxrwxrwx");
                    }
                    else
                    {
                        stbuf.st_mode = FilePermissions.S_IFREG | NativeConvert.FromOctalPermissionString("0444");
                    }
                    stbuf.st_nlink = 1;
                    stbuf.st_size = baseFileSystem.GetFileLength(path);
                    return 0;
                }
                else
                {
                    return Errno.ENOENT;
                }
            }
        }

        protected override Errno OnOpenDirectory(string directory, OpenedPathInfo info)
        {
            LogInfo("OnOpenDirectory:" + directory);

            return 0;// base.OnOpenDirectory(directory, info);
        }

        protected override Errno OnReleaseDirectory(string directory, OpenedPathInfo info)
        {
            LogInfo("OnReleaseDirectory:" + directory);

            return 0; //base.OnReleaseDirectory(directory, info);
        }

        protected override Errno OnAccessPath(string path, AccessModes mode)
        {
            LogInfo("OnAccessPath:" + path + " mode " + mode.ToString());

            return 0; //base.OnAccessPath(path, mode);
        }

        protected override Errno OnChangePathOwner(string path, long owner, long group)
        {
            LogInfo("OnChangePathOwner:" + path + " owner: " + owner.ToString() + " group:" + group.ToString());

            return 0;//base.OnChangePathOwner(path, owner, group);
        }

        protected override Errno OnChangePathPermissions(string path, FilePermissions mode)
        {
            LogInfo("OnChangePathPermissions:" + path + " mode " + mode.ToString());

            return 0;//base.OnChangePathPermissions(path, mode);
        }

        protected override Errno OnChangePathTimes(string path, ref Utimbuf buf)
        {
            LogInfo("OnChangePathTimes:" + path + " Utimbuf " + buf.ToString());

            return 0;//base.OnChangePathTimes(path, ref  buf);
        }

        protected override Errno OnCreateDirectory(string directory, FilePermissions mode)
        {
            LogInfo("OnCreateDirectory:" + directory + " mode " + mode.ToString());

            return 0;//base.OnCreateDirectory(directory, mode);
        }

        protected override Errno OnCreateHandle(string file, OpenedPathInfo info, FilePermissions mode)
        {
            LogInfo("OnCreateHandle:" + file + " info " + BuildOpenedPathInfo(info) + " mode " + mode.ToString());

            return 0;//base.OnCreateHandle(file, info, mode);
        }

        protected override Errno OnCreateHardLink(string oldpath, string link)
        {
            LogInfo("OnCreateHardLink:" + oldpath + " link " + link);

            return 0;//base.OnCreateHardLink(oldpath, link);
        }

        protected override Errno OnCreateSpecialFile(string file, FilePermissions perms, ulong dev)
        {
            LogInfo("OnCreateSpecialFile:" + file + " perms " + perms.ToString() + " dev " + dev.ToString());

            return 0;// base.OnCreateSpecialFile(file, perms, dev);
        }

        protected override Errno OnCreateSymbolicLink(string target, string link)
        {
            LogInfo("OnCreateSymbolicLink:" + target + " link " + link);

            return 0;//base.OnCreateSymbolicLink(target, link);
        }

        protected override Errno OnGetFileSystemStatus(string path, out Statvfs buf)
        {
            LogInfo("OnGetFileSystemStatus:" + path);

            return base.OnGetFileSystemStatus(path, out buf);
        }

        protected override Errno OnGetPathExtendedAttribute(string path, string name, byte[] value, out int bytesWritten)
        {
            LogInfo("OnGetPathExtendedAttribute:" + path + " name " + name);

            bytesWritten = 0;

            return 0;// base.OnGetPathExtendedAttribute(path, name, value, out bytesWritten);
        }

        protected override Errno OnListPathExtendedAttributes(string path, out string[] names)
        {
            LogInfo("OnListPathExtendedAttributes:" + path);

            names = new string[] { };

            return 0; //base.OnListPathExtendedAttributes(path, out names);
        }

        protected override Errno OnOpenHandle(string file, OpenedPathInfo info)
        {
            LogInfo("OnOpenHandle:" + file + " info " + BuildOpenedPathInfo(info));

            if (!baseFileSystem.IsValidFiles(file))
            {
                return Errno.ENOENT;
            }

            if (info.OpenAccess != OpenFlags.O_RDONLY)
            {
                if (info.OpenAccess == OpenFlags.O_WRONLY
                    && baseFileSystem.IsTempFile(file))
                {
                    return 0;
                }

                if (!baseFileSystem.IsFolder(file) && !baseFileSystem.IsExcuteFile(file))
                {
                    return Errno.EACCES;
                }
            }

            return 0;
        }

        protected override Errno OnReadHandle(string file, OpenedPathInfo info, byte[] buf, long offset, out int bytesWritten)
        {
            LogInfo("OnReadHandle:" + file + " info " + BuildOpenedPathInfo(info) + " offset " + offset.ToString());

            Errno errNo = 0;
            int size = 0;
            if (baseFileSystem.IsValidFiles(file))
            {
                size = buf.Length;
                byte[] source = baseFileSystem.GetFileData(file);
                if (offset < (long)source.Length)
                {
                    if (offset + (long)size > (long)source.Length)
                        size = (int)((long)source.Length - offset);
                    Buffer.BlockCopy(source, (int)offset, buf, 0, size);
                }
                else
                {
                    size = 0;
                }
            }
            else
            {
                errNo = Errno.ENOENT;
            }

            bytesWritten = size;

            return errNo;
        }

        protected override Errno OnReadSymbolicLink(string link, out string target)
        {
            LogInfo("OnReadSymbolicLink:" + link);
            target = string.Empty;
            return 0;//base.OnReadSymbolicLink(link, out target);
        }

        protected override Errno OnRemoveDirectory(string directory)
        {
            LogInfo("OnRemoveDirectory:" + directory);

            return 0;//base.OnRemoveDirectory(directory);
        }

        protected override Errno OnRemoveFile(string file)
        {
            LogInfo("OnRemoveFile:" + file);

            return 0;//base.OnRemoveFile(file);
        }

        protected override Errno OnRemovePathExtendedAttribute(string path, string name)
        {
            LogInfo("OnRemovePathExtendedAttribute:" + path + " name " + name);

            return 0;//base.OnRemovePathExtendedAttribute(path, name);
        }

        protected override Errno OnRenamePath(string oldpath, string newpath)
        {
            LogInfo("OnRenamePath:" + oldpath + " newpath " + newpath);

            return 0;//base.OnRenamePath(oldpath, newpath);
        }

        protected override Errno OnSetPathExtendedAttribute(string path, string name, byte[] value, XattrFlags flags)
        {
            LogInfo("OnSetPathExtendedAttribute:" + path + " name " + name + " flags " + flags.ToString());

            return 0;// base.OnSetPathExtendedAttribute(path, name, value, flags);
        }

        protected override Errno OnSynchronizeDirectory(string directory, OpenedPathInfo info, bool onlyUserData)
        {
            LogInfo("OnSynchronizeDirectory:" + directory + " info " + BuildOpenedPathInfo(info) + " onlyUserData " + onlyUserData.ToString());

            return 0;//base.OnSynchronizeDirectory(directory, info, onlyUserData);
        }

        protected override Errno OnSynchronizeHandle(string file, OpenedPathInfo info, bool onlyUserData)
        {
            LogInfo("OnSynchronizeHandle:" + file + " info " + BuildOpenedPathInfo(info) + " onlyUserData " + onlyUserData.ToString());

            return 0;//base.OnSynchronizeHandle(file, info, onlyUserData);
        }

        protected override Errno OnTruncateFile(string file, long length)
        {
            LogInfo("OnTruncateFile:" + file + " length " + length.ToString());

            return 0;//base.OnTruncateFile(file, length);
        }

        protected override Errno OnTruncateHandle(string file, OpenedPathInfo info, long length)
        {
            LogInfo("OnTruncateHandle:" + file + " info " + BuildOpenedPathInfo(info) + " length " + length.ToString());

            return 0;//base.OnTruncateHandle(file, info, length);
        }

        /// <summary>
        /// Excute file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="info"></param>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <param name="bytesRead"></param>
        /// <returns></returns>
        protected override Errno OnWriteHandle(string file, OpenedPathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            LogInfo("OnWriteHandle:" + file + " info " + BuildOpenedPathInfo(info) + " offset " + offset.ToString());
            LogInfo("value : " + UTF8Encoding.UTF8.GetString(buf));

            //Excute function & change file here

            bytesRead = baseFileSystem.WriteFileBuffer(file, buf, offset);

            return 0;//base.OnWriteHandle(file, info, buf, offset, out bytesRead);
        }

        #endregion

        #region Log Info

        private string BuildOpenedPathInfo(OpenedPathInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DirectIO:\t" + info.DirectIO.ToString());
            sb.Append(" Handle:\t" + info.Handle.ToString());
            sb.Append(" KeepCache:\t" + info.KeepCache.ToString());
            sb.Append(" OpenAccess:\t" + info.OpenAccess.ToString());
            sb.Append(" OpenFlags:\t" + info.OpenFlags.ToString());
            sb.Append(" WritePage:\t" + info.WritePage.ToString());
            return sb.ToString();
        }

        private void LogInfo(string msg)
        {
            LogManager.LogInfo(log, msg);
        }

        #endregion

        #region Implement File Handler

        private IEnumerable<DirectoryEntry> ListNames(string directory)
        {
            foreach (string fileName in baseFileSystem.GetFolders(directory))
            {
                yield return new DirectoryEntry(fileName.Substring(1));
            }

            foreach (string fileName in baseFileSystem.GetFiles(directory))
            {
                yield return new DirectoryEntry(fileName);
            }
        }

        #endregion


    }
}
