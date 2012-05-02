using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
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
        int countDownTime = 0, gameOverWait = 3000;
        bool waitForGameOverText = false;
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

        public GamePlayController(ScreenManager screen, Map selectedMap) : base(screen)
        {
            this.players = new Dictionary<int, PlayerStatsController>();
            this.characterControllers = new List<CharacterController>();
            this.map = new MapController(screen, selectedMap);
        }

        private List<PlayerStats> GetGameStats()
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

            if (!Screen.GameOptions.UseLifes)
            {
                timeLeft = new TimeSpan(0, Screen.GameOptions.Minutes, 0);
                timer = new TextBox(getTimeLeft(), FontDefualt, new Vector2(Constants.WindowWidth - 80, 10), Color.White);
                timer.Layer = 150;
                timer.StaticPosition = true;
                AddView(timer);
            }
            
            AddController(camera);
            AddController(this.map);

            SubscribeToGameState = true;
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
                if (pad.PlayerModel.SelectedCharacter != null)
                {
                    var character = new CharacterController(Screen, pad, map.CurrentMap.startingPosition[i], countDownTime);
                    //Add HitHappens listener
                    character.OnHit += OnPlayerHit;
                    // Add Playerdeath listener.
                    character.OnCharacterDeath += OnPlayerDeath;
                    //Adds the controller 
                    AddController(character);

                    characterControllers.Add(character);
                    camera.AddCharacterTarget(character);


                    players.Add(pad.PlayerIndex, new PlayerStatsController(Screen, character.model, Screen.GameOptions));
                    AddController(players.Last().Value);

                    pad.OnStartPress += OnStartPress;
                    i++;
                }

            }
        }

        public override void Unload()
        {
            DisposeController(map, camera, effectImg, countDown, powerUps, gameOver);

            foreach (var c in players)
            {
                DisposeController(c.Value);
            }

            foreach (var character in characterControllers)
            {
                character.OnCharacterDeath -= OnPlayerDeath;
                character.OnHit -= OnPlayerHit;
                DisposeController(character);
            }

            foreach (var pad in GamePadControllers)
            {
                pad.OnStartPress -= OnStartPress;
            }

            RemoveView(timer);

            Screen.ControllerViewManager.camera.ResetCamera();

            System.GC.SuppressFinalize(this);
        }

        public override void Update(GameTime gameTime)
        {
            //If wait for gameovertext = true then game is over, just waiting for gameOver text to animate + som extra time
            if (CurrentState != GameState.GamePause)
            {
                if (!waitForGameOverText)
                {

                    bool gameOver = false;

                    #region Checks if game is over
                    //If uselifes is set, then check if there is only one player left with one life
                    //else check if timeLEft is 0
                    if (Screen.GameOptions.UseLifes)
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
                    #endregion

                    //If gameIsOVer and gameOver not is run yet
                    //Then add the gameover text and animate it in
                    //Game is put in gameover state when animation is done
                    if (gameOver)
                    {
                        Screen.popupMenuController.State = PopupState.Removed;

                        #region Check and sets which player that has won game
                        if (Screen.GameOptions.UseLifes)
                        {
                            //if use lifes it finds the player that has lifes left
                            players.Where(a => a.Value.PlayerStats.LifesLeft != 0).First().Value.PlayerStats.IsWinner = true;
                        }
                        else
                        {
                            int max = 0;
                            List<int> winners = new List<int>();
                            foreach (var p in players)
                            {
                                int kills = p.Value.PlayerStats.PlayerKills.Count();
                                if (kills > max)
                                {
                                    max = kills;
                                    winners = new List<int>() { p.Value.PlayerStats.PlayerIndex };
                                }
                                else if (kills == max)
                                {
                                    winners.Add(p.Value.PlayerStats.PlayerIndex);
                                }
                            }

                            foreach (var p in players)
                            {
                                if (winners.Exists(a => a == p.Value.PlayerStats.PlayerIndex))
                                {
                                    p.Value.PlayerStats.IsWinner = true;
                                }
                            }


                        }
                        #endregion

                        waitForGameOverText = true;

                        //Shows the gameover text
                        AddController(this.gameOver);
                        ImageModel model = this.gameOver.SetPosition(Constants.WindowWidth / 2, Constants.WindowHeight / 2);
                        //Animates it in
                        this.gameOver.AnimateScale(1f, 800, false);

                        //Disables the start menu so you can't press start while the gameovertext shows
                        Screen.popupMenuController.Disabled = true;
                    }
                }
                else
                {
                    //Terminate the controller and starts the menu controller when the gameoverwait is done
                    gameOverWait -= gameTime.ElapsedGameTime.Milliseconds;
                    if (gameOverWait <= 0)
                    {
                        CurrentState = GameState.GameOver;
                        AddController(new MenuController(Screen, GetGameStats()));
                        DisposeController(this);
                    }
                }
            }
           
        }

        #region Observers

        public override void OnNext(GameStateManager value)
        {
            switch (value.PreviousState)
            {
                case GameState.GamePause:
                    characterControllers.ForEach(a => a.Freezed = false);
                    break;
            }

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    DisposeController(this);
                    break;
                case GameState.CharacterMenu:
                    DisposeController(this);
                    break;
                case GameState.MapsMenu:
                    DisposeController(this);
                    break;

                case GameState.GamePause:
                    characterControllers.ForEach(a => a.Freezed = true);
                    break;
            }
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
            Screen.soundController.PlayGameSound(GameSoundType.death);
            Vector2 pos = characterController.model.position;
            int playerIndex = characterController.model.playerIndex;
            
            int gotKilled = characterController.model.playerIndex;
            
            int killer;
            if (players[gotKilled].GotKilled(out killer))
            {
                //Removes powerup from player
                powerUps.RemovePowerUp(gotKilled);
                //Updates killer 
                players[killer].Killed(gotKilled);
                characterController.Reset(map.CurrentMap.startingPosition[resetPos], behindScreen);
                if (resetPos == 3) resetPos = 0;
                else resetPos++;
            }
            else
            {
                camera.RemoveCharacterTarget(characterController);
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
        }
    }

}
