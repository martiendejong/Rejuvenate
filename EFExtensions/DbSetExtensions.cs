using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFExtensions
{
    public static class DbSetExtensions
    {
        public static DbContext GetContext<EntityType>(this DbSet<EntityType> dbSet) where EntityType:class,new()
        {
            var fieldInfo = typeof(DbSet).GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance);
            var context = fieldInfo.GetValue(dbSet) as DbContext;
            return context;
        }
    }
}
