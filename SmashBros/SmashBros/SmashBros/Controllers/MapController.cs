using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using SmashBros.Model;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace SmashBros.Controllers
{
    public class MapController : Controller
    {
        private Map _currentMap;
        private List<Body> _boxes;

        public MapController(ScreenController screen, Map currentMap) : base(screen)
        {
            _boxes = new List<Body>();
        }

        public Map CurrentMap { get { return _currentMap; } set { _currentMap = value; SetUpMap(); } }

        /// <summary>
        /// Sets up the body boxes on the map according to the currentMap
        /// </summary>
        private void SetUpMap()
        {
            foreach (var box in _currentMap.boxes)
            {
                _boxes.Add(box.CreateBody(screen.world));
            }

        }

        public override void Load(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
