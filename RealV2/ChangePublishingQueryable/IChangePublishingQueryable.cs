using ChangePublishingDbContext;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IChangePublishingQueryable<EntityType> : IQueryable<EntityType> where EntityType : class, new()
    {
        event EntitiesChangedHandler<EntityType> EntitiesChanged;

        Expression<Func<EntityType, bool>> Condition { get; set; }

        IChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression);
    }
}
