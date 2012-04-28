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
        GameOptions gameOptions;
        MapController map;
        CameraController camera;
        Dictionary<int,PlayerStatsController> players;
        ImageController effectImg;
        ImageController countDown;
        PowerUpController powerUps;

        int resetPos = 0;

        public GamePlayController(ScreenManager screen, Map selectedMap, GameOptions gameOptions) : base(screen)
        {
            this.gameOptions = gameOptions;
            this.players = new Dictionary<int, PlayerStatsController>();
            this.map = new MapController(screen, selectedMap);
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

            countDown = new ImageController(Screen, "GameStuff/CountDown", 900, true);
            countDown.OriginDefault = new Vector2(150, 150);
            countDown.ScaleDefault = 0;
            countDown.SetFrameRectangle(300, 300);
            countDown.FramesPerRow = 2;
            var imgModel = countDown.SetPosition(Constants.WindowWidth / 2, Constants.WindowHeight / 2);
            imgModel.Callback = countDownNext;
            countDown.AnimateScale(0.7f, (countDownTime/4) * 1000, true);
            AddController(countDown);

            this.powerUps = new PowerUpController(Screen, map.Model.DropZone);
            AddController(powerUps);

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
                    this.camera.AddCharacterTarget(character);


                    players.Add(pad.PlayerIndex, new PlayerStatsController(Screen, character.model, gameOptions));
                    AddController(players.Last().Value);
                   // this.players.Add(pad.PlayerIndex, new PlayerStats(character, percent, player, percentBg));

                    pad.OnStartPress += OnStartPress;
                    i++;
                }

            }
            
            World.Gravity = new Vector2(0, Constants.GamePlayGravity);
            AddController(camera);
            AddController(this.map);
        }

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

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Deactivate()
        {
            RemoveController(map);
            RemoveController(effectImg);
            RemoveController(powerUps);
            
            foreach (var c in players)
            {
                RemoveController(c.Value);
            }
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
            characterController.Reset(map.CurrentMap.startingPosition[resetPos], behindScreen);
            if (resetPos == 3) resetPos = 0;
            else resetPos++;
            // else unload characterController
            int gotKilled = characterController.model.playerIndex;
            int killer = players[gotKilled].GameStats.LastHitBy;
            players[killer].Killed(characterController.model.playerIndex);
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
    }

}
