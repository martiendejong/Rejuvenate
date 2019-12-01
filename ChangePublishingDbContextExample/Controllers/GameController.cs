using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChangePublishingDbContextExample.Models;
using Rejuvenate;
using System.Data.Entity;
using System.Net;

namespace ChangePublishingDbContextExample.Controllers
{
    public class GameController : Controller
    {
        private IGameContext DbContext;

        public GameController(IGameContext dbContext)
        {
            DbContext = dbContext;
        }

        // GET: Game
        public ActionResult Index()
        {
            var vm = new GameViewModel
            {
                Players = DbContext.Players,
                Areas = DbContext.Areas,
                Guids = new Guid[] 
                {
                    /*DbContext.Players.Select(p => p.Area).Publisher<GameHub>().Id,
                    DbContext.Areas.Publisher<GameHub>().Id,*/
                    DbContext.Players.Publisher<GameHub>().Id
                }
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddPlayer(string name)
        {            
            var area = DbContext.Areas.FirstOrDefault();
            DbContext.Players.Add(new Player { Name = name, Area = area });
            DbContext.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult ClearList()
        {
            var players = DbContext.Players.ToList();
            players.ForEach(p => DbContext.Players.Remove(p));
            DbContext.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}