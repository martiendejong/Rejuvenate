using Rejuvenate;
using RejuvenatingExample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RejuvenatingExample
{
    public class ExampleHub : RejuvenatingHub
    {
        public static IExampleContext DbContext;

        public static Dictionary<string, string> UserNameByConnectionId = new Dictionary<string, string>();

        public void hostGame()
        {
            DbContext.Games.Add(new Game(UserNameByConnectionId[Context.ConnectionId]));
            DbContext.SaveChanges();
        }

        public void setUser(string name)
        {
            UserNameByConnectionId[Context.ConnectionId] = name;
        }


        // first example functions
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