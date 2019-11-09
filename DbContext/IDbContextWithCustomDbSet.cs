using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IDbContextWithCustomDbSet
    {
        IDbSet<Entity> GetCustomDbSet<Entity>(DbSet<Entity> dbSet) where Entity : class, new();
    }
}
