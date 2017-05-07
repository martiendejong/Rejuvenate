using Rejuvenate.Db;
using System.Collections.Generic;
using System.Data.Entity;

namespace RejuvenatingTests.TestClasses
{
    public interface ITestContext : IChangePublishingDbContext
    {

        DbSet<TestEntity> Entities { get; set; }

        DbSet<TestEntity2> Entities2 { get; set; }

        IChangePublishingQueryable<TestEntity> ChangePublishingEntities { get; }

        IChangePublishingQueryable<TestEntity2> ChangePublishingEntities2 { get; }

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

        // declare the change aware db query
        public IChangePublishingQueryable<TestEntity> ChangePublishingEntities
        {
            get
            {
                return new ChangePublishingQueryable<TestEntity>(Entities, this);
            }
        }

        public IChangePublishingQueryable<TestEntity2> ChangePublishingEntities2
        {
            get
            {
                return new ChangePublishingQueryable<TestEntity2>(Entities2, this);
            }
        }

        // declare the polling executors for the change aware entities
        override protected List<IChangeProcessor> ChangeProcessors
        {
            get
            {
                return new List<IChangeProcessor>
                {
                    GetChangeProcessor<TestEntity>(),
                    GetChangeProcessor<TestEntity2>()
                };
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