using System;
using System.Collections.Generic;
using System.Linq;
using Nofs.Net.Utils;
using NUnit.Framework;
using Nofs.Net.nofs_addressbook;
using System.Text;
using Nofs.Net.nofs.stocks;

namespace nofs.fuse.test
{
    [TestFixture]
    [Category("StreamUtil")]
    class StreamUtilTest
    {
        [Test]
        public void TestSerializeToString()
        {
            Category c = new Category();
            c.Add(new Contact("Tom", "Cat"));
            c.Add(new Contact("Jerry", "Mouse"));
            string s = StreamUtil.SerializeToString(c);
            Console.WriteLine(string.Empty);
            Console.WriteLine(s);
            Assert.Greater(s.Length, 0);

            BookInfo b = new BookInfo();
            s = StreamUtil.SerializeToString(b);
            Console.WriteLine(s);
            Assert.Greater(s.Length, 350);

        }

        [Test]
        public void TestStockSerializeToString()
        {
            Stock s = new Stock("MORN");
            string line = StreamUtil.SerializeToString(s);
            Console.WriteLine(line);
            Assert.AreEqual(line.Length, 282);
            s.UpdateData("MORN, 56.94, 2011-01-02, 10:10");
            line = StreamUtil.SerializeToString(s);
            Console.WriteLine(line);
            Assert.Greater(line.Length, 100);
        }
    }
}
