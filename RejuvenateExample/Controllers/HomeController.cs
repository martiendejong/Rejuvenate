using Microsoft.AspNet.SignalR;
using Rejuvenate;
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