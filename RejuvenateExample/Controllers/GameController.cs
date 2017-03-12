using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RejuvenatingExample.Models;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;

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
            var result = rejuvenatingQuery.AsQueryable().ToList();
            return View(result);
        }
    }
}