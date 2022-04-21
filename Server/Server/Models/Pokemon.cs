using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Pokemon
    {
        public string name { get; set; }
        public string shortname { get; set; }
        public int hp { get; set; }
        public Info info { get; set; }
        public Images images { get; set; }
        public Move[] moves { get; set; }
    }

    public class Info
    {
        public int id { get; set; }
        public string type { get; set; }
        public string weakness { get; set; }
        public string description { get; set; }
    }

    public class Images
    {
        public string photo { get; set; }
        public string typeIcon { get; set; }
        public string weaknessIcon { get; set; }
    }

    public class Move
    {
        public string name { get; set; }
        public string type { get; set; }
        public int dp { get; set; }
    }
}