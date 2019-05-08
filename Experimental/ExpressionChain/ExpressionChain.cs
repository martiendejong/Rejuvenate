using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionChain
{
    public interface IChainLinqQueryable<EntityType> : IQueryable<EntityType> where EntityType : class, new()
    {

    }

    public class ChainLinqQueryable<EntityType> : IChainLinqQueryable<EntityType> where EntityType : class, new()
    {
        public ChainLinqQueryable(IChainLinqExpression<EntityType> expression, IQueryable<EntityType> queryable)
        {
            _expression = expression;
            _queryable = queryable;
        }

        public ChainLinqQueryable<SelectEntityType> Select<SelectEntityType>(Expression<Func<EntityType, SelectEntityType>> select) 
            where SelectEntityType : class, new()
        {
            return new ChainLinqQueryable<SelectEntityType>(
                new ChainLinqSelectExpression<EntityType, SelectEntityType>(_expression, select),
                _queryable.Select(select)
            );
        }

        public ChainLinqQueryable<EntityType> Where(Expression<Func<EntityType, bool>> where)
        {
            return new ChainLinqQueryable<EntityType>(
                _expression = new ChainLinqWhereExpression<EntityType>(_expression, where),
                _queryable = _queryable.Where(where)
            );
        }

        protected IQueryable<EntityType> _queryable;

        public IChainLinqExpression<EntityType> _expression;

        public Type ElementType => _queryable.ElementType;

        public Expression Expression => _queryable.Expression;

        public IQueryProvider Provider => _queryable.Provider;

        public IEnumerator<EntityType> GetEnumerator() => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queryable.GetEnumerator();
    }

    public interface IChainLinqExpression
    {
        IChainLinqExpression Parent { get; }
    }

    public interface IChainLinqExpression<ToEntityType> : IChainLinqExpression where ToEntityType : class, new()
    {
        
    }

    public class ChainLinqExpression : IChainLinqExpression
    {
        protected IChainLinqExpression _parent;

        public IChainLinqExpression Parent => _parent;

        public ChainLinqExpression(IChainLinqExpression parent)
        {
            _parent = parent;
        }
    }

    public class ChainLinqWhereExpression<EntityType> : ChainLinqExpression, IChainLinqExpression<EntityType> where EntityType : class, new()
    {
        public ChainLinqWhereExpression(IChainLinqExpression<EntityType> parent, Expression<Func<EntityType, bool>> where) : base(parent)
        {

        }
    }

    public class ChainLinqSelectExpression<EntityType, SelectEntityType> : ChainLinqExpression, IChainLinqExpression<SelectEntityType> where EntityType : class, new() where SelectEntityType : class, new()
    {
        public ChainLinqSelectExpression(IChainLinqExpression<EntityType> parent, Expression<Func<EntityType, SelectEntityType>> select) : base(parent)
        {

        }
    }









        /*public interface IChainLinqInputProcessor<InputEntityType>
        {
            IChainLinqOutputter<InputEntityType> Input { get; }
        }

        public interface IChainLinqOutputter<OutputEntityType>
        {
            //IChainLinqInputProcessor<OutputEntityType> Output { get; }
        }

        public interface IChainLinqExpression<InputEntityType, OutputEntityType> 
            : IChainLinqInputProcessor<InputEntityType>, IChainLinqOutputter<OutputEntityType>
            where InputEntityType : class, new() 
            where OutputEntityType : class, new()
        {

        }

        public interface IChainLinqWhereExpression<EntityType> : IChainLinqExpression<EntityType, EntityType> 
            where EntityType : class, new()
        {
            Expression<Func<EntityType, bool>> Where { get; }
        }

        public class ChainLinqWhereExpression<EntityType> : IChainLinqWhereExpression<EntityType>
            where EntityType : class, new()
        {
            public ChainLinqWhereExpression(IChainLinqOutputter<EntityType> parent, Expression<Func<EntityType, bool>> where)
            {
                _parent = parent;
                _where = where;
            }

            protected Expression<Func<EntityType, bool>> _where;

            public Expression<Func<EntityType, bool>> Where => _where;

            protected IChainLinqOutputter<EntityType> _parent;

            public IChainLinqOutputter<EntityType> Input => _parent;

            protected IChainLinqInputProcessor<EntityType> _child;

            //public IChainLinqInputProcessor<EntityType> Output => throw new NotImplementedException();
        }

        public interface IChainLinqSelectExpression<EntityType, SelectEntityType> : IChainLinqExpression<EntityType, SelectEntityType> 
            where EntityType : class, new()
            where SelectEntityType : class, new()
        {
            Expression<Func<EntityType, SelectEntityType>> Select { get; }
        }

        public interface IChainLinqQueryable<EntityType> : IChainLinqOutputter<EntityType>
            where EntityType : class, new()
        {
            IChainLinqOutputter<EntityType> Parent { get; }

            IChainLinqQueryable<EntityType> Where(Expression<Func<EntityType, bool>> where);

            IChainLinqQueryable<SelectEntityType> Select<SelectEntityType>(Expression<Func<EntityType, SelectEntityType>> where) 
                where SelectEntityType : class, new();
        }

        public static class IChainLinqOutputterExtensions
        {
            public static IChainLinqOutputter<EntityType> 
                Where<EntityType>
                (
                    this 
                    IChainLinqOutputter<EntityType> outputter, 
                    Expression<Func<EntityType, bool>> where
                ) 
                where EntityType : class, new()
            {
                return new ChainLinqWhereExpression<EntityType>(outputter, where);
            }
        }*/


        /*public class ChainLinqQueryable<EntityType> : IChainLinqQueryable<EntityType>
            where EntityType : class, new()
        {
            protected IChainLinqOutputter<EntityType> _parent;

            public IChainLinqOutputter<EntityType> Parent => _parent;

            public IChainLinqQueryable<SelectEntityType> Select<SelectEntityType>(Expression<Func<EntityType, SelectEntityType>> where) where SelectEntityType : class, new()
            {
                return null;
            }

            public IChainLinqOutputter<EntityType> Where(Expression<Func<EntityType, bool>> where)
            {
                //return null;
                return new ChainLinqWhereExpression<EntityType>(Parent, where);
            }
        }*/

        /*public interface IChainComponent
        {
            IChainComponent Parent { get; }
        }

        public interface IExpressionChainComponent<EntityType, ExpressionReturnType>

        public abstract class AExpressionChainComponent : IChainComponent
        {
            protected IChainComponent _parent;

            public IChainComponent Parent => _parent;

            public AExpressionChainComponent(IChainComponent parent)
            {
                _parent = parent;
            }
        }

        public class ExpressionChainWhereComponent<EntityType> : AExpressionChainComponent
        {
            public ExpressionChainWhereComponent(IChainComponent parent, Expression<Func<EntityType, bool>> where) : base(parent)
            {

            }
        }

        public class ExpressionChainQueryable<EntityType> where EntityType : class, new()
        {
            protected IQueryable<EntityType> _queryable;

            protected IChainComponent _component;

            public ExpressionChainQueryable(IQueryable<EntityType> queryable, IChainComponent component)
            {
                _queryable = queryable;
                _component = component;
            }

            public ExpressionChainQueryable<EntityType> Where(Expression<Func<EntityType, bool>> where)
            {
                return new ExpressionChainQueryable<EntityType>(_queryable.Where(where), new );
            }
        }*/
}
