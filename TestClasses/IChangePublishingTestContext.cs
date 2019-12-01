
using Rejuvenate;
using Rejuvenate.Implementation;
using System.Collections.Generic;
using System.Data.Entity;

namespace ChangePublishingDbContextTest
{
    public interface IChangePublishingTestContext : IChangePublishingDbContext
    {
        IChangePublishingDbSet<TestEntity> TestEntities { get; }

        IChangePublishingDbSet<TestEntity2> TestEntities2 { get; }

    }
}