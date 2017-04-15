using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RejuvenatingExample.Models;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Collections.Generic;
using System;
using Rejuvenate;
using System.Linq.Expressions;

namespace RejuvenatingExample.Controllers
{
    public class GameController : Controller
    {
        public IExampleContext DbContext { get; }

        public GameController(IExampleContext dbContext)
        {
            DbContext = dbContext;
            ExampleHub.DbContext = DbContext;
        }

        [Authorize]
        public ActionResult Index()
        {
            var owinContext = HttpContext.GetOwinContext();

            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(owinContext.Get<ApplicationDbContext>()));
            var user = manager.FindById(User.Identity.GetUserId());

            ViewBag.UserEmail = user.Email;

            //DbContext.Games.Add(new Game(user.Email));
            //DbContext.SaveChanges();

            var rejuvenatingQuery = DbContext.RejuvenatingGames;
            var rejuvenator = rejuvenatingQuery.RejuvenateQuery<ExampleHub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;





            var result = rejuvenatingQuery.AsQueryable().Include(game => game.Host).Include(game => game.Players).ToList();

            // this should become a replacement of the thing below
            Func<IQueryable<int>, IQueryable<Game>> getOriginalGames = (originalGameIds) => DbContext.Games.Where(g => originalGameIds.Contains(g.Id));
            rejuvenatingQuery.RejuvenateInclude<Player, ExampleHub, int>(DbContext.RejuvenatingPlayers, player => player.Game, player => player.GameId.Value, getOriginalGames, rejuvenator.Id);

            
            //var where = (originalGameIds, g) => originalGameIds.Contains();
            /*RejuvenateClientCallback<Player> callback = (Type type, int rejuvenatorId, EntityState state, IEnumerable<KeyValuePair<Player, Player>> entities) => 
            {
                var signalRHubRejuvenator = new SignalRHubRejuvenator<ExampleHub>();

                var games = entities.Select(player => player.Key.Game).Distinct();
                signalRHubRejuvenator.Rejuvenate(type, rejuvenator.Id, EntityState.Modified, games);

                var originalGameIds = entities.Where(p => p.Value != null).Select(player => player.Value.GameId).Distinct();
                var updatedGames = DbContext.Games.Where(g => originalGameIds.Contains(g.Id));
                signalRHubRejuvenator.Rejuvenate(type, rejuvenator.Id, EntityState.Modified, updatedGames);
            };
            DbContext.RejuvenatingPlayers.RejuvenateQuery(callback);*/
            return View(result);
        }
    }
}