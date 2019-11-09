using ChangePublishingDbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContextTest
{
    public class TestDbContextWithCustomDbSet2Parent : ADbContextWithCustomDbSet
    {
        protected override string _customDbSetClassName => "ICustomDbSet";

        public override IDbSet<EntityType> GetCustomDbSet<EntityType>(DbSet<EntityType> dbSet)
        {
            return new TestDbSet<EntityType>(dbSet);
        }

        public TestDbContextWithCustomDbSet2Parent(string connString) : base(connString) { }
    }
}
