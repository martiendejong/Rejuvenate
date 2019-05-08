using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChangePublishingDbContextExample.Models;
using ChangePublishingDbContext;
using System.Data.Entity;

namespace ChangePublishingDbContextExample.Controllers
{
    public class GameController : Controller
    {
        private IGameContext db;

        public GameController(IGameContext dbContext)
        {
            db = dbContext;
        }

        public ActionResult TestAddPlayer()
        {
            var vm = new GameViewModel
            {
            };

            var area = db.Areas.FirstOrDefault();
            db.Players.Add(new Player { Name = "yo", Area = area });
            db.SaveChanges();

            return View(vm);
        }

            // GET: Game
        public ActionResult Index()
        {
            var x = db.Players.Select(p => p.Area);
            var pub = x.Publisher<GameHub>();

            var vm = new GameViewModel
            {
                Players = db.Players,
                Areas = db.Areas,
                Guids = new Guid[] { db.Players.Select(p => p.Area).Publisher<GameHub>().Id, db.Areas.Publisher<GameHub>().Id }
            };

            return View(vm);
        }
    }
}