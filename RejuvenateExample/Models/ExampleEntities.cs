using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

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

    public class Player : EntityWithNumericalKey
    {
        public string Name { get; set; }

        //public int GameId { get; set; }

        public Nullable<int> GameId { get; set; }

        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }
    }

    public class Game : EntityWithNumericalKey
    {
        public string Name { get; set; }

        //public int HostId { get; set; }

        [ScriptIgnore]
        [JsonIgnore]
        public virtual Player Host { get; set; }

        [JsonProperty(PropertyName = "Host")]
        public string HostName { get { return Host == null ? "" : Host.Name; } }

        [ScriptIgnore]
        [JsonIgnore]
        public virtual ICollection<Player> Players { get; set; }

        [JsonProperty(PropertyName = "Players")]
        public IEnumerable<string> PlayerNames { get { return Players == null ? new List<string>() : Players.Select(p => p.Name); } }

        public Game()
        {
            Players = new List<Player>();
        }

        public Game(Player host)
        {
            Host = host;
            Players = new List<Player>();
            Players.Add(host);
        }
    }
}