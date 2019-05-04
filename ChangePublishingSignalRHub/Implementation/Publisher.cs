using ChangePublishingDbContext;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IHubContextFactory
    {
        IHubContext HubContext { get; }
    }

    public class HubContextFactory<HubType> : IHubContextFactory where HubType : IHub
    {
        public IHubContext HubContext => GlobalHost.ConnectionManager.GetHubContext<HubType>();
    }

    public class Publisher<EntityType, HubType> : IPublisher, IPublisher<EntityType, HubType> where EntityType : class, new() where HubType : IHub
    {
        public readonly Guid _guid = Guid.NewGuid();

        public Expression<Func<EntityType, bool>> Condition { get; set; }

        public Guid Id => _guid;

        public readonly List<string> _clientIds = new List<string>();

        public List<string> ClientIds => _clientIds;

        public IHubContextFactory HubContextFactory = new HubContextFactory<HubType>();

        public IHubContext HubContext => HubContextFactory.HubContext;

        public void Publish(IEnumerable<EntityChange<EntityType>> changes)
        {
            var added = changes.Where(change => change.State == EntityState.Added);
            if (added.Any())
            {
                ClientIds.ForEach(clientId => HubContext.Clients.Client(clientId).itemsAdded(added.ToList()));
            }

            var changed = changes.Where(change => change.State == EntityState.Modified);
            if (changed.Any())
            {
                ClientIds.ForEach(clientId => HubContext.Clients.Client(clientId).itemsChanged(changed));
            }

            var removed = changes.Where(change => change.State == EntityState.Deleted);
            if (removed.Any())
            {
                ClientIds.ForEach(clientId => HubContext.Clients.Client(clientId).itemsRemoved(removed));
            }
        }
    }
}
