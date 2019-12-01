using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardGame.Models;
using Rejuvenate;
using System.Data.Entity;
using System.Net;
using AutoMapper;

namespace CardGame.Controllers
{
    public class GameController : Controller
    {
        private IGameContext Db;

        private IMapper Mapper;

        public GameController(IGameContext dbContext, IMapper mapper)
        {
            Db = dbContext;
            Mapper = mapper;
        }

        // GET: Games list
        public ActionResult Index()
        {
            return View(Db.Games.Where(game => game.Players.Count < 2).Select(game => Mapper.Map<GameViewModel>(game)));

            /*var vm = new GameViewModel
            {
                Players = Db.Players,
                Areas = Db.Areas,
                Guids = new Guid[] 
                {
                    Db.Players.Publisher<GameHub>().Id
                }
            };

            return View(vm);*/
        }

        [HttpPost]
        public ActionResult AddPlayer(string name)
        {            
            var area = Db.Games.FirstOrDefault();
            Db.Players.Add(new Player { Name = name });
            Db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult ClearList()
        {
            var players = Db.Players.ToList();
            players.ForEach(p => Db.Players.Remove(p));
            Db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}