using System.Collections.Generic;
using System.Linq;
using Nofs.Net.Common.Interfaces.Library;
using NUnit.Framework;
using Nofs.Net.nofs_addressbook;

namespace nofs.fuse.test
{
    [TestFixture]
    [Category("DomainObjectContainer")]
    class DomainObjectContainerTest : BaseTest
    {
        public DomainObjectContainerTest()
            :base()
        {
            
        }

        [Test]
        public void TestLoadClass()
        {
            IDomainObjectContainer container = Fixture.GetContainer(typeof(Contact));

            Contact c = container.NewPersistentInstance<Contact>();
            Assert.IsNotNull(c);

            Assert.AreEqual(container.GetObjectCountForTesting(), 1);

            Contact c2 = container.NewPersistentInstance<Contact>();
            Assert.IsNotNull(c2);

            IEnumerable<Contact> list = container.GetByExample<Contact>(c);
            Assert.AreEqual(list.Count(), 2);

            container.Remove<Contact>(c2);
            Assert.AreEqual(container.GetByExample<Contact>(c).Count(), 1);

        }

    }
}
