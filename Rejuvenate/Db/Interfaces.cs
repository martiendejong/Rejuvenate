using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Db
{
    public interface IDbContext : IDisposable, IObjectContextAdapter
    {
        int SaveChanges();
    }

    public interface IChangePublishingDbContext : IDbContext
    {

    }

    public interface IEntityWithId<IdType>
    {
        IdType Id { get; set; }
    }

    public class EntityWithId<IdType> : IEntityWithId<IdType>
    {
        [Key]
        public IdType Id { get; set; }
    }
}
