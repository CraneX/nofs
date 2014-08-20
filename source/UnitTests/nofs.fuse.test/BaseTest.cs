using NUnit.Framework;

namespace nofs.fuse.test
{
    internal abstract class BaseTest
    {
        protected TestFixture Fixture;
        public BaseTest()
        {
            Fixture = new TestFixture();
        }
        
        [TestFixtureSetUp]
        public void MyTestInitialize()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
          
        }

        [TestFixtureTearDown]
        public void MyTestCleanup()
        {
            Cleanup();
        }

        public virtual void Cleanup()
        {
            if (Fixture != null)
            {
                Fixture.Dispose();
            }
           
        }
       
    }
}
