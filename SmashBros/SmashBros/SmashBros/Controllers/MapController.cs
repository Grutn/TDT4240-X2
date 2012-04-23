using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using SmashBros.Model;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.Views;

namespace SmashBros.Controllers
{
    public class MapController : Controller
    {
        private Map map;
        private ImageTexture bg;
        private List<Body> boxes;

        public MapController(ScreenController screen, Map currentMap) : base(screen)
        {
            this.boxes = new List<Body>();
            this.map = currentMap;
        }

        public Map CurrentMap { get { return map; } set { map = value; SetUpMap(); } }

        /// <summary>
        /// Sets up the body boxes on the map according to the currentMap
        /// </summary>
        private void SetUpMap()
        {
            foreach (var box in map.boxes)
            {
                boxes.Add(box.CreateBody(World, Category.Cat9));
            }

            foreach (var box in map.floatingBoxes)
            {
                boxes.Add(box.CreateBody(World, Category.Cat10));
            }

        }

        public override void Load(ContentManager content)
        {

            bg = new ImageTexture(content, map.bgImage, 0, 0);
            AddView(bg);

            SetUpMap();

            SubscribeToGameState = true;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Deactivate()
        {
        }

        public override void OnNext(System.GameStateManager value)
        {
        }
    }
}
