using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2
{
    public class ChangePublishingDbSet<EntityType> : ChangePublishingQueryable<EntityType>, IDbSet<EntityType>, IQueryable<EntityType>, IEnumerable<EntityType>, IQueryable, IEnumerable where EntityType : class, new()
    {
        public DbSet<EntityType> DbSet;

        public ChangePublishingDbSet(DbSet<EntityType> dbSet, ChangePublishingDbContext dbContext) : base(dbSet, dbContext)
        {
            DbSet = dbSet;
            DbContext = dbContext;
        }

        #region Implement IDbSet

        public ObservableCollection<EntityType> Local
        {
            get
            {
                return DbSet.Local;
            }
        }

        public EntityType Add(EntityType entity)
        {
            return DbSet.Add(entity);
        }

        public EntityType Attach(EntityType entity)
        {
            return DbSet.Attach(entity);
        }

        public EntityType Create()
        {
            return DbSet.Create();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, EntityType
        {
            return DbSet.Create<TDerivedEntity>();
        }

        public EntityType Find(params object[] keyValues)
        {
            return DbSet.Find(keyValues);
        }

        public EntityType Remove(EntityType entity)
        {
            return DbSet.Remove(entity);
        }

        #endregion
    }

}
