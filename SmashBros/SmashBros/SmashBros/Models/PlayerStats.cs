using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmashBros.Models
{
    public class PlayerStats
    {
        public PlayerStats(int lifes)
        {
            PlayerDamageDone = new Dictionary<int, int>();
            PlayerKills = new Dictionary<int, int>();
            LifesLeft = lifes;
        }

        public  Dictionary<int, int> PlayerDamageDone { get; private set; }
        public  Dictionary<int, int> PlayerKills { get; private set; }
        public int LifesLeft { get; set; }
        public int DamageDone { get { return PlayerDamageDone.Sum(a => a.Value); } }
        public int DamagePoints { get; set; }
        public int HitsDone { get; set; }
        public int LastHitBy { get; set; }
    }
}
