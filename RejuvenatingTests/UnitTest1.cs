using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RejuvenatingTests.TestClasses;
using Rejuvenate.Db;
using Rejuvenate.v2;

namespace RejuvenatingTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EntitiesChangedHandlerShouldFireOnceWhenOneEntityWithMultipleSubscribersChanges()
        {
            var context = new TestClasses.TestContext(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            var count = 0;
            EntitiesChangedHandler<TestEntity> handler = (messages, listener) => 
            {
                ++count;
            };
            context.ChangePublishingEntities.Subscribe(handler);
            context.ChangePublishingEntities.Subscribe(handler);

            var entity = new TestEntity();
            context.Entities.Add(entity);
            context.SaveChanges();

            Assert.AreEqual(count, 1);
        }

        [TestMethod]
        public void EntitiesChangedHandlerShouldFireOnceWhenALinkedEntityWithMultipleSubscribersChanges()
        {
            // @todo
            var context = new TestClasses.TestContext(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            var count = 0;
            EntitiesChangedHandler<TestEntity> handler = (messages, listener) =>
            {
                ++count;
            };
            //context.ChangePublishingEntities.SubscribeChildEntity()
            //context.ChangePublishingEntities.SubscribeChildEntity(handler);
            context.ChangePublishingEntities.Subscribe(handler);

            var entity = new TestEntity();
            context.Entities.Add(entity);
            context.SaveChanges();

            Assert.AreEqual(count, 1);
        }
    }
}
