using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChangePublishingDbContext;
using System.Collections;
using System.Collections.Generic;
using ChangePublishingDbContext.Implementation;

namespace ChangePublishingDbContextTest
{
    [TestClass]
    public class ChangeTrackerTest
    {
        //public ITestContext Context = new TestContextWithSaveEvent(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
        public ITestContextWithSaveEvent Context = new TestContextWithSaveEvent (@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");

        public IEntityChangeTracker _changeTracker;
        public IEntityChangeTracker ChangeTracker => _changeTracker == null ? _changeTracker = new EntityChangeTracker(Context) : _changeTracker;

        [TestMethod]
        public void EntitiesChanged_ShouldFireWhenAnEntityIsAdded()
        {
            var count = 0;

            ChangeTracker.Entity<TestEntity>().EntitiesChanged += (entities) => count++;

            Context.TestEntities.Add(new TestEntity { Key = 1, Description = "b" });
            Context.SaveChanges();

            Assert.IsTrue(count == 1);
        }

        [TestMethod]
        public void EntitiesChanged_ShouldNotFireWhenADifferentEntityTypeIsAdded()
        {
            var count = 0;

            ChangeTracker.Entity<TestEntity>().EntitiesChanged += (entities) => count++;

            Context.TestEntities2.Add(new TestEntity2 { Key = 1 });
            Context.SaveChanges();

            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void EntitiesChanged_ShouldFireWhenARelationshipIsChanged()
        {
            var count = 0;

            var entity1 = new TestEntity { Key = 2, Description = "b" };
            Context.TestEntities.Add(entity1);
            Context.SaveChanges();

            var entity2 = new TestEntity2 { Key = 1, TestEntities = new List<TestEntity> { entity1 } };
            ChangeTracker.Entity<TestEntity>().EntitiesChanged += (entities) => count++;

            Context.TestEntities2.Add(entity2);
            Context.SaveChanges();

            Assert.IsTrue(count == 1);
        }
    }
}
