
using Rejuvenate;

namespace ChangePublishingDbContextExample.Models
{
    public interface IGameContext : IChangePublishingDbContext
    {
        IChangePublishingDbSet<Player> Players { get; }

        IChangePublishingDbSet<Area> Areas { get; }
    }

    public class GameContext : Rejuvenate.ChangePublishingDbContext, IGameContext
    {
        public GameContext() : base("name=DefaultConnection") { }

        public GameContext(string connString) : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual IChangePublishingDbSet<Player> Players { get; set; }

        public virtual IChangePublishingDbSet<Area> Areas { get; set; }
    }
}