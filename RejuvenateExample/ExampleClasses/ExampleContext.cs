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
    public class ExampleContext : RejuvenatingDbContext
    {
        #region Regular DbContext

        public ExampleContext() : base("name=DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Item> Items { get; set; }

        #endregion

        #region RejuvenatingDbContext

        // declare the change aware db query
        public IRejuvenatingQueryable<Item> ChangeAwareItems
        {
            get
            {
                return new RejuvenatingQueryable<Item>(Items, this);
            }
        }

        // declare the polling executors for the change aware entities
        override protected List<IEntityRejuvenator> EntityRejuvenators
        {
            get
            {
                return new List<IEntityRejuvenator>
                {
                    GetRejuvenator<Item>()
                };
            }
        }

        #endregion
    }
}