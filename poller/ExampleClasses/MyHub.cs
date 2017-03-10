using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace poller
{
    public class MyHub : PollerHub
    {
        public static PollerExampleContext DbContext;

        public void update()
        {
            var item = DbContext.Items.First();
            item.Name = item.Name + "+";
            DbContext.SaveChanges();
        }

        public void add()
        {
            DbContext.Items.Add(new Item { Name = "asdas" });
            DbContext.SaveChanges();
        }

        public void remove()
        {
            var item = DbContext.Items.First();
            DbContext.Items.Remove(item);
            DbContext.SaveChanges();
        }
    }
}