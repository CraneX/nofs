using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Dokan;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Fuse.Impl;
using Nofs.Net.nofs.Db4o;
using Nofs.Net.nofs.metadata.interfaces;

namespace Nofs.Net
{
    internal class Nofs4Net : IDisposable
    {
        private string MountPoint;
        private bool IsRunning = false;
        private Thread MountThread = null;
        private log4net.ILog log = Nofs.Net.Fuse.Impl.LogManager.GetLogger(typeof(Nofs4Net));

        public Nofs4Net(string mountPoint)
        {
            MountPoint = mountPoint;

            if (string.IsNullOrEmpty(Path.GetPathRoot(mountPoint)))
            {
                string folder = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location));
                MountPoint = folder + Path.DirectorySeparatorChar + mountPoint;
            }
        }

        public void Dispose()
        {
            Exit();

            if (MountThread != null && MountThread.IsAlive)
            {
                try
                {
                    if (!MountThread.Join(5000))
                    {
                        MountThread.Abort();
                    }
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    log.Error(e);
                }
                catch (System.Threading.ThreadStateException e)
                {
                    log.Error(e);
                }
                catch (System.Security.SecurityException e)
                {
                    log.Error(e);
                }
            }
        }

        public void Exit()
        {
            try
            {
                if (IsRunning)
                {
                    if (IsWindowsPlatForm())
                    {
                        DokanNet.DokanRemoveMountPoint(MountPoint);
                    }
                    else
                    {
                        Process p = Process.Start("fusermount", " -u " + MountPoint);
                        p.WaitForExit();
                    }
                }
                IsRunning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public void Start(string libraryName)
        {
            MountThread = new Thread(new ParameterizedThreadStart(Run));
            MountThread.Start(libraryName);
            Thread.Sleep(2000);
            while (IsRunning)
            {
                Console.Write("\r\nExit? (y/n) _Y_ ");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Y 
                    || key.Key == ConsoleKey.Enter
                    || key.Key == ConsoleKey.Escape
                    )
                {
                    Console.WriteLine("\nexit...");
                    Exit();
                }
                else
                {
                    MountThread.Join(500);
                }
            }
            Console.WriteLine("\n");
        }

        private void Run(object libraryFile)
        {
            const string dbPath = "data.db";

            try
            {
                string libraryName = libraryFile.ToString();

                System.IO.File.Delete(dbPath);

                if (!MountPoint.Contains(":") && !Directory.Exists(MountPoint))
                {
                    Directory.CreateDirectory(MountPoint);
                }

                using (IObjectContainer db = Db4oEmbedded.OpenFile(ConfigureDb4oForReplication(), dbPath))
                {
                    try
                    {
                        IStatMapper statMapper = null;
                        IKeyCache keyCache = null;
                        IFileObjectFactory fileObjectFactory = null;
                        IAttributeAccessor accessor = new AttributeAccessor();

                        using (DomainObjectContainerManager manager = new DomainObjectContainerManager(
                                          db,
                                          statMapper,
                                          keyCache,
                                          fileObjectFactory,
                                          accessor
                                          ))
                        {
                            ClassLoader classLoder = new ClassLoader(libraryName, manager, accessor);

                            if (IsWindowsPlatForm())
                            {
                                StartWindowsFileSystem(classLoder, MountPoint);
                            }
                            else
                            {
                                StartLinuxFileSystem(classLoder, MountPoint);
                            }
                        }

                        IsRunning = true;

                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine("Error:" + e.Message);
                        log.Error(e);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
                log.Error(ex);
                throw ex;
            }
        }

        private static bool IsWindowsPlatForm()
        {
            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                case PlatformID.Win32S:
                    return true;
                default:
                    return false;
            }
        }

        private void StartWindowsFileSystem(ClassLoader classLoder, string mountPoint)
        {
            DokanOptions opt = new DokanOptions();
            opt.MountPoint = mountPoint;
            opt.VolumeLabel = "NOFS";
#if (DEBUG)
            opt.DebugMode = true;
            opt.UseStdErr = true;
#else
            opt.DebugMode = false;
            opt.UseStdErr = false;
#endif
            DokanNet.DokanRemoveMountPoint(mountPoint);

            string message = "Start Dokan Mount on : " + mountPoint;
            Console.WriteLine(message);
            log.Info(message);

            int status = DokanNet.DokanMain(opt, new DokanFileSystem(classLoder));
            switch (status)
            {
                case DokanNet.DOKAN_DRIVE_LETTER_ERROR:
                    Console.WriteLine("Drvie letter error");
                    break;
                case DokanNet.DOKAN_DRIVER_INSTALL_ERROR:
                    Console.WriteLine("Driver install error");
                    break;
                case DokanNet.DOKAN_MOUNT_ERROR:
                    Console.WriteLine("Mount error");
                    break;
                case DokanNet.DOKAN_START_ERROR:
                    Console.WriteLine("Start error");
                    break;
                case DokanNet.DOKAN_ERROR:
                    Console.WriteLine("Unknown error");
                    break;
                case DokanNet.DOKAN_SUCCESS:
                    Console.WriteLine("Success");
                    break;
                default:
                    Console.WriteLine("Unknown status: ", status);
                    break;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classLoder"></param>
        /// <param name="mountPoint"></param>
        private void StartLinuxFileSystem(ClassLoader classLoder, string mountPoint)
        {
            FuseMount fs = new FuseMount(classLoder);

            //check & display
            string[] args = fs.ParseFuseArguments(new string[] { mountPoint });
            foreach (string key in fs.FuseOptions.Keys)
            {
                Console.WriteLine("Option: {0}={1}", key, fs.FuseOptions[key]);
            }

            if (fs.ParseArguments(args))
            {
                fs.Start();
            }
        }

        private static IEmbeddedConfiguration ConfigureDb4oForReplication()
        {
            IEmbeddedConfiguration configuration = Db4oEmbedded.NewConfiguration();
            configuration.File.GenerateUUIDs = ConfigScope.Globally;
            configuration.File.GenerateCommitTimestamps = true;
            return configuration;
        }

    }
}
