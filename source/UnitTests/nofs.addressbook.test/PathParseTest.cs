using Nofs.Net.nofs_addressbook;
using Nofs.Net.Utils;
using NUnit.Framework;

namespace nofs.fuse.test
{
    [TestFixture]
    [Category("PathParse")]
    class PathParseTest
    {
        [Test]
        public void TestPath()
        {
            string path = "/folder/foo";
            PathParser p = new PathParser(path);
            Assert.AreEqual(p.Folder, "/folder");
            Assert.AreEqual(p.FileName, "foo");

            path = "/folder/subfolder/bar";
            p = new PathParser(path);
            Assert.AreEqual(p.Folder, "/folder/subfolder");
            Assert.AreEqual(p.FileName, "bar");

            path = @"c:\testFolder\My Programs\dataFile";
            p = new PathParser(path);
            Assert.AreEqual(p.Folder, @"c:/testFolder/My Programs");
            Assert.AreEqual(p.FileName, "dataFile");
        }
    }
}
