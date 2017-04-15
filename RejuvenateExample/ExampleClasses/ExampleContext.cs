using Rejuvenate;
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
    public interface IExampleContext : IRejuvenatingDbContext
    {
        DbSet<Item> Items { get; }

        DbSet<Game> Games { get; }

        DbSet<Player> Players { get; }

        IRejuvenatingQueryable<Item> RejuvenatingItems { get; }

        IRejuvenatingQueryable<Game> RejuvenatingGames { get; }

        IRejuvenatingQueryable<Player> RejuvenatingPlayers { get; }
    }

    public class ExampleContext : RejuvenatingDbContext, IExampleContext
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
        public IRejuvenatingQueryable<Item> RejuvenatingItems
        {
            get
            {
                return new RejuvenatingQueryable<Item>(Items, this);
            }
        }

        public IRejuvenatingQueryable<Game> RejuvenatingGames
        {
            get
            {
                return new RejuvenatingQueryable<Game>(Games, this);
            }
        }

        public IRejuvenatingQueryable<Player> RejuvenatingPlayers
        {
            get
            {
                return new RejuvenatingQueryable<Player>(Players, this);
            }
        }

        // declare the polling executors for the change aware entities
        override protected List<IEntityRejuvenator> EntityRejuvenators
        {
            get
            {
                return new List<IEntityRejuvenator>
                {
                    GetRejuvenator<Item>(),
                    GetRejuvenator<Game>(),
                    GetRejuvenator<Player>()
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