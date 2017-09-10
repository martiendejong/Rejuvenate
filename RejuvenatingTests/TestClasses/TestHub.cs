using Rejuvenate;
using Rejuvenate.v2.SignalRChangePublishing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RejuvenatingTests.TestClasses
{
    public class TestHub : ChangePublishingHubV2
    {
        public static ITestContext DbContext;

        public static Dictionary<string, string> UserNameByConnectionId = new Dictionary<string, string>();
        

        public void setUser(string name)
        {
            UserNameByConnectionId[Context.ConnectionId] = name;
        }


        // first example functions
        public void update()
        {
            /*var item = DbContext.Items.First();
            item.Name = item.Name + "+";
            DbContext.SaveChanges();*/
        }

        public void add()
        {
            /*DbContext.Items.Add(new Item { Name = "asdas" });
            DbContext.SaveChanges();*/
        }

        public void remove()
        {
            /*var item = DbContext.Items.First();
            DbContext.Items.Remove(item);
            DbContext.SaveChanges();*/
        }
    }
}