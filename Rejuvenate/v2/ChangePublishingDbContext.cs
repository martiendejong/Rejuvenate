using EFExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using Rejuvenate.Db.Helpers;
using System.Collections.ObjectModel;

namespace Rejuvenate.v2
{
    public delegate void EntitiesChangedChannelHandler<EntityType>(IEnumerable<EntityChangedMessage<EntityType>> entities, ChangePublishingChannel<EntityType> channel);

    public class ChangePublishingChannel<EntityType>
    {
        public ChangePublishingChannel(Expression<Func<EntityType, bool>> expression, EntitiesChangedChannelHandler<EntityType> handler, Guid guid)
        {
            Expression = expression;
            Handler = handler;
            Guid = guid;
        }

        public Guid Guid;

        public Expression<Func<EntityType, bool>> Expression;

        public EntitiesChangedChannelHandler<EntityType> Handler;

        public void Receive(IEnumerable<EntityChangedMessage<EntityType>> messages)
        {
            var expressionDlg = Expression.Compile();

            var added = messages.Where(message => message.EntityState == EntityState.Added && expressionDlg(message.NewEntity));
            var added2 = messages.Where(message => message.EntityState == EntityState.Modified && expressionDlg(message.NewEntity) && !expressionDlg(message.OldEntity));
            var deleted = messages.Where(message => message.EntityState == EntityState.Deleted && expressionDlg(message.OldEntity));
            var deleted2 = messages.Where(message => message.EntityState == EntityState.Deleted && !expressionDlg(message.NewEntity) && expressionDlg(message.OldEntity));
            var modified = messages.Where(message => message.EntityState == EntityState.Modified && expressionDlg(message.NewEntity) && expressionDlg(message.OldEntity));

            var all = added.Concat(added2).Concat(deleted).Concat(deleted2).Concat(modified);
            if(all.Count() > 0)
                Handler(all, this);
        }
    }

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

    public class ChangePublishingQueryable<EntityType> : IQueryable<EntityType> where EntityType : class, new()
    {
        #region Constructors

        public ChangePublishingQueryable(IQueryable<EntityType> originalQueryable, ChangePublishingDbContext dbContext)
        {
            InnerQueryable = originalQueryable;
            DbContext = dbContext;
        }

        public ChangePublishingQueryable(IQueryable<EntityType> originalQueryable, ChangePublishingDbContext dbContext, Expression<Func<EntityType, bool>> expression) : this(originalQueryable, dbContext)
        {
            InnerExpression = expression;
        }

        #endregion

        protected Expression<Func<EntityType, bool>> And(Expression<Func<EntityType, bool>> expression)
        {
            return InnerExpression == null ? expression : InnerExpression.And(expression);
        }

        public IQueryable<EntityType> InnerQueryable;

        public Expression<Func<EntityType, bool>> InnerExpression;

        public ChangePublishingDbContext DbContext;


        public ChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression)
        {
            return new ChangePublishingQueryable<EntityType>(InnerQueryable.Where(expression), DbContext, And(expression));
        }

        public static List<ChangePublishingChannel<EntityType>> Channels = new List<ChangePublishingChannel<EntityType>>();

        public static ChangePublishingChannel<EntityType> GetChannel(Expression<Func<EntityType, bool>> expression, EntitiesChangedChannelHandler<EntityType> handler)
        {
            foreach(var exprHandler in Channels)
            {
                if(LambdaCompare.Eq(exprHandler.Expression, expression) && exprHandler.Handler == handler)
                {
                    return exprHandler;
                }
            }
            return null;
        }

        public ChangePublishingChannel<EntityType> Subscribe(EntitiesChangedChannelHandler<EntityType> handler)
        {
            var channel = GetChannel(InnerExpression, handler);
            if(channel == null)
            {
                Guid guid;
                do
                {
                    guid = Guid.NewGuid();
                }
                while (Channels.Any(c => c.Guid == guid));

                channel = new ChangePublishingChannel<EntityType>(InnerExpression, handler, guid);
                Channels.Add(channel);
                DbContext.ChangePublisher.Subscribe<EntityType>(channel.Receive);
            }
            return channel;
        }

        #region Implement IQueryable

        public Type ElementType
        {
            get
            {
                return InnerQueryable.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return InnerQueryable.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return InnerQueryable.Provider;
            }
        }

        public IEnumerator<EntityType> GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }

        #endregion
    }

    public class ChangePublishingDbContext : ChangePublishingDbContextBase
    {
        // Convenience method for declaring a Queryable
        public ChangePublishingDbSet<EntityType> Set<EntityType>() where EntityType : class, new()
        {
            return new ChangePublishingDbSet<EntityType>(base.Set<EntityType>(), this);
        }

        #region Inherit constructors from the base class

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext() : base() { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(DbCompiledModel model) : base(model) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(string nameOrconnectionString) : base(nameOrconnectionString) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(string nameOrConnectiongString, DbCompiledModel model) : base(nameOrConnectiongString, model) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(ObjectContext objectContext, bool contextOwnsObjectContext) : base(objectContext, contextOwnsObjectContext) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }

        #endregion
    }
}
