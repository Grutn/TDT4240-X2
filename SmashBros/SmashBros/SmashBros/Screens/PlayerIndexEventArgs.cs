using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmashBros
{
    class PlayerIndexEventArgs : EventArgs
    {
        PlayerIndex playerIndex;

        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }
    }
}
