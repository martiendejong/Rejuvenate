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
            // get user
            var owinContext = HttpContext.GetOwinContext();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(owinContext.Get<ApplicationDbContext>()));
            var user = manager.FindById(User.Identity.GetUserId());

            // get query results
            var query = DbContext.ChangePublishingGames;
            var result = query.AsQueryable()
                .Include(game => game.Host)
                .Include(game => game.Players)
                .ToList();

            // subscribe to changes in Game entities
            var publisher = query.Subscribe<ExampleHub>();
            // subscribe to changes in Player entities and publish the corresponding Game entities
            query.SubscribeLinkedEntity<Player, ExampleHub, int>
            (
                DbContext.ChangePublishingPlayers, 
                player => player.Game, 
                player => player.GameId.Value, 
                (originalGameIds) => DbContext.Games.Where
                (
                    g => originalGameIds.Contains(g.Id)
                )
                .Include(game => game.Host)
                .Include(game => game.Players), 
                publisher.Id
            );

            // store data in the viewbag
            ViewBag.UserEmail = user.Email;
            ViewBag.PublisherId = publisher.Id;

            return View(result);
        }
    }
}