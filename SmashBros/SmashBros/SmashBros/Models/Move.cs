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
        /// Minimum/Maximum ventetid før angrepsfirkant opprettes og angrepet utføres.
        /// </summary>
        public int minWait, maxWait;
        
        /// <summary>
        /// Minimum og maksimun slagkraft, regnes med hensyn til min-/max-Wait.
        /// </summary>
        public Vector2 minPower, maxPower;

        /// <summary>
        /// Vektorer som viser forflyttelse til kropp, og angrepsfirkant relativt til kropp.
        /// </summary>
        public Vector2 sqRange, bodyRange;

        /// <summary>
        /// Hastighet til henholdsvis agrepsfirkant og kropp.
        /// </summary>
        public Vector2 sqSpeed, bodySpeed;
        
        /// <summary>
        /// Tid det tar for firkant/kropp og bevege seg.
        /// </summary>
        public int duration;
        
        /// <summary>
        /// Bredde/høyde til angrepsfirkant
        /// </summary>
        public int sqWidth, sqHeight;
    }
}
