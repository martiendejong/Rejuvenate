using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChangePublishingDbContext;
using System.Collections;
using System.Collections.Generic;
using ChangePublishingDbContext.Implementation;
using System.Linq;

namespace ChangePublishingDbContextTest
{
    [TestClass]
    public class ChangePublishingDbContextTest
    {
        //public ITestContext Context = new TestContextWithSaveEvent(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
        public IChangePublishingTestContext Context = new ChangePublishingTestContext(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");



        public IEntityChangeTracker _changeTracker;
        public IEntityChangeTracker ChangeTracker => _changeTracker == null ? _changeTracker = new EntityChangeTracker(Context) : _changeTracker;

        public IConditionalChangeTrackerManager<TestEntity> _conditionalChangeTrackerManager;
        public IConditionalChangeTrackerManager<TestEntity> ConditionalChangeTrackerManager => _conditionalChangeTrackerManager == null ? _conditionalChangeTrackerManager = new ConditionalChangeTrackerManager<TestEntity>(ChangeTracker.Entity<TestEntity>()) : _conditionalChangeTrackerManager;



        [TestMethod]
        public void EntitiesChanged_ShouldFireWhenAnEntityIsAddedThatMeetsTheConditions()
        {
            var count = 0;

            Context.TestEntities.Where(entity => entity.Description.StartsWith("b")).EntitiesChanged += (entities) => count++;

            Context.TestEntities.Add(new TestEntity { Key = 1, Description = "b" });
            Context.SaveChanges();

            Assert.IsTrue(count == 1);
        }

        [TestMethod]
        public void EntitiesChanged_ShouldNotFireWhenAnEntityTypeIsAddedThatDoesNotMeetTheConditions()
        {
            var count = 0;

            Context.TestEntities.Where(entity => entity.Description.StartsWith("q")).EntitiesChanged += (entities) => count++;

            Context.TestEntities.Add(new TestEntity { Key = 1, Description = "b" });
            Context.SaveChanges();

            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void EntitiesChanged_ShouldFireWhenARelationshipIsChanged()
        {
            var count = 0;

            var entity1 = new TestEntity { Key = 2, Description = "q" };
            Context.TestEntities.Add(entity1);
            Context.SaveChanges();

            Context = new ChangePublishingTestContext(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");
            Context.TestEntities.Where(entity => entity.Description.StartsWith("q")).EntitiesChanged += (entities) => count++;

            var entity2 = new TestEntity2 { Key = 1, TestEntities = new List<TestEntity> { entity1 } };
            Context.TestEntities2.Add(entity2);
            Context.SaveChanges();

            Assert.IsTrue(count == 1);
        }


        [TestMethod]
        public void EntitiesChanged_ShouldNotFireWhenARelationshipIsChangedThatDoesNotMeetTheConditions()
        {
            var count = 0;

            var entity1 = new TestEntity { Key = 2, Description = "z" };
            Context.TestEntities.Add(entity1);
            Context.SaveChanges();

            Context = new ChangePublishingTestContext(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");
            Context.TestEntities.Where(entity => entity.Description.StartsWith("q")).EntitiesChanged += (entities) => count++;

            var entity2 = new TestEntity2 { Key = 1, TestEntities = new List<TestEntity> { entity1 } };
            Context.TestEntities2.Add(entity2);
            Context.SaveChanges();

            Assert.IsTrue(count == 0);
        }
    }
}
