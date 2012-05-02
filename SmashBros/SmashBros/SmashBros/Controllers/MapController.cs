using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.Views;
using FarseerPhysics.Factories;
using SmashBros.MySystem;
using SmashBros.Models;

namespace SmashBros.Controllers
{
    public class MapController : Controller
    {
        public Map Model;
        private ImageController bg;
        private ImageController map;
        private List<Body> boxes;

        public MapController(ScreenManager screen, Map currentMap) : base(screen)
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

            boxes.Add(CreateEdge(x, y, 40, h));//left
            boxes.Add(CreateEdge(x, y, w, 40, Category.Cat7));//top
            boxes.Add(CreateEdge(x + w, y, 40, h));//right
            boxes.Add(CreateEdge(x, y + h, w, 40));//bottom

        }

        private Body CreateEdge(float x, float y, float width, float height, Category cat = Category.Cat8)
        {
            Body b = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1f);
            b.IsSensor = true;
            b.Position = ConvertUnits.ToSimUnits(x+width/2, y+height/2);
            b.CollisionCategories = cat;
            boxes.Add(b);

            return b;
        }

        public override void Load(ContentManager content)
        {

            bg = new ImageController(Screen, Model.bgImage, Vector2.Zero, 1, true);
            AddController(bg);

            map = new ImageController(Screen, Model.mapImage, Vector2.Zero, 3, false);
            AddController(map);
            SetUpMap();

            SubscribeToGameState = true;
        }

        public override void Unload()
        {
            DisposeController(bg, map);
            foreach (var box in boxes)
            {
                box.Dispose();
            }
            //System.GC.SuppressFinalize(this);
        }

        float elapsedTime = 0;
        public override void Update(GameTime gameTime)
        {

            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if(elapsedTime >= 2000 ){
                DisposeBoxes();
                SetUpMap();
                elapsedTime = 0;
            }
            //bg.Scale = 1/screen.controllerViewManager.camera.Zoom-0.1f;// (1 - (-0.7f) / 1.5f) * 1.2f;
            //bg.Position(screen.controllerViewManager.camera.Position-(new Vector2(2300,1500)*bg.Scale)/2);
        }

        public void DisposeBoxes()
        {
            foreach (var box in boxes)
            {
                box.Dispose();
            }


            boxes = new List<Body>();
        }

        public override void OnNext(MySystem.GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }
    }
}
