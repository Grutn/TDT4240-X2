using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmashBros.Model
{
    public class Move
    {
        /// <summary>
        /// F.Eks. Skyting vs. Melee
        /// </summary>
        public bool range;

        /// <summary>
        /// Om retningen til angrepsfirkanten kan styres.
        /// </summary>
        public bool adjustable;

        /// <summary>
        /// Maximum radians per sec the movedirection changes with navigation.
        /// </summary>
        public float adjustAcc;

        /// <summary>
        /// Minimum/Maximum ventetid før angrepsfirkant opprettes og angrepet utføres.
        /// </summary>
        public int minWait, maxWait;
        
        /// <summary>
        /// Minimum og maksimun slagkraft, regnes med hensyn til min-/max-Wait.
        /// </summary>
        public int minPower, maxPower;

        /// <summary>
        /// Minimun and maximun damage done by move.
        /// </summary>
        public int minDamage, maxDamage;

        /// <summary>
        /// Vektorer som viser forflyttelse til kropp, og angrepsfirkant relativt til kropp.
        /// </summary>
        public float sqRange, bodyRange;

        /*
        /// <summary>
        /// Hastighet til henholdsvis agrepsfirkant og kropp.
        /// </summary>
        public int sqSpeed, bodySpeed;
        */

        /// <summary>
        /// Bredde/høyde til angrepsfirkant
        /// </summary>
        public int sqWidth, sqHeight;

        /// <summary>
        /// Tid det tar for firkant/kropp og bevege seg.
        /// </summary>
        public int duration;

        /// <summary>
        /// From what frame to what frame the animation of the move is.
        /// </summary>
        public int aniFrom, aniTo;
    }
}
