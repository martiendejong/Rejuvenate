using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace poller.Controllers
{
    public class HomeController : Controller
    {
        public PollerExampleContext DbContext = new PollerExampleContext();

        public ActionResult Index()
        {
            // todo make di
            MyHub.DbContext = DbContext;
            IEnumerable<Item> query = DbContext.ChangeAwareItems.Where(i => i.Name.Length < 10).AddPollingFunction((type, pollerId, state, entries) => { PublishItems(type, pollerId, state, entries); });
            return View(query);
        }

        // todo refactor away
        public void PublishItems(Type type, int pollerId, EntityState state, IEnumerable<Item> entries)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            switch (state)
            {
                case EntityState.Added:
                    context.Clients.All.itemsAdded(type, pollerId, entries);
                    //Clients.All.itemsAdded(type, pollerId, entries);
                    break;
                case EntityState.Deleted:
                    context.Clients.All.itemsRemoved(type, pollerId, entries);
                    //Clients.All.itemsRemoved(type, pollerId, entries);
                    break;
                case EntityState.Modified:
                    context.Clients.All.itemsUpdated(type, pollerId, entries);
                    //Clients.All.itemsUpdated(type, pollerId, entries);
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