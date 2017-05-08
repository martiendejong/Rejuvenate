using Microsoft.AspNet.SignalR;
using Rejuvenate;
using Rejuvenate.v2;
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
        public IExampleContext DbContext { get; }

        public IExampleV2Context V2Context { get; }

        public Rejuvenate.v2.ISignalRHubPublisher<ExampleV2Hub> Publisher { get; }

        public HomeController(IExampleContext dbContext, IExampleV2Context v2Context, Rejuvenate.v2.ISignalRHubPublisher<ExampleV2Hub> publisher)
        {
            DbContext = dbContext;
            V2Context = v2Context;
            Publisher = publisher;
        }

        public ActionResult Index()
        {
            // todo use DI
            ExampleHub.DbContext = DbContext;
            var rejuvenatingQuery = DbContext.ChangePublishingItems.Where(i => i.Name.Length < 10);
            var rejuvenator = rejuvenatingQuery.Subscribe<ExampleHub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            IEnumerable<Item> result = rejuvenatingQuery.AsQueryable();
            return View(result);
        }

        public ActionResult Index2()
        {
            // todo use DI
            ExampleHub.DbContext = DbContext;
            var rejuvenatingQuery = DbContext.ChangePublishingItems.Where(i => i.Name.Length > 9);
            var rejuvenator = rejuvenatingQuery.Subscribe<ExampleHub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            IEnumerable<Item> result = rejuvenatingQuery.AsQueryable();
            return View("Index", result);
        }
        
        public ActionResult Index3()
        {
            
            var channel2 = V2Context.Items.Where(x2 => x2.Name.Length < 5).Subscribe<ExampleV2Hub>();


            V2Context.Items.Add(new Item { Name = "hoi" });
            V2Context.SaveChanges();


            
            ViewBag.RejuvenatorId = channel2.Guid.ToString();

            // old way
            // todo use DI
            /*ExampleHub.DbContext = DbContext;
            var rejuvenatingQuery = DbContext.ChangePublishingItems.Where(i => i.Name.Length > 9);
            var rejuvenator = rejuvenatingQuery.Subscribe<ExampleHub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            IEnumerable<Item> result = rejuvenatingQuery.AsQueryable();*/
            return View("Index3", V2Context.Items);
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