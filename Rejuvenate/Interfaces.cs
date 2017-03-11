using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IClientRejuvenator<T> where T : class
    {
        int Id { get; set; }

        Expression<Func<T, bool>> Expression { get; set; }

        RejuvenateClientCallback<T> Rejuvenate { get; set; }
    }

    public interface IEntityRejuvenator
    {
        void PrepareRejuvenation();

        void Rejuvenate();
    }

    public interface IRejuvenatingQueryable<T> where T : class
    {
        IRejuvenatingQueryable<T> Where(Expression<Func<T, bool>> expression);

        IQueryable<T> RejuvenateQuery(RejuvenateClientCallback<T> publish);
    }

}
