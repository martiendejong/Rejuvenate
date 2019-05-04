using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RejuvenatingTests.TestClasses;
using Rejuvenate.Db;
using Rejuvenate.v2;
using System.Linq;

namespace RejuvenatingTests
{
    [TestClass]
    public class UnitTest1
    {
        public TestClasses.TestContext context = new TestClasses.TestContext(@"Server =.\SQLEXPRESS64; Database = RejuvenatingTests; Integrated Security = True;");
        //var context = new TestClasses.TestContext(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");


        [TestMethod]
        public void EntitiesChangedHandlerShouldFireOnceWhenOneEntityWithMultipleSubscribersChanges()
        {
            var count = 0;
            EntitiesChangedHandler<TestEntity> handler = (messages, listener) => 
            {
                ++count;
            };
            context.ChangePublishingEntities.Subscribe(handler);
            context.ChangePublishingEntities.Subscribe(handler);

            var entity = new TestEntity() { Description = "as" };
            context.Entities.Add(entity);
            context.SaveChanges();

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void EntitiesChangedHandlerShouldFireOnceWhenALinkedEntityWithMultipleSubscribersChanges()
        {
            var entity = new TestEntity() { Description = "as" };
            var entity2 = new TestEntity2();
            entity.TestEntity2 = entity2;

            context.Entities.Add(entity);
            context.Entities2.Add(entity2);
            context.SaveChanges();


            var context2 = new TestClasses.TestContext(@"Server =.\SQLEXPRESS64; Database = RejuvenatingTests; Integrated Security = True;");
            var count = 0;
            EntitiesChangedHandler<TestEntity2> handler = (messages, listener) =>
            {
                ++count;
            };
            var listener2 = context2.ChangePublishingEntities2.Subscribe(handler);


            context2.ChangePublishingEntities2.SubscribeChildEntity<TestEntity>
            (
                listener2,
                e => e.TestEntity2
            );
            context2.ChangePublishingEntities2.SubscribeChildEntity<TestEntity>
            (
                listener2,
                e => e.TestEntity2
            );

            entity = context2.Entities.First(ent => ent.Description == "as");
            entity.Description = "hoi";
            context2.SaveChanges();


            Assert.AreEqual(1, count);

            context.Entities.RemoveRange(context.Entities);
            context.Entities2.RemoveRange(context.Entities2);
        }


        [TestMethod]
        public void EntitiesChangedHandlerShouldFireTwiceWhenALinkedEntityWithMultipleSubscribersChangesRelation()
        {
            // create an entity that is linked to another entity and save it to db
            var entity = new TestEntity() { Description = "as" };
            var entity2 = new TestEntity2();

            entity.TestEntity2 = entity2;

            context.Entities.Add(entity);
            context.Entities2.Add(entity2);
            context.SaveChanges();


            // create a new entity and save it to db context 2
            var context2 = new TestClasses.TestContext(@"Server =.\SQLEXPRESS64; Database = RejuvenatingTests; Integrated Security = True;");
            var entity3 = new TestEntity2();
            context2.Entities2.Add(entity3);
            context2.SaveChanges();


            // create the listener
            var count = 0;
            EntitiesChangedHandler<TestEntity2> handler = (messages, listener) =>
            {
                ++count;
            };
            var listener2 = context2.ChangePublishingEntities2.Subscribe(handler);
            
            // subscribe child entity TestEntity to the listener
            context2.ChangePublishingEntities2.SubscribeChildEntity<TestEntity>
            (
                listener2,
                e => e.TestEntity2
            );

            // find the first entity and link it to the unlinked entity
            entity = context2.Entities.First(ent => ent.Description == "as");
            entity.TestEntity2 = entity3;

            // save changes. now the subscription listener should fire
            context2.SaveChanges();


            Assert.AreEqual(1, count);

            context.Entities.RemoveRange(context.Entities);
            context.Entities2.RemoveRange(context.Entities2);
        }
    }
}
