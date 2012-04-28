using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.Model;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SmashBros.MySystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using SmashBros.Models;
using Microsoft.Xna.Framework.Graphics;

namespace SmashBros.Controllers
{
    /// <summary>
    /// Start and controlls the main gameplay
    /// </summary>
    public class GamePlayController : Controller
    {
        int countDownTime = 5;
        bool waitForGameOverText = false;
        GameOptions gameOptions;
        MapController map;
        CameraController camera;
        Dictionary<int,PlayerStatsController> players;
        List<CharacterController> characterControllers;
        ImageController effectImg;
        ImageController countDown;
        ImageController gameOver;
        TextBox timer;
        PowerUpController powerUps;
        TimeSpan timeLeft;

        int resetPos = 0;

        public GamePlayController(ScreenManager screen, Map selectedMap, GameOptions gameOptions) : base(screen)
        {
            this.gameOptions = gameOptions;
            this.players = new Dictionary<int, PlayerStatsController>();
            this.characterControllers = new List<CharacterController>();
            this.map = new MapController(screen, selectedMap);
        }

        public List<PlayerStats> GetGameStats()
        {
            List<PlayerStats> gameStats = new List<PlayerStats>();
            foreach (var stats in players)
            {
                gameStats.Add(stats.Value.PlayerStats);
            }

            return gameStats;
        }

        public override void Load(ContentManager content)
        {

            Screen.soundController.LoadGameSounds(content, this, map.CurrentMap.backgroundMusic);
            
            this.camera = new CameraController(Screen, map.Model.zoomBox);

            //The effects image for hits
            this.effectImg = new ImageController(Screen, "GameStuff/GameEffects", 110);
            this.effectImg.OriginDefault = new Vector2(130 / 2, 130 / 2);
            this.effectImg.ScaleDefault = 0.1f;
            this.effectImg.OnAnimationDone += OnHitAnimationDone;
            this.effectImg.SetFrameRectangle(200, 200);
            this.effectImg.FramesPerRow = 2;
            AddController(effectImg);

            //Creates the countdown before starting game
            countDown = new ImageController(Screen, "GameStuff/CountDown", 900, true);
            countDown.OriginDefault = new Vector2(150, 150);
            countDown.ScaleDefault = 0;
            countDown.SetFrameRectangle(300, 300);
            countDown.FramesPerRow = 2;
            var imgModel = countDown.SetPosition(Constants.WindowWidth / 2, Constants.WindowHeight / 2);
            //Set callback to determin when the game is finished
            imgModel.Callback = countDownNext;
            countDown.AnimateScale(0.7f, (countDownTime/4) * 1000, true);
            AddController(countDown);

            gameOver = new ImageController(Screen, "GameStuff/GameOver", 150, true);
            gameOver.OriginDefault = new Vector2(300, 100);
            gameOver.ScaleDefault = 0;

            this.powerUps = new PowerUpController(Screen, map.Model.DropZone);
            AddController(powerUps);

            //Creates characterController, and PlayerStatsController
            createCharacters(content);

            if (!gameOptions.UseLifes)
            {
                timeLeft = new TimeSpan(0, gameOptions.Minutes,0);
                timer = new TextBox(getTimeLeft(), FontDefualt, new Vector2(Constants.WindowHeight - 10, 10), Color.White);
                timer.Layer = 150;
                timer.StaticPosition = true;
                AddView(timer);
            }
            
            World.Gravity = new Vector2(0, Constants.GamePlayGravity);
            AddController(camera);
            AddController(this.map);
        }

        private string getTimeLeft()
        {
            return timeLeft.ToString("mm") + ":" + timeLeft.ToString("ss");
        }

        private void createCharacters(ContentManager content)
        {
            //Loops through the gamepads to check which controllers that has selected characters
            //And crates CharacterCOntrooles for them
            int i = 0;
            foreach (var pad in GamePadControllers)
            {
                if (pad.SelectedCharacter != null)
                {
                    var character = new CharacterController(Screen, pad, map.CurrentMap.startingPosition[i]);
                    //Add HitHappens listener
                    character.OnHit += OnPlayerHit;
                    // Add Playerdeath listener.
                    character.OnCharacterDeath += OnPlayerDeath;
                    //Adds the controller 
                    AddController(character);

                    characterControllers.Add(character);
                    camera.AddCharacterTarget(character);


                    players.Add(pad.PlayerIndex, new PlayerStatsController(Screen, character.model, gameOptions));
                    AddController(players.Last().Value);

                    pad.OnStartPress += OnStartPress;
                    i++;
                }

            }
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            bool gameOver = false;
            if (gameOptions.UseLifes)
            {
                //Checks if all except one player is dead
                int deadPlayers = 0;
                foreach (var stats in players)
                {
                    if (stats.Value.PlayerStats.LifesLeft == 0)
                    {
                        deadPlayers++;
                    }
                }

                gameOver = deadPlayers == players.Count() - 1;

            }
            else
            {
                timeLeft = timeLeft - new TimeSpan(0, 0, 0, 0, gameTime.ElapsedGameTime.Milliseconds);
                timer.Text = getTimeLeft();

                if (timeLeft.Minutes <= 0 && timeLeft.Seconds <= 0)
                {
                    gameOver = true;
                }
            }

            DebugWrite("GameOver", gameOver);

            //If gameIsOVer and gameOver not is run yet
            //Then add the gameover text and animate it in
            //Game is put in gameover state when animation is done
            if (gameOver && !waitForGameOverText)
            {
                waitForGameOverText = true;
                AddController(this.gameOver);
                ImageModel model = this.gameOver.SetPosition(Constants.WindowWidth / 2, Constants.WindowHeight / 2);
                model.Callback = OnGameOverDone;
                this.gameOver.AnimateScale(1f, 800, false);
            }
        }

        #region Observers

        private void OnGameOverDone(ImageModel model)
        {
            CurrentState = GameState.GameOver;
        }

        public override void OnNext(GameStateManager value)
        {
        }

        private void OnPlayerHit(Vector2 pos, int damageDone, int newDamagepoints, int puncher, int reciever, GameSoundType soundtype)
        {
            Screen.soundController.PlayGameSound(soundtype);

            this.players[puncher].DidHit(damageDone, reciever);
            this.players[reciever].GotHit(newDamagepoints, puncher);

            Random r = new Random();


            ImageModel inf = effectImg.AddPosition(pos);
            inf.CurrentFrame = r.Next(0, 4);
            effectImg.AnimateScale(inf, 0.7f, 200);
        }

        private void OnHitAnimationDone(ImageController target, ImageModel imagePosition)
        {
            target.RemovePosition(imagePosition);
        }

        private void OnPlayerDeath(CharacterController characterController, bool behindScreen)
        {
            Screen.soundController.PlayGameSound(GameSoundType.death);//behindScreen? GameSoundType.deathFar : GameSoundType.death);
            Vector2 pos = characterController.model.position;
            int playerIndex = characterController.model.playerIndex;
            // lifes--
            // if lifes > 0
            
            // else unload characterController
            int gotKilled = characterController.model.playerIndex;
            
            int killer;
            if (players[gotKilled].GotKilled(out killer))
            {
                players[killer].Killed(gotKilled);
                characterController.Reset(map.CurrentMap.startingPosition[resetPos], behindScreen);
                if (resetPos == 3) resetPos = 0;
                else resetPos++;
            }
            else
            {
                RemoveController(characterController);
            }


        }

        private void OnStartPress(int playerIndex)
        {
            switch (CurrentState)
            {
                case GameState.GamePlay:
                    CurrentState = GameState.GamePause;
                    break;
                case GameState.GamePause:
                    CurrentState = GameState.GamePlay;
                    break;
            }
        } 

        #endregion

        private void countDownNext(ImageModel imgModel)
        {
            if (imgModel.CurrentFrame == 3)
            {
                imgModel.AnimationOn = false;
                RemoveController(countDown);
            }
            else
            {
                if (imgModel.CurrentFrame == 2)
                {
                    imgModel.EndScale = 1.1f;
                }
                imgModel.CurrentFrame++;
            }
        }

        public override void Deactivate()
        {
            RemoveController(map);
            RemoveController(camera);
            RemoveController(effectImg);
            RemoveController(countDown);
            RemoveController(powerUps);
            RemoveController(gameOver);

            Screen.ControllerViewManager.camera.ResetCamera();

            foreach (var c in players)
            {
                RemoveController(c.Value);
            }

            foreach (var c in characterControllers)
            {
                RemoveController(c);
            }

            System.GC.SuppressFinalize(this);
        }
    }

}
