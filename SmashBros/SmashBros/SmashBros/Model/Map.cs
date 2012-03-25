using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.Objects
{
    class Map
    {
        /// <summary>
        /// Static image that displays the whole map.
        /// </summary>
        Texture2D backgroundImage;

        /// <summary>
        /// A list of rectangles that have collisiondetection in all directions.
        /// </summary>
        List<Rectangle> boxes;

        /// <summary>
        /// A list of rectangles that only have upwards collisiondetection and only if the player is not holding down "down".
        /// </summary>
        List<Rectangle> floatingBoxes;
    }
}
