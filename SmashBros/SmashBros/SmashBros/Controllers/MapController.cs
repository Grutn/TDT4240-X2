using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using SmashBros.Model;

namespace SmashBros.Controllers
{
    public class MapController
    {
        private Map _currentMap;
        private List<Body> _boxes;
        private World _world;

        public MapController(World world)
        {
            _boxes = new List<Body>();
            _world = world;
        }

        public Map CurrentMap { get { return _currentMap; } set { _currentMap = value; SetUpMap(); } }

        public void Draw(SpriteBatch _batch)
        {

        }

        /// <summary>
        /// Sets up the body boxes on the map according to the currentMap
        /// </summary>
        private void SetUpMap()
        {
            foreach (var box in _currentMap.boxes)
            {
                _boxes.Add(box.CreateBody(_world));
            }

        }
    }
}
