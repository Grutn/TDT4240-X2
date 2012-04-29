using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmashBros.Models
{
    public class PlayerStats
    {
        public PlayerStats(int lifes, int playerIndex)
        {
            PlayerDamageDone = new Dictionary<int, int>();
            PlayerKills = new Dictionary<int, int>();
            LifesLeft = lifes;
            this.PlayerIndex = playerIndex;
        }

        public int PlayerIndex;
        public  Dictionary<int, int> PlayerDamageDone { get; private set; }
        public  Dictionary<int, int> PlayerKills { get; private set; }
        public int LifesLeft { get; set; }
        public int DamageDone { get { return PlayerDamageDone.Sum(a => a.Value); } }
        public int DamagePoints { get; set; }
        public int HitsDone { get; set; }
        public int LastHitBy { get; set; }
        public bool IsWinner { get; set; }

        public override string ToString()
        {
            string s = "Player "+ (PlayerIndex + 1) + "\n";
            if (IsWinner)
                s += "Won";
            else s += "Lost";

            s += " this game \n";


            return s;
        }
    }
}
