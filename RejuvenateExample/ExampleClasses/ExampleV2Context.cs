using Rejuvenate.Db;
using Rejuvenate.v2;
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
    public class ExampleV2Context : Rejuvenate.v2.ChangePublishingDbContext
    {
        public ExampleV2Context() : base("name=DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        private DbSet<Item> _items { get; set; }

        private DbSet<Game> _games { get; set; }

        private DbSet<Player> _players { get; set; }

        public ChangePublishingDbSet<Item> Items
        {
            get
            {
                return Set<Item>();
            }
        }

        public ChangePublishingQueryable<Game> Games
        {
            get
            {
                return Set<Game>();
            }
        }

        public ChangePublishingQueryable<Player> Players
        {
            get
            {
                return Set<Player>();
            }
        }
    }
}