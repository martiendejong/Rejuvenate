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

        public HomeController(IExampleContext dbContext)
        {
            DbContext = dbContext;
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
            // new way

            var x = new ExampleV2Context();
            x.ChangePublisher.Subscribe<Item>((changes) => 
            {
                Console.Out.WriteLine("Entities changed");
            });

            EntitiesChangedChannelHandler<Item> func = (changes, channelId) =>
            {
                Console.Out.WriteLine("Entities changed");
            };
            

            var channel = x.Items.Where(x2 => x2.Name.Length > 3).Subscribe(func);
            Guid g2 = x.Items.Where(x2 => x2.Name.Length > 3).Subscribe(func).Guid;


            EntitiesChangedHandler<Item> func2 = (changes) =>
            {
                //itemP.Receive<Item>
                Console.Out.WriteLine("Entities changed");
            };
            Rejuvenate.v2.SignalRHubPublisher<Rejuvenate.v2.ChangePublishingHubV2> itemP = new SignalRHubPublisher<ChangePublishingHubV2>();
            var channel2 = x.Items.Where(x2 => x2.Name.Length < 5).Subscribe(itemP.Receive);


            x.Items.Add(new Item { Name = "hoi" });
            x.SaveChanges();


            
            ViewBag.RejuvenatorId = channel.Guid.ToString();

            // old way
            // todo use DI
            /*ExampleHub.DbContext = DbContext;
            var rejuvenatingQuery = DbContext.ChangePublishingItems.Where(i => i.Name.Length > 9);
            var rejuvenator = rejuvenatingQuery.Subscribe<ExampleHub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            IEnumerable<Item> result = rejuvenatingQuery.AsQueryable();*/
            return View("Index", x.Items);
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