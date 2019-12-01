
using Rejuvenate;

namespace ChangePublishingDbContextTest
{
    public class ChangePublishingTestContext : Rejuvenate.ChangePublishingDbContext, IChangePublishingTestContext
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