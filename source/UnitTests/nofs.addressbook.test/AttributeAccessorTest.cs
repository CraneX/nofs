using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nofs.Net.nofs_addressbook;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Fuse.Impl;

namespace nofs.test
{
    [TestFixture]
    [Category("AttributeAccessor")]
    class AttributeAccessorTest
    {
        [Test]
        public void TestGetNameFromObject()
        {
            Contact c = new Contact("Tom", "12345678");
            IAttributeAccessor accessor = new AttributeAccessor();
            Assert.AreEqual("Tom", accessor.GetNameFromObject(c));
        }
    }
}
