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
            this.startingPosition = new List<Vector2>();
        }

        /// <summary>
        /// Source name for image used in background during gameplay
        /// </summary>
        public string bgImage;

        /// <summary>
        /// Source for the real map
        /// </summary>
        public string mapImage;

        /// <summary>
        /// Of much the map is offset from the 
        /// </summary>
        public Vector2 mapPosition;
        /// <summary>
        /// Source name of thumb Image, used for map selection screen
        /// </summary>
        public string thumbImage;

        /// <summary>
        /// Name of the map
        /// </summary>
        public string name;

        /// <summary>
        /// A list of rectangles that have collisiondetection in all directions.
        /// </summary>
        public List<Box> boxes;

        /// <summary>
        /// A list of rectangles that only have upwards collisiondetection and only if the player is not holding down "down".
        /// </summary>
        public List<Box> floatingBoxes;

        /// <summary>
        /// List with four positions that says where the players can start
        /// </summary>
        public List<Vector2> startingPosition;

        /// <summary>
        /// How large the map is, edges is placed at the edges of this size
        /// X and y becomes the offset of the rectangle
        /// </summary>
        public Box size;

        /// <summary>
        /// The zooming bounds of the map
        /// </summary>
        public Box zoomBox;


        public void AddBoxes(params Box[] boxes)
        {
            this.boxes = boxes.ToList();
        }

        public void AddBox(float x, float y, float widht, float height, float rotation = 0)
        {
            if (this.boxes == null) this.boxes = new List<Box>();

            this.boxes.Add(new Box(widht, height, x, y, rotation));
        }

        public void AddFloatBox(float x, float y, float widht, float rotation = 0)
        {
            if (this.floatingBoxes == null) this.floatingBoxes = new List<Box>();

            this.floatingBoxes.Add(new Box(widht, 10, x, y, rotation));
        }

        public void AddStartPos(float x, float y)
        {
            if (this.startingPosition == null) this.startingPosition = new List<Vector2>();
            this.startingPosition.Add(new Vector2(x, y));
        }
    }

}
