using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangePublishingDbContextExample.Models
{
    public class Area
    {
        [Key]
        public int Key { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public string Name { get; set; }
    }
}