using Microsoft.AspNet.SignalR;
using Rejuvenate;
using Rejuvenate.v2;
using Rejuvenate.v2.SignalRChangePublishing;
using RejuvenatingExample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RejuvenatingExample.Controllers
{
    public class HomeController : Controller
    {
        //public IExampleContext DbContext { get; }

        public IExampleV2Context V2Context { get; }

        public ISignalRHubListener<ExampleV2Hub> Publisher { get; }

        public HomeController(/*IExampleContext dbContext, */IExampleV2Context v2Context, ISignalRHubListener<ExampleV2Hub> publisher)
        {
            //DbContext = dbContext;
            V2Context = v2Context;
            Publisher = publisher;
        }

        public ActionResult Index()
        {
            // todo use DI
            ExampleV2Hub.DbContext = V2Context;
            var rejuvenatingQuery = V2Context.Items.Where(i => i.Name.Length < 8);
            var rejuvenator = rejuvenatingQuery.Subscribe<ExampleV2Hub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            IEnumerable<Item> result = rejuvenatingQuery.AsQueryable();
            return View(result);
        }

        public ActionResult Index2()
        {
            // todo use DI
            ExampleV2Hub.DbContext = V2Context;
            var rejuvenatingQuery = V2Context.Items.Where(i => i.Name.Length > 8);
            var rejuvenator = rejuvenatingQuery.Subscribe<ExampleV2Hub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            IEnumerable<Item> result = rejuvenatingQuery.AsQueryable();
            return View("Index", result);
        }
        
        public ActionResult Index3()
        {
            ExampleV2Hub.DbContext = V2Context;

            var items = V2Context.Items.Where(x2 => x2.Name.Length < 8);

            var channel2 = items.Subscribe<ExampleV2Hub>();

            var channel = V2Context.Games.Subscribe<ExampleV2Hub>();
            V2Context.Games.Where(x2 => x2.Name.Length < 5).SubscribeChildEntity<Player, int?, ExampleV2Hub>
            (
                player => player.GameId, 
                id => V2Context.Games.SingleOrDefault(game => game.Id == id), 
                channel
            );


            // todo use this for extension methods
            /*
            DbContext ctx = (DbContext)V2Context;
            var objCtx = ((IObjectContextAdapter)ctx).ObjectContext;
            bool eventCalled = false;
            objCtx.SavingChanges += (sender, args) => eventCalled = true;
            ctx.SaveChanges();
            */


            //V2Context.Items.Add(item);
            //V2Context.SaveChanges();
            //V2Context.Entry(item).Reload();
            //(DbContext).Entry( .get
            //V2Context.Items.Include()
                


            ViewBag.RejuvenatorId = channel2.Id.ToString();

            // old way
            // todo use DI
            /*ExampleHub.DbContext = DbContext;
            var rejuvenatingQuery = DbContext.ChangePublishingItems.Where(i => i.Name.Length > 9);
            var rejuvenator = rejuvenatingQuery.Subscribe<ExampleHub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            IEnumerable<Item> result = rejuvenatingQuery.AsQueryable();*/
            return View("Index3", items.ToList());
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