using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class Comunicazione
    {
        public int clientID { get; set; }
        public string method { get; set; }
        public string info { get; set; }
        public bool statoPartita { get; set; }
        public bool readyEnabled { get; set; }
        public bool chatEnabled { get; set; }
        public Pokemon mypokemon { get; set; }

        public int Turno { get; set; }

        public int hpP1 { get; set; }
        public int hpP2 { get; set; }

        public string atkname { get; set; }
        public int atkdp { get; set; }
        public string atktype { get; set; }
    }
}
