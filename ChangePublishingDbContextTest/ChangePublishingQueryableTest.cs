using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChangePublishingDbContext;
using System.Collections;
using System.Collections.Generic;
using ChangePublishingDbContext.Implementation;
using System.Data.Entity;
using System.Linq;

namespace ChangePublishingDbContextTest
{
    [TestClass]
    public class ChangePublishingQueryableTest
    {
        //public ITestContext Context = new TestContextWithSaveEvent(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
        public ITestContextWithSaveEvent Context = new TestContextWithSaveEvent (@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");

        public IEntityChangeTracker _changeTracker;
        public IEntityChangeTracker ChangeTracker => _changeTracker == null ? _changeTracker = new EntityChangeTracker(Context) : _changeTracker;

        public IConditionalChangeTrackerFactory<TestEntity> _conditionalChangeTrackerManager;
        public IConditionalChangeTrackerFactory<TestEntity> ConditionalChangeTrackerManager => _conditionalChangeTrackerManager == null ? _conditionalChangeTrackerManager = new ConditionalChangeTrackerFactory<TestEntity>(ChangeTracker.Entity<TestEntity>()) : _conditionalChangeTrackerManager;

        [TestMethod]
        public void EntitiesChanged_ShouldFireWhenAnEntityIsAddedThatMeetsTheConditions()
        {
            var count = 0;

            var q = new ChangePublishingQueryable<TestEntity>(Context as DbContext, Context.TestEntities, ConditionalChangeTrackerManager, null, (x) => true);
            q.Where(entity => entity.Description.StartsWith("b")).EntitiesChanged += (entities) => count++;

            Context.TestEntities.Add(new TestEntity { Key = 1, Description = "b" });
            Context.SaveChanges();

            Assert.IsTrue(count == 1);
        }

        [TestMethod]
        public void EntitiesChanged_ShouldNotFireWhenAnEntityTypeIsAddedThatDoesNotMeetTheConditions()
        {
            var count = 0;

            var q = new ChangePublishingQueryable<TestEntity>(Context as DbContext, Context.TestEntities, ConditionalChangeTrackerManager, null, (x) => true);
            q.Where(entity => entity.Description.StartsWith("q")).EntitiesChanged += (entities) => count++;

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

            Context = new TestContextWithSaveEvent (@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");
            _changeTracker = new EntityChangeTracker(Context);
            _conditionalChangeTrackerManager = new ConditionalChangeTrackerFactory<TestEntity>(ChangeTracker.Entity<TestEntity>());
            var q = new ChangePublishingQueryable<TestEntity>(Context as DbContext, Context.TestEntities, ConditionalChangeTrackerManager, null, (x) => true);
            q.Where(entity => entity.Description.StartsWith("q")).EntitiesChanged += (entities) => count++;

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

            Context = new TestContextWithSaveEvent (@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");
            _changeTracker = new EntityChangeTracker(Context);
            _conditionalChangeTrackerManager = new ConditionalChangeTrackerFactory<TestEntity>(ChangeTracker.Entity<TestEntity>());
            var q = new ChangePublishingQueryable<TestEntity>(Context as DbContext, Context.TestEntities, ConditionalChangeTrackerManager, null, (x) => true);
            q.Where(entity => entity.Description.StartsWith("q")).EntitiesChanged += (entities) => count++;

            var entity2 = new TestEntity2 { Key = 1, TestEntities = new List<TestEntity> { entity1 } };
            Context.TestEntities2.Add(entity2);
            Context.SaveChanges();

            Assert.IsTrue(count == 0);
        }
    }
}
