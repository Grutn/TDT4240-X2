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

    /// <summary>
    /// Holds and controls all powerUps on map and drops powerups at random time at random position 
    /// inside the dropZone
    /// </summary>
    class PowerUpController : Controller
    {
        //WaitMax=how long poweupsWaits, minNew/maxNew = min/max time before new poweupShows ups
        int waitMax = 7, minNew = 12, maxNew = 30;
        List<PowerUp> powerUps;
        //List with powerups that is picked up, key is playerindex
        Dictionary<int, PowerUpStatus> activePowerUps;
        //List with powerups that are lying on map
        List<PowerUpStatus> waitingPowerUps;

        ImageController powerUpImg;
        float elapsedTime = 0, timeToNext;
        Box dropZone;
        Random random;
        
        /// <summary>
        /// Constructs the powerup controller  
        /// </summary>
        /// <param name="dropZone">dropZone for powerups</param>
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
            powerUpImg.FrameRectangle = new Rectangle(0, 0, 50, 50);
            powerUpImg.OriginDefault = new Vector2(25, 25);
            powerUpImg.FramesPerRow = 5;
            powerUpImg.ScaleDefault = 0;
            AddController(powerUpImg);

            generateTimeToNext();

        }

        public override void Unload()
        {
            DisposeController(powerUpImg);
            System.GC.SuppressFinalize(this);
        }

        public override void Update(GameTime gameTime)
        {
            //if game is paused then no updates
            if (CurrentState != GameState.GamePause)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime / 1000 > random.Next(minNew, maxNew))
                {
                    addPowerUpToMap();
                }
                
                //Updates the powerups that havent been picked up yet
                //Removes the powerups that has layn ther to long
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

                //Updates the powerup that players has, remove then when time left if 0
                count = activePowerUps.Count();
                for (int i = 0; i < count; i++)
                {
                    var p = activePowerUps.ElementAt(i);
                    p.Value.ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (p.Value.ElapsedTime / 1000 > p.Value.PowerUp.duration)
                    {
                        p.Value.Player.RemovePowerUp(p.Value.PowerUp);
                        activePowerUps.Remove(p.Key);
                        count--;
                    }
                }
            }
        }

        /// <summary>
        /// Adds new random powerup to the map insise the drop zone
        /// </summary>
        public void addPowerUpToMap()
        {
            ImageModel model = powerUpImg.AddPosition(randomDropPosition());
            model.SetBoundBox(World, 50, 50, Vector2.Zero, Category.Cat6, Category.All);
            model.BoundBox.OnCollision += OnCollision;
            powerUpImg.AnimateScale(model, 1f, 400, false);
            elapsedTime = 0;
            generateTimeToNext();

            PowerUpStatus status = new PowerUpStatus(randomPowerUp(), model);
            model.BoundBox.UserData = status;
            model.CurrentFrame = (int)MathHelper.Clamp(status.PowerUp.imageFrame, 0f, 3f);
            waitingPowerUps.Add(status);
        }

        /// <summary>
        /// Create and returns random position inside the dropZone
        /// </summary>
        /// <returns></returns>
        private Vector2 randomDropPosition()
        {
            return new Vector2((float)(dropZone.X + random.NextDouble() * dropZone.Width),
                (float)(dropZone.Y + random.NextDouble() * dropZone.Height));
        }

        /// <summary>
        /// Returns random powerup from the list
        /// </summary>
        /// <returns></returns>
        private PowerUp randomPowerUp()
        {
            return powerUps[random.Next(0, powerUps.Count() - 1)];
        }

        /// <summary>
        /// Set random time until next powerUp is droped
        /// uses min and maxNew as upper & lower limitS
        /// </summary>
        private void generateTimeToNext()
        {
            timeToNext = random.Next(minNew,maxNew);
        }

        public override void OnNext(GameStateManager value)
        {
        }

        /// <summary>
        /// Runs when a character collides with a powerUp
        /// </summary>
        public bool OnCollision(Fixture powerUpGeom, Fixture character, Contact contant)
        {
            if (character.CollisionCategories == Category.Cat11)
            {
                //Gets the powerupstatus from geomData
                PowerUpStatus powerUpStatus = (PowerUpStatus)powerUpGeom.Body.UserData;
                waitingPowerUps.Remove(powerUpStatus);

                //Gets the charactercontroller
                CharacterController player = (CharacterController)character.Body.UserData;
                player.AddPowerUp(powerUpStatus.PowerUp);

                //Setup the powerupstatus for the player
                powerUpStatus.ElapsedTime = 0;
                powerUpStatus.Player = player;

                //Removes the powerup from screen
                powerUpStatus.Image.DisposBoundBox();
                powerUpImg.RemovePosition(powerUpStatus.Image);

                //Sets the active powerup on the player, so it's removed by the update when time is up
                if (activePowerUps.ContainsKey(player.model.playerIndex))
                    activePowerUps[player.model.playerIndex] = powerUpStatus;
                else activePowerUps.Add(player.model.playerIndex, powerUpStatus);

                return false;
            }

            return true;
        }

        internal void RemovePowerUp(int gotKilled)
        {
            if (activePowerUps.ContainsKey(gotKilled))
            {
                activePowerUps[gotKilled].Player.RemovePowerUp(activePowerUps[gotKilled].PowerUp);
            }
        }
    }
}
