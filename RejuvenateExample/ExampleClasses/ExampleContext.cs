using Rejuvenate.Db;
using RejuvenatingExample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RejuvenatingExample
{
    public interface IExampleContext : IChangePublishingDbContext
    {
        DbSet<Item> Items { get; }

        DbSet<Game> Games { get; }

        DbSet<Player> Players { get; }

        IChangePublishingQueryable<Item> ChangePublishingItems { get; }

        IChangePublishingQueryable<Game> ChangePublishingGames { get; }

        IChangePublishingQueryable<Player> ChangePublishingPlayers { get; }
    }

    public class ExampleContext : ChangePublishingDbContext, IExampleContext
    {
        #region Regular DbContext

        public ExampleContext() : base("name=DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Item> Items { get; set; }

        public virtual DbSet<Game> Games { get; set; }

        public virtual DbSet<Player> Players { get; set; }

        #endregion

        #region RejuvenatingDbContext

        // declare the change aware db query
        public IChangePublishingQueryable<Item> ChangePublishingItems
        {
            get
            {
                return new ChangePublishingQueryable<Item>(Items, this);
            }
        }

        public IChangePublishingQueryable<Game> ChangePublishingGames
        {
            get
            {
                return new ChangePublishingQueryable<Game>(Games, this);
            }
        }

        public IChangePublishingQueryable<Player> ChangePublishingPlayers
        {
            get
            {
                return new ChangePublishingQueryable<Player>(Players, this);
            }
        }

        // declare the polling executors for the change aware entities
        override protected List<IChangeProcessor> ChangeProcessors
        {
            get
            {
                return new List<IChangeProcessor>
                {
                    GetChangeProcessor<Item>(),
                    GetChangeProcessor<Game>(),
                    GetChangeProcessor<Player>()
                };
            }
        }

        #endregion
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                        .HasOptional(p => p.Game)
                        .WithMany(g => g.Players);
            modelBuilder.Entity<Game>()
                        .HasOptional(p => p.Host);
        }
    }
}