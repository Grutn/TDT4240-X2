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

namespace SmashBros.Controllers
{
    class CameraController : Controller
    {
        Camera2D camera;
        List<Sprite> targets { get; set; }
        Body box;
        public CameraController(ScreenController screen) : base(screen)
        {
            this.camera = screen.controllerViewManager.camera;
        }

        public override void Load(ContentManager content)
        {

            box = BodyFactory.CreateRectangle(World, 1280, 720, 1.0f);
            box.IgnoreCCD = true;
            box.IsStatic = true;

            this.camera.TrackingBody = box;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }
    }
}
