using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace poller
{
    public class PollerExampleContext : PollerContext
    {
        public virtual DbSet<Item> Items { get; set; }

        public ChangeAwareQueriable<Item> ChangeAwareItems { get { return new ChangeAwareQueriable<Item>(Items) { DbContext = this }; } }

        override protected void SaveChangesStart()
        {
            ChangeTracker.DetectChanges();
            // todo make T
            var itemEntries = ChangeTracker.Entries<Item>();
            SetupPollers(itemEntries);
        }

        override protected void SaveChangesCompleted()
        {
            // todo make T
            ExecutePollers<Item>();
        }
    }
}