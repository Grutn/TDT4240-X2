using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.System;
using SmashBros.Model;
using SmashBros.Views;
using SmashBros.Models;
using FarseerPhysics.Dynamics;

namespace SmashBros.Controllers
{
    class PowerUpController : Controller
    {
        List<PowerUp> powerUps;
        ImageController powerUpImg;
        float elapsedTime = 0;
        Box dropZone;
        Random random;

        public PowerUpController(ScreenManager screen, Box dropZone)
            :base(screen)
        {
            this.random = new Random();
            this.dropZone = dropZone;
        }

        public override void Load(ContentManager content)
        {
            powerUps = Serializing.LoadPowerUps();
            powerUpImg = new ImageController(Screen, "GameStuff/PowerUps", 120, false);
            powerUpImg.FrameRectangle = new Rectangle(0, 0, 60, 60);
            AddController(powerUpImg);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime / 1000 > 3)
            {
                ImageModel model = powerUpImg.AddPosition(createDropPosition());
                model.SetBoundBox(World, 60, 60, Vector2.Zero, Category.Cat6, Category.All);
                elapsedTime = 0;
            }
        }

        private Vector2 createDropPosition()
        {
            return new Vector2((float)(dropZone.X + random.NextDouble() * dropZone.Width),
                (float)(dropZone.Y + random.NextDouble() * dropZone.Height));
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }
    }
}
