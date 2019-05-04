using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContextExample.Models
{
    public class Player
    {
        [Key]
        public int Key { get; set; }

        public Area Area { get; set; }

        public string Name { get; set; }
    }
}
