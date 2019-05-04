using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ChangePublishingDbContext;
using System.Data.Entity;

namespace ChangePublishingDbContext
{
    public class ChangePublishingSignalRHub : Hub
    {
        public static Dictionary<Guid, IPublisher> Publishers = new Dictionary<Guid, IPublisher>();

        public void Receive(List<string> publisherIds)
        {
            Subscribe(publisherIds);

            Clients.Client(Context.ConnectionId).hoi();
        }

        public void Subscribe(string publisherId)
        {
            var guid = Guid.Parse(publisherId);
            if (Publishers.ContainsKey(guid))
            {
                var publisher = Publishers[guid];
                publisher.ClientIds.Add(Context.ConnectionId);
            }
        }

        public void Subscribe(List<string> publisherIds)
        {
            publisherIds.ForEach(publisherId => Subscribe(publisherId));
        }

        public void UnSubscribe(string publisherId)
        {
            var guid = Guid.Parse(publisherId);
            if (Publishers.ContainsKey(guid))
            {
                var publisher = Publishers[guid];
                publisher.ClientIds.Remove(Context.ConnectionId);
            }
        }

        public void UnSubscribe(List<string> publisherIds)
        {
            publisherIds.ForEach(publisherId => UnSubscribe(publisherId));
        }
    }
}
