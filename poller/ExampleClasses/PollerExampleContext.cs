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

        // declare the change aware db query
        public ChangeAwareQueriable<Item> ChangeAwareItems
        {
            get
            {
                return new ChangeAwareQueriable<Item>(Items, this);
            }
        }

        // declare the polling executors for the change aware entities
        override protected List<IPollerExecutor> Executors
        {
            get
            {
                return new List<IPollerExecutor>
                {
                    new PollerExecutor<Item>(ChangeTracker, this)
                };
            }
        }
    }
}