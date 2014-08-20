using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Mono.Fuse;
using Mono.Unix.Native;
using log4net;

namespace nofs.fuse.test
{
    class SimpleFS : FileSystem
    {
        private const string TestData = "<root>Test Data</root>";
        private const string Command = "echo $1 > ..method1";
        private List<string> names = new List<string>();
        private const int data_size = 10000;

        private ILog log;

        public SimpleFS()
            : base()
        {

            // Pull in log4net configuration
            log4net.Config.XmlConfigurator.Configure();
            if (log == null)
            {
                log = LogManager.GetLogger(typeof(SimpleFS));
            }

            names.Add("/folder");
            names.Add("/foo");
            names.Add("/bar");
            names.Add("/baz");
        }

        protected override Errno OnReadDirectory(
            string directory, 
            OpenedPathInfo info,
            out IEnumerable<DirectoryEntry> names)
        {
            LogInfo("OnReadDirectory:" + directory);

            if (directory != "/")
            {
                if (directory == "/folder")
                {

                }
                else
                {
                    names = null;
                    return Errno.ENOENT;
                }
            }

            names = ListNames(directory);
            return 0;
        }

        private IEnumerable<DirectoryEntry> ListNames(string directory)
        {
            foreach (string name in names)
            {
                yield return new DirectoryEntry(name.Substring(1));
            }
        }

        protected override Errno OnGetPathStatus(string path, out Stat stbuf)
        {
            LogInfo("OnGetPathStatus:" + path);

            stbuf = new Stat();
            if (path == "/" || path == "/folder")
            {
                stbuf.st_mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");
                stbuf.st_nlink = 1;
                return 0;
            }
            if (!names.Contains(path))
            {
                if (!path.StartsWith("/folder") && !path.StartsWith("/.."))
                {
                    return Errno.ENOENT;
                }
            }

            if (path == "/foo")
            {
                stbuf.st_mode = FilePermissions.S_IFREG | NativeConvert.FromUnixPermissionString("-rwxrwxrwx");
            }
            else
            {
                stbuf.st_mode = FilePermissions.S_IFREG | NativeConvert.FromOctalPermissionString("0444");
            }

            stbuf.st_nlink = 1;
            int size = 0;
            switch (path)
            {
                case "/bar":
                    size = TestData.Length;
                    break;
                case "/foo":
                    size = Command.Length;
                    break;
                case "/baz":
                    //case data_im_path: 
                    size = data_size;
                    break;
                default:
                    break;
            }

            stbuf.st_size = size;

            return 0;
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

        //protected override Errno OnFlushHandle(string file, OpenedPathInfo info)
        //{
        //    LogInfo("OnFlushHandle:" + file + " info " + BuildOpenedPathInfo(info));

        //    return 0;// base.OnFlushHandle(file, info);
        //}

        protected override Errno OnGetFileSystemStatus(string path, out Statvfs buf)
        {
            LogInfo("OnGetFileSystemStatus:" + path);

            return base.OnGetFileSystemStatus(path, out buf);
        }

        //protected override Errno OnGetHandleStatus(string file, OpenedPathInfo info, out Stat buf)
        //{
        //    LogInfo("OnGetHandleStatus:" + file + " info " + BuildOpenedPathInfo(info));

        //    return base.OnGetHandleStatus(file, info, out buf);
        //}

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

            if (!names.Contains(file) && !file.StartsWith("/.."))
            {
                return Errno.ENOENT;
            }

            if (info.OpenAccess != OpenFlags.O_RDONLY)
            {
                //if (info.OpenAccess == OpenFlags.O_APPEND | OpenFlags.O_WRONLY)
                if (file == "/foo" || file.StartsWith("/.."))
                {
                }
                else
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

            int size = buf.Length;
            if (file == "/bar")
            {
                byte[] source = Encoding.UTF8.GetBytes(TestData);

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
            else if (file == "/foo")
            {
                byte[] source = Encoding.UTF8.GetBytes(Command);

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
            else if (file == "/baz")
            {
                int max = System.Math.Min((int)data_size, (int)(offset + buf.Length));
                for (int i = 0, j = (int)offset; j < max; ++i, ++j)
                {
                    if ((j % 27) == 0)
                        buf[i] = (byte)'\n';
                    else
                        buf[i] = (byte)((j % 26) + 'a');
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

        //protected override Errno OnReleaseHandle(string file, OpenedPathInfo info)
        //{
        //    LogInfo("OnReleaseHandle:" + file + " info " + BuildOpenedPathInfo(info));

        //    return 0;//base.OnReleaseHandle(file, info);
        //}

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

        protected override Errno OnWriteHandle(string file, OpenedPathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            LogInfo("OnWriteHandle:" + file + " info " + BuildOpenedPathInfo(info) + " offset " + offset.ToString());
            LogInfo("value : " + UTF8Encoding.UTF8.GetString(buf));
         

            //TODO:
            //Excute function & change file here

            List<byte> list = new List<byte>();
            list.AddRange(buf);
            bytesRead = buf.Length;

            names.Add(file.Replace("/..", "/"));

            return 0;//base.OnWriteHandle(file, info, buf, offset, out bytesRead);
        }

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
            if (log != null)
            {
                log.Info(msg);
            }
            Console.WriteLine(msg);
        }

        private bool ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    //todo:
                    default:
                        base.MountPoint = args[i];
                        break;
                }
            }
            return true;
        }


        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: \r\n SimpleFS folder");
                return;
            }

            using (SimpleFS fs = new SimpleFS())
            {
                string[] unhandled = fs.ParseFuseArguments(args);
                foreach (string key in fs.FuseOptions.Keys)
                {
                    Console.WriteLine("Option: {0}={1}", key, fs.FuseOptions[key]);
                }
                if (!fs.ParseArguments(unhandled))
                    return;
                // fs.MountAt ("path" /* , args? */);

                fs.Start();
            }
        }
    }
}
