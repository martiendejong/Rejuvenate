
using Rejuvenate;
using Rejuvenate.Implementation;
using System.Collections.Generic;
using System.Data.Entity;

namespace ChangePublishingDbContextTest
{
    public interface ITestContextWithSaveEvent : IDbContextWithSaveEvent
    {
        DbSet<TestEntity> TestEntities { get; }

        DbSet<TestEntity2> TestEntities2 { get; }

    }

    public class TestContextWithSaveEvent : DbContextWithSaveEvent, ITestContextWithSaveEvent
    {
        #region Regular DbContext

        public TestContextWithSaveEvent () : base("name=DefaultConnection") { }

        public TestContextWithSaveEvent (string connString) : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<TestEntity> TestEntities { get; set; }

        public virtual DbSet<TestEntity2> TestEntities2 { get; set; }

        #endregion
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>()
                        .HasOptional(e => e.TestEntity2)
                        .WithMany(g => g.TestEntities);
        }
    }
}