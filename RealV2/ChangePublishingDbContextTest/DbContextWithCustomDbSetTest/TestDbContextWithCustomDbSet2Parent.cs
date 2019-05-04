using ChangePublishingDbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContextTest
{
    public class TestDbContextWithCustomDbSet2Parent : DbContextWithCustomDbSet
    {
        protected override string _derivedSetName => "ICustomDbSet";

        public override IDbSet<EntityType> GetDerivedSet<EntityType>(DbSet<EntityType> dbSet)
        {
            return new TestDbSet<EntityType>(dbSet);
        }

        public TestDbContextWithCustomDbSet2Parent(string connString) : base(connString) { }
    }
}
