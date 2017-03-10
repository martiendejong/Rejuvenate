using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace poller
{
    public class EntityWithNumericalKey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    public class Item : EntityWithNumericalKey
    {
        public string Name { get; set; }
    }

    public delegate void Publish<T>(Type type, int pollerId, EntityState state, IEnumerable<T> entities) where T : class;

    public class Poller<T> where T : class
    {
        public int Id;

        public Expression<Func<T, bool>> Expression;

        public Publish<T> Publish;
    }

    public class ParameterRebinder : ExpressionVisitor
    {

        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }

    public static class ExpressionUtility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }

    public class ChangeAwareQueriable<T> where T : class
    {
        public IQueryable<T> Queryable;

        public PollerContext DbContext;

        public Expression<Func<T, bool>> Expression;

        public ChangeAwareQueriable(IQueryable<T> queryable)
        {
            Queryable = queryable;
        }

        public ChangeAwareQueriable<T> Where(Expression<Func<T, bool>> expression)
        {
            return new ChangeAwareQueriable<T>(Queryable.Where(expression)) { DbContext = DbContext, Expression = Expression == null ? expression : Expression.And(expression) };
        }

        public IQueryable<T> AddPollingFunction(Publish<T> publish)
        {
            // todo figure this out
            //Expression<Func<T, bool>> x = (Expression<Func<T, bool>>)Queryable.Expression;
            Poller<T> poller = new Poller<T>();
            // todo auto increment
            poller.Id = 1;
            poller.Expression = Expression;
            poller.Publish = publish;
            // todo add to dbcontext
            DbContext.AddPoll(poller);
            return Queryable;
        }
    }

    public class PollerContext : DbContext
    {
        public PollerContext() : base("name=DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public override Task<int> SaveChangesAsync()
        {
            SaveChangesStart();
            Task<int> task = base.SaveChangesAsync();
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(SaveChangesCompleted);
            return task;
        }

        public override int SaveChanges()
        {
            SaveChangesStart();
            var res = base.SaveChanges();
            SaveChangesCompleted();
            return res;
        }

        virtual protected void SaveChangesStart()
        {
        }

        virtual protected void SaveChangesCompleted()
        {
        }

        protected void SetupPollers<T>(IEnumerable<DbEntityEntry<T>> entries) where T : class
        {
            foreach (var untypedPoller in Pollers[typeof(T)])
            {
                var poller = (Poller<T>)untypedPoller.Value;
                var pollingStates = new[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };
                var entities = entries.Where(e => pollingStates.Contains(e.State)).Select(i => i.Entity).OfType<T>().AsQueryable();

                var polledEntities = entities.Where(poller.Expression);
                var polledEntries = entries.Where(e => polledEntities.Contains(e.Entity));

                foreach (var entry in polledEntries)
                {
                    var state = entry.State;
                    if (!PolledEntries.ContainsKey(untypedPoller))
                        PolledEntries[poller] = new Dictionary<EntityState, List<DbEntityEntry>>();
                    if (!PolledEntries[poller].ContainsKey(state))
                        PolledEntries[poller][state] = new List<DbEntityEntry>();
                    PolledEntries[poller][state].Add(entry);
                }
            }
        }

        public void ExecutePollers<T>() where T : class
        {
            foreach (var pollerPair in PolledEntries)
            {
                var poller = (Poller<T>) pollerPair.Key;
                foreach(var statePair in pollerPair.Value)
                {
                    var state = statePair.Key;
                    poller.Publish(typeof(T), poller.Id, state, statePair.Value.Select(i => (T)i.Entity));
                }
                PolledEntries.Remove(pollerPair);
            }
        }

        protected Dictionary<Type, Dictionary<int, object>> Pollers = new Dictionary<Type, Dictionary<int, object>>();

        protected Dictionary<object, Dictionary<EntityState, List<DbEntityEntry>>> PolledEntries = new Dictionary<object, Dictionary<EntityState, List<DbEntityEntry>>>();

        public void AddPoll<T>(Poller<T> poller) where T : class
        {
            var type = typeof(T);
            if (!Pollers.ContainsKey(type))
                Pollers[type] = new Dictionary<int, object>();
            // todo autoincrement
            Pollers[type].Add(1, poller);
        }
    }
}
