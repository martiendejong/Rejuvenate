using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CardGame.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }

        public IEnumerable<Player> Players { get; set; }
    }
}