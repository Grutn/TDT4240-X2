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
using FarseerPhysics.Factories;
using SmashBros.System;

namespace SmashBros.Controllers
{
    public class MapController : Controller
    {
        public Map Model;
        private ImageTexture bg;
        private ImageTexture map;
        private List<Body> boxes;

        public MapController(ScreenController screen, Map currentMap) : base(screen)
        {
            this.boxes = new List<Body>();
            this.Model = currentMap;
        }

        public Map CurrentMap { get { return Model; } set { Model = value; SetUpMap(); } }

        /// <summary>
        /// Sets up the body boxes on the map according to the currentMap
        /// </summary>
        private void SetUpMap()
        {
            //Create normal boxes from map model
            foreach (var box in Model.boxes)
            {
                boxes.Add(box.CreateBody(World, Category.Cat9));
            }

            //Create floating boxes from map model 
            foreach (var box in Model.floatingBoxes)
            {
                boxes.Add(box.CreateBody(World, Category.Cat10));
            }

            //Creates edges around the map uses the map size
            float x = Model.size.X, y = Model.size.Y,
                w = Model.size.Width, h = Model.size.Height;

            CreateEdge(x, y, 10, h);//left
            CreateEdge(x, y, w, 10);//top
            CreateEdge(x + w, y, 10, h);//right
            CreateEdge(x, y + h, w, 10);//bottom

        }

        private void CreateEdge(float x, float y, float width, float height)
        {
            Body b = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1f);
            b.IsSensor = true;
            b.Position = ConvertUnits.ToSimUnits(x+width/2, y+height/2);
            b.CollisionCategories = Category.Cat8;
            boxes.Add(b);
        }

        public override void Load(ContentManager content)
        {

            bg = new ImageTexture(content, Model.bgImage, 0, 0);
            bg.Layer = 1;
            bg.StaticPosition = true;
            AddView(bg);

            map = new ImageTexture(content, Model.mapImage, 0, 0);
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
