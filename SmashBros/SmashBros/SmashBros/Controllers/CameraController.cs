using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.System;
using SmashBros.Views;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using SmashBros.Model;
using FarseerPhysics.Collision.Shapes;

namespace SmashBros.Controllers
{
    class CameraController : Controller
    {
        Camera2D camera;
        List<CharacterController> characters{ get; set; }
        DateTime time;
            
        public CameraController(ScreenController screen) : base(screen)
        {
            this.characters = new List<CharacterController>();
            this.camera = screen.controllerViewManager.camera;
        }

        public override void Load(ContentManager content)
        {

            time = DateTime.Now;

            
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            float minX = 9999, minY = 9999, maxX = 0, maxY = 0;
            foreach (var chara in characters)
            {
                var t = chara.character;
                if (t != null)
                {
                    if (minX > t.PositionX) minX = t.PositionX;
                    if (minY > t.PositionY) minY = t.PositionY;

                    if (maxX < t.PositionX + 200) maxX = t.PositionX + 200;
                    if (maxY < t.PositionY + 200) maxY = t.PositionY + 200;
                }
            }
            minX -= 200;
            maxX += 200;

            minY -= 200;
           // maxY += 200;

            maxY = MathHelper.Clamp(maxY, 0, 1400);
            maxX = MathHelper.Clamp(maxX, 0, 1920);
            float zoom = MathHelper.Min(
                Constants.WindowWidth / (maxX - minX),
                Constants.WindowHeight / (maxY - minY)
            );

            //camera.MinPosition = new Vector2(750/zoom, 300/zoom);
            //camera.MaxPosition = new Vector2(3000, 3000);

            camera.Position = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
            camera.Zoom = zoom;

            DebugWrite("Zoom", zoom);
            DebugWrite("Min pos", camera.MinPosition.X, camera.MinPosition.Y);
            DebugWrite("Max pos", camera.MaxPosition.X, camera.MaxPosition.Y);
            DebugWrite("Position", camera.Position.X, camera.Position.Y);
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        internal void AddCharacterTarget(CharacterController character)
        {
            this.characters.Add(character);
        }
    }
}
