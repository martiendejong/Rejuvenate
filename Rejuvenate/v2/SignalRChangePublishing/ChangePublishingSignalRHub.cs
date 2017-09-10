using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Rejuvenate.v2.EntityChangePublishing;
using Rejuvenate.v2;

namespace Rejuvenate.v2.SignalRChangePublishing
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

    public interface ISignalRHubListener<HubType> where HubType : IHub
    {
        void Receive<EntityType>(IEnumerable<EntityChangeMessage<EntityType>> messages, EntitiesChangedListener<EntityType> channel) where EntityType : class, new();
    }

    public class SignalRHubListener<HubType> : ISignalRHubListener<HubType> where HubType : IHub
    {
        public void Receive<EntityType>(IEnumerable<EntityChangeMessage<EntityType>> messages, EntitiesChangedListener<EntityType> channel) where EntityType : class, new()
        {
            var clients = ChangePublishingHubV2.Subscribers.Where(client => client.ChannelIds.Contains(channel.Id));
            if (clients.Any())
                Broadcast(messages, channel, clients);
        }

        private void Broadcast<EntityType>(IEnumerable<EntityChangeMessage<EntityType>> messages, EntitiesChangedListener<EntityType> channel, IEnumerable<ISignalRSubscriber> clients) where EntityType : class, new()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<HubType>();

            var added = messages.Where(m => m.State == EntityState.Added).Select(m => m.Current);
            var deleted = messages.Where(m => m.State == EntityState.Deleted).Select(m => m.Last);
            var modified = messages.Where(m => m.State == EntityState.Modified).Select(m => m.Current);

            if (added.Count() > 0 || deleted.Count() > 0 || modified.Count() > 0)
            {
                foreach (var client in clients)
                {
                    if (added.Any())
                        context.Clients.Client(client.ConnectionId).itemsAdded(typeof(EntityType).ToString(), channel.Id.ToString(), added);
                    if (deleted.Any())
                        context.Clients.Client(client.ConnectionId).itemsRemoved(typeof(EntityType).ToString(), channel.Id.ToString(), deleted);
                    if (modified.Any())
                        context.Clients.Client(client.ConnectionId).itemsUpdated(typeof(EntityType).ToString(), channel.Id.ToString(), modified);
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