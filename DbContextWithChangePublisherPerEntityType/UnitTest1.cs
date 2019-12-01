using System;
using ChangePublishingDbContextTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbContextWithChangePublisherPerEntityType
{
    [TestClass]
    public class TestDbContextWithChangesEventPerEntityType
    {
        public IChangePublishingTestContext Context = new ChangePublishingTestContext(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");

        [TestMethod]
        public void WhenAnEntityIsAddedAnEventShouldFire()
        {
            Context.AllChangesPublisher.Entity<TestEntity>().EntitiesChanged
        }
    }
}
