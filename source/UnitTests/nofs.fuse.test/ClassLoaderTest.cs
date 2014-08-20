using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Nofs.Net.AnnotationDriver;
using NUnit.Framework;
using Nofs.Net.Fuse.Impl;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.nofs.Db4o;
using Nofs.Net.nofs.metadata.interfaces;

namespace nofs.fuse.test
{
    [TestFixture]
    [Category("ClassLoader")]
    class ClassLoaderTest : BaseTest
    {
        private ClassLoader Loader;
        private string fileName;
       
        public ClassLoaderTest()
            :base()
        {
            string folder = Path.GetDirectoryName(Path.GetFullPath(Process.GetCurrentProcess().MainModule.ModuleName));
            fileName = folder + @"\..\..\..\..\nofs-addressbook\bin\Debug\nofs-addressbook.dll";

            Loader = new ClassLoader(fileName, Fixture.Manager, new AttributeAccessor());
        }

        [Test]
        public void TestRootClass()
        {
            Assert.AreEqual(Loader.RootClass.Name, "Book");
            Assert.AreEqual(Loader.RootClass.FullName, "Nofs.Net.nofs_addressbook.Book");
        }

        [Test]
        public void TestFoldersList()
        {
            Console.WriteLine("List folders List");
            IEnumerable<string> folders = Loader.GetFolders(Loader.RootClass);
            Console.WriteLine(string.Empty);
            foreach (string s in folders)
            {
                Assert.IsTrue(s.StartsWith("/"));
                Assert.IsTrue(Loader.IsFolder(s));
                Console.WriteLine(s);
            }
            Assert.AreEqual(folders.Count(), 3 + 1);//root folder

            Loader.RemoveFolder("/");
            Assert.AreEqual(folders.Count(), 3);//root folder
            
        }

        [Test]
        public void TestFilesList()
        {
            Console.WriteLine("List File List");
            IEnumerable<string> files = Loader.GetFiles(Loader.RootClass, "/");
            Console.WriteLine(string.Empty);
            foreach (string s in files)
            {
                Console.WriteLine(s);
                if (s == "AddAContact")
                {
                    Assert.IsTrue(Loader.IsExcuteFile("/" + s));
                }
            }
            Assert.AreEqual(files.Count(), 5);
        }

        [Test]
        public void TestFilesContent()
        {
            IEnumerable<string> files = Loader.GetFiles(Loader.RootClass, "/");
            Console.WriteLine(string.Empty);
            Console.WriteLine("List File Detail Contecn");
            foreach (string fileName in files)
            {
                string s = Loader.GetFileContent("/", fileName);
                Console.WriteLine(s);
                
                if (!s.Contains("﻿<?xml"))
                {
                    switch (System.Environment.OSVersion.Platform)
                    {
                        case PlatformID.Win32Windows:
                        case PlatformID.Win32NT:
                        case PlatformID.WinCE:
                        case PlatformID.Win32S:
                            Assert.IsTrue(s.StartsWith("Const"));
                            Assert.IsTrue(s.Contains("Scripting.FileSystemObject"));
                            Assert.IsTrue(s.Contains("wscript.Arguments"));
                            break;
                        default:
                            Assert.IsTrue(s.StartsWith("echo"));
                            Assert.IsTrue(s.Contains("$1"));
                            Assert.IsTrue(s.Contains(" > .."));
                            break;
                    }
                }
            }
            Assert.AreEqual(files.Count(), 5);
        }

        [Test]
        public void TestAddAndRemove()
        {
            Loader.ClearCache();
            object o = Loader.ExecuteFile("/", "AddAContact", "a", "b");
            Assert.IsNotNull(o);

            Loader.ExecuteFile("/", "RemoveAContact", "a");
            IEnumerable<string> files = Loader.GetFiles("/Contacts");
            Assert.AreEqual(files.Count(), 0);
        }

        [Test]
        public void TestFilesInSubFolder()
        {
            object o = Loader.ExecuteFile("/", "AddAContact", "a", "b");
            Assert.IsNotNull(o);
            IEnumerable<string> files = Loader.GetFiles("/FirstTenContacts");
            Assert.AreEqual(files.Count(), 1);
            Assert.AreEqual(files.First<string>(), "a");

            files = Loader.GetFiles("/Contacts");
            Assert.AreEqual(files.Count(), 1);

            files = Loader.GetFiles("/Categories");
            Assert.AreEqual(files.Count(), 0);

            files = Loader.GetFiles("/FirstTenContacts");
            Assert.AreEqual(files.Count(), 1);

            Loader.ExecuteFile("/", "RenameAContact", "a", "newNameAAA");
            files = Loader.GetFiles("/Contacts");
            Assert.AreEqual(files.Count(), 1);
            Assert.AreEqual(files.First<string>(), "newNameAAA");

            Loader.ExecuteFile("/", "RemoveAContact", "/Contacts/newNameAAA");
            files = Loader.GetFiles("/Contacts");
            Assert.AreEqual(files.Count(), 0);


            for (int i = 0; i < 20; ++i)
            {
                o = Loader.ExecuteFile("/", "AddAContact", i.ToString(), Guid.NewGuid().ToString());
                Assert.IsNotNull(o);
            }

            files = Loader.GetFiles("/FirstTenContacts");
            Assert.AreEqual(files.Count(), 10);

            files = Loader.GetFiles("/Contacts");
            Assert.AreEqual(files.Count(), 20);
        }
    }
}
