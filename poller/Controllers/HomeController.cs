using Microsoft.AspNet.SignalR;
using RejuvenatingExample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RejuvenatingExample.Controllers
{
    public class HomeController : Controller
    {
        public ExampleContext DbContext = new ExampleContext();

        public ActionResult Index()
        {
            // todo make di
            ExampleHub.DbContext = DbContext;
            IEnumerable<Item> query = DbContext.ChangeAwareItems.Where(i => i.Name.Length < 10).RejuvenateQuery(PublishItems);
            return View(query);
        }

        // todo refactor away
        public void PublishItems(Type type, int rejuvenatorId, EntityState state, IEnumerable<Item> entries)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ExampleHub>();
            switch (state)
            {
                case EntityState.Added:
                    context.Clients.All.itemsAdded(type, rejuvenatorId, entries);
                    break;
                case EntityState.Deleted:
                    context.Clients.All.itemsRemoved(type, rejuvenatorId, entries);
                    break;
                case EntityState.Modified:
                    context.Clients.All.itemsUpdated(type, rejuvenatorId, entries);
                    break;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}