using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SmashBros.MySystem;
using SmashBros.Views;
using Microsoft.Xna.Framework.Graphics;
using SmashBros.Models;

namespace SmashBros.Controllers
{
    public class PlayerStatsController : Controller
    {
        private TextBox percentBox;
        private TextBox playerText;
        private ImageView percentBg;
        private ImageController lifes;

        private GameOptions gameOptions;
        private CharacterModel characterModel;
        public GameStats GameStats;
        
        public PlayerStatsController(ScreenManager screen, CharacterModel characterModel, GameOptions gameOptions)
            :base(screen)
        {
            this.GameStats = new GameStats();
            this.characterModel = characterModel;
            this.gameOptions = gameOptions;
        }

        public void Dispose()
        {

        }

        public override void Load(ContentManager content)
        {
            
            Vector2 percentPos = new Vector2(300 * characterModel.playerIndex + 70, Constants.WindowHeight - 120);

            percentBox = new TextBox("0%", GetFont("Impact.large"), percentPos + new Vector2(80, 50), Color.Black, 1f);
            percentBox.StaticPosition = true;
            percentBox.Layer = 150;
            percentBox.Origin = new Vector2(20, 0);
            AddView(percentBox);

            TextBox player = new TextBox("Player " + (characterModel.playerIndex + 1), FontDefualt, 
                percentPos + new Vector2(70, 100), Color.White, 0.6f);
            player.StaticPosition = true;
            player.Layer = 150;
            AddView(player);

            ImageView percentBg = new ImageView(content.Load<Texture2D>("GameStuff/PlayerPercentBg"), percentPos, 149, true);
            AddView(percentBg);

            if (gameOptions.UseLifes)
            {

                for (int i = 0; i < gameOptions.Lifes; i++)
                {
                    
                }
            }
        }

        public void DidHit(int damageDone, int playerIndexReciever)
        {
            GameStats.HitsDone++;
            if (GameStats.PlayerDamageDone.ContainsKey(playerIndexReciever))
                GameStats.PlayerDamageDone[playerIndexReciever] += damageDone;
            else
                GameStats.PlayerDamageDone.Add(playerIndexReciever, damageDone);
        }

        public void Killed(int playerIndex)
        {
            if (GameStats.PlayerKills.ContainsKey(playerIndex))
                GameStats.PlayerKills[playerIndex]++;
            else
                GameStats.PlayerKills.Add(playerIndex, 1);
        }

        public void GotHit(int damagePoints, int playerIndex)
        {
            GameStats.DamagePoints = damagePoints;
            GameStats.LastHitBy = playerIndex;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            this.percentBox.Text = characterModel.damagePoints + "%";
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
            RemoveView(percentBox);
            RemoveView(playerText);
            RemoveView(percentBg);
        }
    }
}
