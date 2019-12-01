using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rejuvenate;
using System.Collections;
using System.Collections.Generic;
using Rejuvenate.Implementation;
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
            var clientId = "1";

            dynamic client = new ExpandoObject();
            var mockClients = new Mock<IHubConnectionContext<dynamic>>();
            mockClients.Setup(m => m.Client(clientId)).Returns((ExpandoObject)client);
            var mockHubContext = new Mock<IHubContext>();
            mockHubContext.Setup(hh => hh.Clients).Returns(mockClients.Object);
            var mockFactory = new Mock<IHubContextFactory>();
            mockFactory.Setup(fa => fa.HubContext).Returns(mockHubContext.Object);
            var mockRequest = new Mock<IRequest>();
            Hub.Context = new HubCallerContext(mockRequest.Object, clientId);
            client.itemsAdded = new Action<IEnumerable<EntityChange<TestEntity>>>((changes) => {
                called = true;
            });

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

            //var publisher = new Publisher<TestEntity, ChangePublishingSignalRHub>();
            //ChangePublishingSignalRHub.Publishers.Add(publisher.Id, publisher);
            var publisher = Context.TestEntities.Where(entity => entity.Description.StartsWith("b")).Publisher<ChangePublishingSignalRHub>();
            /*publisher.h
            var guid = publisher.Id;
            Hub.Subscribe(guid.ToString());


            if (Hub.Publishers.ContainsKey(guid))
            {
                var publisher = Hub.Publishers[guid];
                publisher..ClientIds.Add(Context.ConnectionId);
            }*/

            //Context.TestEntities.Where(entity => entity.Description.StartsWith("b")).EntitiesChanged += publisher.Publish;

            Context.TestEntities.Add(new TestEntity { Key = 1, Description = "b" });
            Context.SaveChanges();

            Assert.IsTrue(count == 1);
        }
    }
}
