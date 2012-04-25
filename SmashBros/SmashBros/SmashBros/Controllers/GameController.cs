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
    class GameController : Controller
    {
        MapController map;
        CameraController camera;
        Dictionary<int,PlayerStats> players;

        SoundEffect sound_background;
        SoundEffectInstance sound_instance;
        SoundEffect sound_hit;
        ImageTexture effectImg;

        public GameController(ScreenController screen, Map selectedMap) : base(screen)
        {
            this.players = new Dictionary<int,PlayerStats>();
            this.map = new MapController(screen, selectedMap);
        }

        public override void Load(ContentManager content)
        {
            sound_hit = content.Load<SoundEffect>("Sound/hit");

            if (Constants.Music)
            {
                sound_background = content.Load<SoundEffect>("Sound/main");
                sound_instance = sound_background.CreateInstance();
                sound_instance.IsLooped = true;
                sound_instance.Play();
            }
            
            this.camera = new CameraController(screen, map.Model.zoomBox);

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
                    var character = new CharacterController(screen, pad, map.CurrentMap.startingPosition[i]);
                    //Add HitHappens listener
                    character.OnHit += OnPlayerHit;
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

        private void OnPlayerHit(Vector2 pos, int damageDone, int newDamagepoints, int puncher, int reciever)
        {
            sound_hit.Play();
            this.players[puncher].DidHit(damageDone, reciever);
            this.players[reciever].GotHit(newDamagepoints);

            Random r = new Random();
           

            ImageInfo inf = effectImg.AddPosition(pos);
            inf.CurrentFrame = r.Next(0, 4);
            inf.StartAnimation(pos- new Vector2(30, 30), 180, false, 0.7f);
        }

        private void OnHitAnimationDone(ImageTexture target, ImageInfo imagePosition)
        {
            target.RemovePosition(imagePosition);
        }

        private void OnPlayerDeath(CharacterController characterController)
        {
            Vector2 pos = characterController.position;
            int playerIndex = characterController.playerIndex;
            Random r = new Random();
            characterController.Reset(map.CurrentMap.startingPosition[r.Next(0 ,4)]);
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



        public void DidHit(int damageDone, int playerIndexReciever){
            HitsDone++;
            if (PlayerDamageDone.ContainsKey(playerIndexReciever))
                PlayerDamageDone[playerIndexReciever] += damageDone;
            else
                PlayerDamageDone.Add(playerIndexReciever, damageDone);
        }

        public void KilledPlayer(int playerIndexKilled)
        {
            if (PlayerKills.ContainsKey(playerIndexKilled))
                PlayerKills[playerIndexKilled]++;
            else
                PlayerKills.Add(playerIndexKilled,1);    
        }

        public void Died()
        {

        }

        public void GotHit(int damagePoints){
            this.DamagePoints = damagePoints;
            this.percentBox.Text = damagePoints + "%";
        }

    }
}
