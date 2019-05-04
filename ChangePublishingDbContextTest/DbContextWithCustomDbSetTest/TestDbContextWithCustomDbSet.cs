using ChangePublishingDbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContextTest
{
    public class TestDbContextWithCustomDbSet : DbContextWithCustomDbSet
    {
        public IDbSet<TestEntity> TestEntities { get; set; }

        protected override string _derivedSetName => "ICustomDbSet";

        public override IDbSet<EntityType> GetDerivedSet<EntityType>(DbSet<EntityType> dbSet)
        {
            return new TestDbSet<EntityType>(dbSet);
        }

        public TestDbContextWithCustomDbSet(string connString) : base(connString) { }
    }
}
