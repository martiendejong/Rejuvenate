using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;

namespace ChangePublishingDbContextTest
{
    [TestClass]
    public class DbContextWithCustomDbSetTest
    {
        //public TestDbContextWithCustomDbSet Context = new TestDbContextWithCustomDbSet(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
        public TestDbContextWithCustomDbSet Context = new TestDbContextWithCustomDbSet(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");
        //public TestDbContextWithCustomDbSet Context = new TestDbContextWithCustomDbSet(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
        public TestDbContextWithCustomDbSet2 Context2 = new TestDbContextWithCustomDbSet2(@"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");

        [TestMethod]
        public void DbContextWithCustomDbSetShouldSaveWithoutErrors()
        {
            Context.TestEntities.Add(new TestEntity {Key = 1, Description = "c" });
            Context.SaveChanges();
        }

        [TestMethod]
        public void DbContextWithCustomDbSet2ShouldSaveWithoutErrors()
        {
            Context2.TestEntities.Add(new TestEntity { Key = 1, Description = "c" });
            Context2.SaveChanges();
        }
    }
}
