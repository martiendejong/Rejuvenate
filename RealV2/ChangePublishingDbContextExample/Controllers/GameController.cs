using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChangePublishingDbContextExample.Models;
using ChangePublishingDbContext;

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

            db.Players.Add(new Player { Name = "yo" });
            db.SaveChanges();

            return View(vm);
        }

            // GET: Game
        public ActionResult Index()
        {
            var q = db.Players;//.Where(a => true);
            var fn = q.Condition;
            var pp = ChangePublishingSignalRHub.Publishers.Select(p => p.Value).OfType<Publisher<Player, GameHub>>();
            var playerPublisher = pp.FirstOrDefault(p => LambdaCompare.Eq(p.Condition, fn));
            if (playerPublisher == null)
            {
                playerPublisher = new Publisher<Player, GameHub> { Condition = fn };
                ChangePublishingSignalRHub.Publishers.Add(playerPublisher.Id, playerPublisher);
                q.EntitiesChanged += playerPublisher.Publish;
            }

            var q2 = db.Areas;//.Where(a => true);
            var fn2 = q2.Condition;
            var pp2 = ChangePublishingSignalRHub.Publishers.Select(p => p.Value).OfType<Publisher<Area, GameHub>>();
            var areaPublisher = pp2.FirstOrDefault(p => LambdaCompare.Eq(p.Condition, fn2));
            if (areaPublisher == null)
            {
                areaPublisher = new Publisher<Area, GameHub> { Condition = fn2 };
                ChangePublishingSignalRHub.Publishers.Add(areaPublisher.Id, areaPublisher);
                q2.EntitiesChanged += areaPublisher.Publish;
            }

            var vm = new GameViewModel
            {
                Players = db.Players,
                Areas = db.Areas,
                Guids = new Guid[] { playerPublisher.Id, areaPublisher.Id }
            };

            return View(vm);
        }
    }
}