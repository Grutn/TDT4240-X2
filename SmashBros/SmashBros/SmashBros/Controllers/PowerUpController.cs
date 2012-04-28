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
using FarseerPhysics.Dynamics.Contacts;

namespace SmashBros.Controllers
{
    class PowerUpController : Controller
    {
        List<PowerUp> powerUps;
        Dictionary<int, PowerUp> activePowerUps;
        ImageController powerUpImg;
        float elapsedTime = 0, timeToNext;
        Box dropZone;
        Random random;
        

        public PowerUpController(ScreenManager screen, Box dropZone)
            :base(screen)
        {
            this.random = new Random();
            this.dropZone = dropZone;
            this.activePowerUps = new Dictionary<int, PowerUp>();
        }

        public override void Load(ContentManager content)
        {
            powerUps = Serializing.LoadPowerUps();
            powerUpImg = new ImageController(Screen, "GameStuff/PowerUps", 120, false);
            powerUpImg.FrameRectangle = new Rectangle(0, 0, 60, 60);
            powerUpImg.ScaleDefault = 0;
            AddController(powerUpImg);

            generateTimeToNext();
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime / 1000 > 3)
            {
                ImageModel model = powerUpImg.AddPosition(randomDropPosition());
                model.SetBoundBox(World, 60, 60, Vector2.Zero, Category.Cat6, Category.All);
                model.BoundBox.OnCollision += OnCollision;
                model.BoundBox.UserData = randomPowerUp();
                powerUpImg.AnimateScale(model, 1f, 400, false);
                elapsedTime = 0;
                generateTimeToNext();
            }
        }

        private Vector2 randomDropPosition()
        {
            return new Vector2((float)(dropZone.X + random.NextDouble() * dropZone.Width),
                (float)(dropZone.Y + random.NextDouble() * dropZone.Height));
        }

        private PowerUp randomPowerUp()
        {
            return powerUps[random.Next(0, powerUps.Count() - 1)];
        }

        private void generateTimeToNext()
        {
            timeToNext = random.Next(1, 9);
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public bool OnCollision(Fixture powerUpGeom, Fixture character, Contact contant)
        {
            if (character.CollisionCategories == Category.Cat11)
            {
                int playerIndex = 0;
                //int playerIndex = (int)character.Body.UserData;
                //GamePadControllers[playerIndex].SelectedCharacter
                PowerUp powerUp = (PowerUp)powerUpGeom.Body.UserData;

                return false;
            }

            return true;
        }

        public override void Deactivate()
        {
        }
    }
}
