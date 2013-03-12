using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen
{
    class GameManager
    {
        public List<Player> Players { get; set; }
        public List<Enemy>  Enemies { get; set; }
        public List<Prop>   Props   { get; set; }

        public GameManager()
        {
            Players = new List<Player>();
            Enemies = new List<Enemy>();
            Props = new List<Prop>();
        }
    }
}
