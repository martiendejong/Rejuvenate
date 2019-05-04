using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChangePublishingDbContext.Implementation;
using ChangePublishingDbContext;

namespace ChangePublishingDbContextTest
{
    [TestClass]
    public class IDbContextWithSaveEventTest
    {
        public IDbContextWithSaveEvent Context = new DbContextWithSaveEvent(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");

        [TestMethod]
        public void SaveStart_ShouldFireWhenSaveChangesIsCalled()
        {
            var fired = false;

            Context.SaveStart += (context) => { fired = true; };
            Context.SaveChanges();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void SaveCompleted_ShouldFireWhenSaveChangesIsCalled()
        {
            var fired = false;

            Context.SaveCompleted += (context) => { fired = true; };
            Context.SaveChanges();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void SaveStart_ShouldFireBeforeSaveCompletedWhenSaveChangesIsCalled()
        {
            var i = 1;
            var startRank = 0;
            var completedRank = 0;

            Context.SaveStart += (context) => { startRank = ++i; };
            Context.SaveCompleted += (context) => { completedRank = ++i; };
            Context.SaveChanges();

            Assert.IsTrue(startRank < completedRank);
        }

        [TestMethod]
        public void SaveStart_ShouldFireWhenSaveChangesAsyncIsCalled()
        {
            var fired = false;

            Context.SaveStart += (context) => { fired = true; };
            Context.SaveChangesAsync().GetAwaiter().OnCompleted(() =>
            {
                Assert.IsTrue(fired);
            });
        }

        [TestMethod]
        public void SaveCompleted_ShouldFireWhenSaveChangesAsyncIsCalled()
        {
            var fired = false;

            Context.SaveCompleted += (context) => { fired = true; };
            Context.SaveChangesAsync().GetAwaiter().OnCompleted(() =>
            {
                Assert.IsTrue(fired);
            });
        }

        [TestMethod]
        public void SaveStart_ShouldFireBeforeSaveCompletedWhenSaveChangesAsyncIsCalled()
        {
            var i = 1;
            var startRank = 0;
            var completedRank = 0;

            Context.SaveStart += (context) => { startRank = ++i; };
            Context.SaveCompleted += (context) => { completedRank = ++i; };
            Context.SaveChangesAsync().GetAwaiter().OnCompleted(() =>
            {
                Assert.IsTrue(startRank < completedRank);
            });
        }
    }
}
