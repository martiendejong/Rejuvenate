
using ChangePublishingDbContext;
using ChangePublishingDbContext.Implementation;
using System.Collections.Generic;
using System.Data.Entity;

namespace ChangePublishingDbContextTest
{
    public interface IChangePublishingTestContext : IChangePublishingDbContext
    {
        IChangePublishingDbSet<TestEntity> TestEntities { get; }

        IChangePublishingDbSet<TestEntity2> TestEntities2 { get; }

    }

    public class ChangePublishingTestContext : ChangePublishingDbContext.ChangePublishingDbContext, IChangePublishingTestContext
    {
        #region constructors

        public ChangePublishingTestContext() : base("name=DefaultConnection") { }

        public ChangePublishingTestContext(string connString) : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        #endregion

        public virtual IChangePublishingDbSet<TestEntity> TestEntities { get; set; }

        public virtual IChangePublishingDbSet<TestEntity2> TestEntities2 { get; set; }
    }
}