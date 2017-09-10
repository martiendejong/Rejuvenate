using Rejuvenate.Db;
using Rejuvenate.v2;
using System.Collections.Generic;
using System.Data.Entity;

namespace RejuvenatingTests.TestClasses
{
    public interface ITestContext : Rejuvenate.Db.IDbContext
    {


        ChangePublishingDbSet<TestEntity> ChangePublishingEntities { get; }

        ChangePublishingDbSet<TestEntity2> ChangePublishingEntities2 { get; }

    }

    public class TestContext : ChangePublishingDbContext, ITestContext
    {
        #region Regular DbContext

        public TestContext(string connString) : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<TestEntity> Entities { get; set; }

        public virtual DbSet<TestEntity2> Entities2 { get; set; }

        #endregion

        #region RejuvenatingDbContext

        public ChangePublishingDbSet<TestEntity> ChangePublishingEntities
        {
            get
            {
                return Set<TestEntity>();
            }
        }

        public ChangePublishingDbSet<TestEntity2> ChangePublishingEntities2
        {
            get
            {
                return Set<TestEntity2>();
            }
        }
        
        #endregion
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>()
                        .HasOptional(e => e.TestEntity2)
                        .WithMany(g => g.TestEntities);
        }
    }
}