using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Runtime.Serialization;

namespace SmashBros.Model
{
    public class Map
    {
        public Map()
        {
            this.boxes = new List<Box>();
            this.floatingBoxes = new List<Box>();
        }

        /// <summary>
        /// Static image that displays the whole map.
        /// </summary>
        public string assetName;

        /// <summary>
        /// A list of rectangles that have collisiondetection in all directions.
        /// </summary>
        public List<Box> boxes;

        /// <summary>
        /// A list of rectangles that only have upwards collisiondetection and only if the player is not holding down "down".
        /// </summary>
        public List<Box> floatingBoxes;


        public void AddBoxes(params Box[] boxes)
        {
            this.boxes = boxes.ToList();
        }
    }

}
