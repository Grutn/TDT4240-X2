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
using SmashBros.System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace SmashBros.Controllers
{
    /// <summary>
    /// Start and controlls the main gameplay
    /// </summary>
    public class GamePlayController : Controller
    {
        MapController map;
        CameraController camera;
        Dictionary<int,PlayerStats> players;

        ImageTexture effectImg;
        private int resetPos = 0; 

        public GamePlayController(ScreenManager screen, Map selectedMap) : base(screen)
        {
            this.players = new Dictionary<int,PlayerStats>();
            this.map = new MapController(screen, selectedMap);
        }

        public override void Load(ContentManager content)
        {

            Screen.soundController.LoadGameSounds(content, this, map.CurrentMap.backgroundMusic);
            
            this.camera = new CameraController(Screen, map.Model.zoomBox);

            //The effects image for hits
            this.effectImg = new ImageTexture(content, "GameStuff/GameEffects");
            this.effectImg.Layer = 110;
            this.effectImg.Scale = 0.1f;
            this.effectImg.OnAnimationDone += OnHitAnimationDone;
            this.effectImg.SourceRect = new Rectangle(0, 0, 200, 200);
            this.effectImg.FramesPrRow = 2;
            this.AddView(effectImg);

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

                    Vector2 percentPos = new Vector2( 300 * pad.PlayerIndex +70, Constants.WindowHeight -120); 

                    TextBox percent = new TextBox("0%", GetFont("Impact.large"), percentPos + new Vector2(80,50), Color.Black, 1f);
                    percent.StaticPosition = true;
                    percent.Layer = 150;
                    percent.Origin = new Vector2(20, 0);
                    AddView(percent);

                    TextBox player = new TextBox("Player " + (pad.PlayerIndex + 1), FontDefualt, percentPos + new Vector2(70, 100), pad.PlayerModel.Color, 0.6f);
                    player.StaticPosition = true;
                    player.Layer = 150;
                    AddView(player);

                    ImageTexture percentBg = new ImageTexture(content, "GameStuff/PlayerPercentBg", percentPos);
                    percentBg.StaticPosition = true;
                    percentBg.Layer = 149;
                    AddView(percentBg);

                    this.camera.AddCharacterTarget(character);
                    this.players.Add(pad.PlayerIndex, new PlayerStats(character, percent, player, percentBg));

                    pad.OnStartPress += OnStartPress;
                    i++;
                }

            }
            
            World.Gravity = new Vector2(0, Constants.GamePlayGravity);
            AddController(camera);
            AddController(this.map);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Deactivate()
        {
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
           

            ImageInfo inf = effectImg.AddPosition(pos);
            inf.CurrentFrame = r.Next(0, 4);
            inf.StartAnimation(pos- new Vector2(30, 30), 180, false, 0.7f);
        }

        private void OnHitAnimationDone(ImageTexture target, ImageInfo imagePosition)
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
            int killer = players[gotKilled].LastHitBy;
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

    internal class PlayerStats
    {
        public PlayerStats(CharacterController controller, TextBox percentText, TextBox playerText, ImageTexture percentBg)
        {

            this.PlayerDamageDone = new Dictionary<int, int>();
            this.PlayerKills = new Dictionary<int, int>();
            this.Controller = controller;
            this.percentBox = percentText;
            this.percentBg = percentBg;
            this.playerText = playerText;
        }

        public void Dispose()
        {

        }

        private TextBox percentBox;
        private TextBox playerText;
        private ImageTexture percentBg;

        public CharacterController Controller { get; private set; }
        public Dictionary<int,int> PlayerDamageDone { get; private set; }
        public Dictionary<int,int> PlayerKills { get; private set; }
        public int DamageDone { get { return PlayerDamageDone.Sum(a => a.Value); } }
        public int DamagePoints { get; set; }
        public int HitsDone { get; private set; }
        public int LastHitBy { get; private set; }



        public void DidHit(int damageDone, int playerIndexReciever){
            HitsDone++;
            if (PlayerDamageDone.ContainsKey(playerIndexReciever))
                PlayerDamageDone[playerIndexReciever] += damageDone;
            else
                PlayerDamageDone.Add(playerIndexReciever, damageDone);
        }

        public void Killed(int playerIndex)
        {
            if (PlayerKills.ContainsKey(playerIndex))
                PlayerKills[playerIndex]++;
            else
                PlayerKills.Add(playerIndex,1);    
        }

        public void GotHit(int damagePoints, int playerIndex){
            this.DamagePoints = damagePoints;
            this.percentBox.Text = damagePoints + "%";
            this.LastHitBy = playerIndex;
        }
    }
}
