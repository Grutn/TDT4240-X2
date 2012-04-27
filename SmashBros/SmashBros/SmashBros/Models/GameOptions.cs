using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmashBros.Models
{
    public class GameOptions
    {
        public GameOptions()
        {
            Lifes = 3;
            Minutes = 3;
        }

        public bool UseLifes;
        public int Lifes;
        public int Minutes;
        public int Gravity;
        public int CharacterSeed;

        public List<MenuEntry> CreateMenu(Action<int> action)
        {
            List<MenuEntry> l = new List<MenuEntry>();
            l.Add(new MenuEntry(string.Format("Use lifes : {0}", UseLifes ? "On" : "Off"), action));

            if (UseLifes)
            {
                l.Add(new MenuEntry(string.Format("Number of lifes : {0}", Lifes),action));
            }else{
                l.Add(new MenuEntry(string.Format("Time limit : {0}min", Minutes), action));
            }

            return l;
        }
    }
}
