using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RejuvenatingExample.Models
{
    public class EntityWithNumericalKey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    public class Item : EntityWithNumericalKey
    {
        public string Name { get; set; }
    }

    public class Game : EntityWithNumericalKey
    {
        public string Name { get; set; }
        
        [MinLength(1)]
        public string Host { get; set; }

        public List<string> Players { get; set; }

        public Game()
        {
            Players = new List<string>();
        }

        public Game(string host)
        {
            Host = host;
            Players = new List<string>();
            Players.Add(host);
        }
    }
}