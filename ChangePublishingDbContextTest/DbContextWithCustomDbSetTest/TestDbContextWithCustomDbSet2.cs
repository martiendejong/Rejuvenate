using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContextTest
{
    public class TestDbContextWithCustomDbSet2 : TestDbContextWithCustomDbSet2Parent
    {
        public IDbSet<TestEntity> TestEntities { get; set; }

        public TestDbContextWithCustomDbSet2(string connString) : base(connString) { }
    }
}
