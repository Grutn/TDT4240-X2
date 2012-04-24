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
        private Map model;
        private ImageTexture bg;
        private ImageTexture map;
        private List<Body> boxes;

        public MapController(ScreenController screen, Map currentMap) : base(screen)
        {
            this.boxes = new List<Body>();
            this.model = currentMap;
        }

        public Map CurrentMap { get { return model; } set { model = value; SetUpMap(); } }

        /// <summary>
        /// Sets up the body boxes on the map according to the currentMap
        /// </summary>
        private void SetUpMap()
        {
            foreach (var box in model.boxes)
            {
                boxes.Add(box.CreateBody(World, Category.Cat9));
            }

            foreach (var box in model.floatingBoxes)
            {
                boxes.Add(box.CreateBody(World, Category.Cat10));
            }

        }

        public override void Load(ContentManager content)
        {

            bg = new ImageTexture(content, model.bgImage, 0, 0);
            bg.Layer = 1;
            bg.StaticPosition = true;
            AddView(bg);

            map = new ImageTexture(content, model.mapImage, 0, 0);
            map.Layer = 3;
            AddView(map);
            SetUpMap();

            SubscribeToGameState = true;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {

            //bg.Scale = 1/screen.controllerViewManager.camera.Zoom-0.1f;// (1 - (-0.7f) / 1.5f) * 1.2f;
            //bg.Position(screen.controllerViewManager.camera.Position-(new Vector2(2300,1500)*bg.Scale)/2);
        }

        public override void Deactivate()
        {
        }

        public override void OnNext(System.GameStateManager value)
        {
        }
    }
}
