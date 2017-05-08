using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;



namespace Rejuvenate.v2
{
    /*
                // todo DI gebruiken 
                // send data to frontend
                DbContext.Items.Add(new Item() { Name = "poepie" });
                DbContext.SaveChanges();
                //context.Items...
                //Clients.Caller.updateData(context.Items);
                */
    
    public interface IChangePublishingHubV2
    {

    }

    // This is where commands from the webclient (player ui) are executed
    public class ChangePublishingHubV2 : Hub
    {
        public static List<ISignalRSubscriber> Subscribers = new List<ISignalRSubscriber>();

        public void Subscribe(List<string> channelIds)
        {
            var client = new SignalRSubscriber(Context.ConnectionId);
            client.ChannelIds.AddRange(channelIds.Select(id => Guid.Parse(id)));
            Subscribers.Add(client);
        }
    }

    public interface ISignalRHubPublisher<HubType> where HubType : IHub
    {
        void Receive<EntityType>(IEnumerable<EntityChangedMessage<EntityType>> messages, ChangePublishingChannel<EntityType> channel);
    }

    public class SignalRHubPublisher<HubType> : ISignalRHubPublisher<HubType> where HubType : IHub
    {
        public void Receive<EntityType>(IEnumerable<EntityChangedMessage<EntityType>> messages, ChangePublishingChannel<EntityType> channel)
        {
            var clients = ChangePublishingHubV2.Subscribers.Where(client => client.ChannelIds.Contains(channel.Guid));
            if (clients.Any())
                Broadcast(messages, channel, clients);
        }

        private void Broadcast<EntityType>(IEnumerable<EntityChangedMessage<EntityType>> messages, ChangePublishingChannel<EntityType> channel, IEnumerable<ISignalRSubscriber> clients)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<HubType>();

            var added = messages.Where(m => m.EntityState == EntityState.Added).Select(m => m.NewEntity);
            var deleted = messages.Where(m => m.EntityState == EntityState.Deleted).Select(m => m.OldEntity);
            var modified = messages.Where(m => m.EntityState == EntityState.Modified).Select(m => m.NewEntity);

            if (added.Count() > 0 || deleted.Count() > 0 || modified.Count() > 0)
            {
                foreach (var client in clients)
                {
                    if (added.Any())
                        context.Clients.Client(client.ConnectionId).itemsAdded(typeof(EntityType).ToString(), channel.Guid.ToString(), added);
                    if (deleted.Any())
                        context.Clients.Client(client.ConnectionId).itemsRemoved(typeof(EntityType).ToString(), channel.Guid.ToString(), deleted);
                    if (modified.Any())
                        context.Clients.Client(client.ConnectionId).itemsUpdated(typeof(EntityType).ToString(), channel.Guid.ToString(), modified);
                }
            }
        }
    }




    public interface ISignalRClient
    {
        string ConnectionId { get; }
    }

    public class SignalRClient : ISignalRClient
    {
        public string ConnectionId { get; }

        public SignalRClient(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }

    public interface ISignalRSubscriber : ISignalRClient
    {
        List<Guid> ChannelIds { get; set; }
    }

    public class SignalRSubscriber : SignalRClient, ISignalRSubscriber
    {
        public List<Guid> ChannelIds { get; set; }

        public SignalRSubscriber(string connectionId) : base(connectionId)
        {
            ChannelIds = new List<Guid>();
        }
    }
}