
using CardGame;
using Rejuvenate;
using System.Data.Entity;

namespace CardGame.Models
{
    public interface IGameContext : IChangePublishingDbContext
    {
        IChangePublishingDbSet<Player> Players { get; }

        IChangePublishingDbSet<Game> Games { get; }
    }

    public class GameContext : ChangePublishingDbContext, IGameContext
    {
        public GameContext() : base("name=DefaultConnection") { }

        public GameContext(string connString) : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual IChangePublishingDbSet<Player> Players { get; set; }

        public virtual IChangePublishingDbSet<Game> Games { get; set; }
    }
}