using System;
using System.Linq;
using System.Collections.Generic;
using Nofs.Net.Common.Interfaces.Library;
using NUnit.Framework;
using Nofs.Net.nofs_addressbook;

namespace nofs.fuse.test
{
    [TestFixture]
    [Category("DomainObjectContainerManager")]
    class DomainObjectContainerManagerTest : BaseTest
    {
        public DomainObjectContainerManagerTest()
            :base()
        {
        }

        [Test]
        public void TestManager()
        {
            IDomainObjectContainer container = Fixture.Manager.GetContainer(typeof(Contact));

            Contact c = container.NewPersistentInstance<Contact>();
            Assert.IsNotNull(c);

            Assert.AreEqual(container.GetObjectCountForTesting(), 1);

            Contact c2 = container.NewPersistentInstance<Contact>();
            Assert.IsNotNull(c2);

            IEnumerable<Contact> list = container.GetByExample<Contact>(c);
            Assert.AreEqual(list.Count(), 2);
            Assert.AreEqual(list.Count(), Fixture.Manager.GetObjectCountForTesting());

            IDomainObjectContainer categoryContainer = Fixture.Manager.GetContainer(typeof(Category));
            Category g = categoryContainer.NewPersistentInstance<Category>();
            Assert.AreEqual(categoryContainer.GetObjectCountForTesting(), 1);
            Assert.AreEqual(Fixture.Manager.GetObjectCountForTesting(), 3);

            container.Remove<Contact>(c2);
            Assert.AreEqual(container.GetByExample<Contact>(c).Count(), 1);


            Assert.AreEqual(container, Fixture.Manager.GetContainer(typeof(Contact)));
        }
    }
}
