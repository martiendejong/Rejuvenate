using Rejuvenate;
using RejuvenatingExample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RejuvenatingExample
{
    public class ExampleHub : RejuvenatingHub
    {
        public static IExampleContext DbContext;

        public static Dictionary<string, string> UserNameByConnectionId = new Dictionary<string, string>();

        private Player GetCurrentPlayer()
        {
            var user = UserNameByConnectionId[Context.ConnectionId];
            var player = DbContext.Players.FirstOrDefault(p => p.Name == user);
            if (player == null)
            {
                player = new Player { Name = user };
                DbContext.Players.Add(player);
                DbContext.SaveChanges();
            }
            return player;
        }

        public void hostGame()
        {
            var player = GetCurrentPlayer();
            DbContext.Games.Add(new Game(player));
            DbContext.SaveChanges();
        }

        public void joinGame(int id)
        {
            var player = GetCurrentPlayer();
            var game = DbContext.Games.FirstOrDefault(g => g.Id == id);
            if (game == null || game.Players.Contains(player))
                return;

            foreach (var g in DbContext.Games.Where(g => g.Players.Any(p =>  p.Id == player.Id) && g.Id != id))
            {
                g.Players.Remove(player);
            }
            game.Players.Add(player);
            DbContext.SaveChanges();
        }

        public void setUser(string name)
        {
            UserNameByConnectionId[Context.ConnectionId] = name;
        }


        // first example functions
        public void update()
        {
            var item = DbContext.Items.First();
            item.Name = item.Name + "+";
            DbContext.SaveChanges();
        }

        public void add()
        {
            DbContext.Items.Add(new Item { Name = "asdas" });
            DbContext.SaveChanges();
        }

        public void remove()
        {
            var item = DbContext.Items.First();
            DbContext.Items.Remove(item);
            DbContext.SaveChanges();
        }
    }
}