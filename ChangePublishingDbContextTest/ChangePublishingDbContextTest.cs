using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rejuvenate;
using System.Collections;
using System.Collections.Generic;
using Rejuvenate.Implementation;
using System.Linq;

namespace ChangePublishingDbContextTest
{
    [TestClass]
    public class ChangePublishingDbContextTest
    {
        //public ITestContext Context = new TestContextWithSaveEvent(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
        public IChangePublishingTestContext Context = new ChangePublishingTestContext(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");



        public IChangesPublisherFactory _changeTracker;
        public IChangesPublisherFactory ChangeTracker => _changeTracker == null ? _changeTracker = new ChangesPublisherFactory(Context) : _changeTracker;

        public IEntityChangeFilterProcessorFactory<TestEntity> _conditionalChangeTrackerManager;
        public IEntityChangeFilterProcessorFactory<TestEntity> ConditionalChangeTrackerManager => _conditionalChangeTrackerManager == null ? _conditionalChangeTrackerManager = new EntityChangeFilterProcessorFactory<TestEntity>(ChangeTracker.Entity<TestEntity>()) : _conditionalChangeTrackerManager;



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
        public void ValueChanged_ShouldFireWhenAnEntityIsAdded()
        {
            int sumValue = 0;

            IChangePublishingValue<TestEntity, int> v = Context.TestEntities.Sum(t => t.Key);
            v.ValueChanged += v2 => sumValue = v2;
            Context.TestEntities.Add(new TestEntity { Key = 15, Description = "b" });
            Context.SaveChanges();

            Assert.IsTrue(sumValue > 15);
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
