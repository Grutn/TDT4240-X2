using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.MySystem;
using SmashBros.Model;
using SmashBros.Views;
using SmashBros.Models;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace SmashBros.Controllers
{
    class PowerUpController : Controller
    {
        int waitMax = 7, minNew = 15, maxNew = 30;
        List<PowerUp> powerUps;
        //List with powerups that is picked up, key is playerindex
        Dictionary<int, PowerUpStatus> activePowerUps;
        //List with powerups that are lying on map
        List<PowerUpStatus> waitingPowerUps;

        ImageController powerUpImg;
        float elapsedTime = 0, timeToNext;
        Box dropZone;
        Random random;
        

        public PowerUpController(ScreenManager screen, Box dropZone)
            :base(screen)
        {
            this.random = new Random();
            this.dropZone = dropZone;
            this.activePowerUps = new Dictionary<int, PowerUpStatus>();
            this.waitingPowerUps = new List<PowerUpStatus>();
        }

        public override void Load(ContentManager content)
        {
            powerUps = Serializing.LoadPowerUps();
            powerUpImg = new ImageController(Screen, "GameStuff/PowerUps", 120, false);
            powerUpImg.FrameRectangle = new Rectangle(0, 0, 60, 60);
            powerUpImg.FramesPerRow = 5;
            powerUpImg.ScaleDefault = 0;
            AddController(powerUpImg);

            generateTimeToNext();
        }

        public override void Unload()
        {
            DisposeController(powerUpImg);
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime / 1000 > random.Next(minNew, maxNew))
            {
                ImageModel model = powerUpImg.AddPosition(randomDropPosition());
                model.SetBoundBox(World, 60, 60, Vector2.Zero, Category.Cat6, Category.All);
                model.BoundBox.OnCollision += OnCollision;
                powerUpImg.AnimateScale(model, 1f, 400, false);
                elapsedTime = 0;
                generateTimeToNext();

                PowerUpStatus status = new PowerUpStatus(randomPowerUp(), model);
                model.BoundBox.UserData = status;
                model.CurrentFrame = status.PowerUp.imageFrame;
                waitingPowerUps.Add(status);
            }

            int count = waitingPowerUps.Count();
            for (int i = 0; i < count; i++)
			{
                PowerUpStatus p = waitingPowerUps[i];
			    p.ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (p.ElapsedTime / 1000 > waitMax)
                {
                    waitingPowerUps.RemoveAt(i);
                    p.Image.DisposBoundBox();
                    powerUpImg.RemovePosition(p.Image);
                    count--;
                }
			}

            count = activePowerUps.Count();
            for (int i = 0; i < count; i++)
            {
                var p = activePowerUps.ElementAt(i);
                p.Value.ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (p.Value.ElapsedTime / 1000 > p.Value.PowerUp.duration)
                {
                    p.Value.Player.powerUp = null;
                    activePowerUps.Remove(p.Key);
                    count--;
                }
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
                PowerUpStatus powerUpStatus = (PowerUpStatus)powerUpGeom.Body.UserData;
                waitingPowerUps.Remove(powerUpStatus);

                CharacterModel player = (CharacterModel)character.Body.UserData;
                player.powerUp = powerUpStatus.PowerUp;

                //Setup the powerupstatus for the player
                powerUpStatus.ElapsedTime = 0;
                powerUpStatus.Player = player;

                //Removes the powerup from screen
                powerUpStatus.Image.DisposBoundBox();
                powerUpImg.RemovePosition(powerUpStatus.Image);

                if (activePowerUps.ContainsKey(player.playerIndex))
                    activePowerUps[player.playerIndex] = powerUpStatus;
                else activePowerUps.Add(player.playerIndex, powerUpStatus);

                return false;
            }

            return true;
        }

        public override void Deactivate()
        {
        }
    }
}
