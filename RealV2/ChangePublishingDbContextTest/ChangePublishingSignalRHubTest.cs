using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChangePublishingDbContext;
using System.Collections;
using System.Collections.Generic;
using ChangePublishingDbContext.Implementation;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Moq;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity;
using System.Dynamic;

namespace ChangePublishingDbContextTest
{
    [TestClass]
    public class ChangePublishingSignalRHubTest
    {
        public ChangePublishingSignalRHub Hub = new ChangePublishingSignalRHub();

        [TestMethod]
        public void ClientShouldGetMessages()
        {
            var called = false;

            var mockFactory = new Mock<IHubContextFactory>();
            var mockHubContext = new Mock<IHubContext>();
            var mockClients = new Mock<IHubConnectionContext<dynamic>>();
            dynamic client = new ExpandoObject();
            client.itemsAdded = new Action<IEnumerable<EntityChange<TestEntity>>>((text) => {
                called = true;
            });
            mockFactory.Setup(fa => fa.HubContext).Returns(mockHubContext.Object);
            mockHubContext.Setup(hh => hh.Clients).Returns(mockClients.Object);
            mockClients.Setup(m => m.Client("1")).Returns((ExpandoObject)client);

            var mockRequest = new Mock<IRequest>();
            Hub.Context = new HubCallerContext(mockRequest.Object, "1");

            var publisher = new Publisher<TestEntity, ChangePublishingSignalRHub>();
            publisher.HubContextFactory = mockFactory.Object;
            ChangePublishingSignalRHub.Publishers.Add(publisher.Id, publisher);
            Hub.Subscribe(publisher.Id.ToString());
            publisher.Publish(new List<EntityChange<TestEntity>>(){ new EntityChange<TestEntity>(EntityState.Added, null, null)});

            Assert.IsTrue(called);
        }

        public IChangePublishingTestContext Context = new ChangePublishingTestContext(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");

        [TestMethod]
        public void EntitiesChanged_ShouldFireWhenAnEntityIsAddedThatMeetsTheConditions()
        {
            var count = 0;

            var publisher = new Publisher<TestEntity, ChangePublishingSignalRHub>();
            ChangePublishingSignalRHub.Publishers.Add(publisher.Id, publisher);

            Context.TestEntities.Where(entity => entity.Description.StartsWith("b")).EntitiesChanged += publisher.Publish;

            Context.TestEntities.Add(new TestEntity { Key = 1, Description = "b" });
            Context.SaveChanges();

            Assert.IsTrue(count == 1);
        }
    }
}
