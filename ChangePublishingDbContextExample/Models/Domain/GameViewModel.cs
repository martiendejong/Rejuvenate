using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangePublishingDbContextExample.Models
{
    public class GameViewModel
    {
        public IEnumerable<Area> Areas { get; set; }

        public IEnumerable<Player> Players { get; set; }

        public Guid[] Guids { get; set; }
    }
}