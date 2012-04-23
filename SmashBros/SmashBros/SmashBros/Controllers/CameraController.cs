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

namespace SmashBros.Controllers
{
    class CameraController : Controller
    {
        Camera2D camera;
        List<Sprite> targets { get; set; }
        Body box;
        float zoom = 0.7f;
        public CameraController(ScreenController screen) : base(screen)
        {
            this.camera = screen.controllerViewManager.camera;
        }

        public override void Load(ContentManager content)
        {

            Viewport v = screen.GraphicsDevice.Viewport;
            camera.Zoom = zoom;

            MoveCamera(v.Width * (1 - zoom), v.Height * (1 - zoom)); 
            //this.camera.TrackingBody = box;
        }

        private void MoveCamera(float x, float y)
        {
            camera.MoveCamera(ConvertUnits.ToSimUnits(x,y));
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
