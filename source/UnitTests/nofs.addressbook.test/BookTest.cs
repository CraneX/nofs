using System;
using System.Linq;
using Nofs.Net.Common.Interfaces.Library;
using NUnit.Framework;
using Nofs.Net.nofs_addressbook;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.Fuse.Impl;

namespace nofs.fuse.test
{
    [TestFixture]
    [Category("Book")]
    class BookTest : BaseTest
    {
        public BookTest()
            : base()
        {

        }

        [Test]
        public void TestAddContact()
        {
            IAttributeAccessor accessor = new AttributeAccessor();

            Book b = new Book();
            accessor.SetContainerManagerIfAttributeExists(b, Fixture.Manager);
            accessor.SetContainerIfAttributeExists(b, Fixture.GetContainer(typeof(Book)));
            
            Assert.AreEqual(b.Categories.Count(), 0);

            Assert.AreEqual(b.Contacts.Count(), 0);

            b.AddAContact("Tom", "1234567890");
            Assert.AreEqual(b.Contacts.Count(), 1);

            Contact c = b.AddAContact("Jerry", "9876543210");
            Assert.AreEqual(b.getFirstTenContacts().Count(), 2);

            b.RenameAContact(c, "Mouse");
            Assert.AreEqual(c.Name, "Mouse");

            b.RemoveAContact(c);
            Assert.AreEqual(b.Contacts.Count(), 1);
            Assert.AreEqual(b.getFirstTenContacts().Count(), 1);

            c = b.SearchbyName("Tom");
            Assert.IsNotNull(c);
            b.RemoveAContact(c);
            Assert.AreEqual(b.Contacts.Count(), 0);
        }

        [Test]
        public void TestQueryContact()
        {
            IAttributeAccessor accessor = new AttributeAccessor();
            Book b = new Book();
            accessor.SetContainerManagerIfAttributeExists(b, Fixture.Manager);
            accessor.SetContainerIfAttributeExists(b, Fixture.GetContainer(typeof(Book)));

            Assert.AreEqual(b.Categories.Count(), 0);

            Assert.AreEqual(b.Contacts.Count(), 0);

            Contact c =  b.AddAContact("Tom", "1234567890");
            Assert.AreEqual(b.Contacts.Count(), 1);

            var container = Fixture.GetContainer(typeof(Contact));
            object obj = container.NewTransientInstance();
            Assert.IsTrue(obj is Contact);

            if (accessor.CanSetNameForObject(obj))
            {
                accessor.SetNameForObject(obj, "Tom");
                Assert.AreEqual(((Contact)obj).Name, "Tom");

                var o = container.GetByExample(obj);
                Assert.AreEqual(o, c);
            }


        }

    }
}
