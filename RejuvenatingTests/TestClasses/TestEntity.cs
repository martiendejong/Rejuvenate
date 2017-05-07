using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RejuvenatingTests.TestClasses
{
    public class TestEntity
    {
        [Key]
        public int Key { get; set; }

        public TestEntity2 TestEntity2 { get; set; }
    }

    public class TestEntity2
    {
        [Key]
        public int Key { get; set; }

        public virtual ICollection<TestEntity> TestEntities { get; set; }
    }
}
