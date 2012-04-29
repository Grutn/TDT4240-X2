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
        private ImageView percentBg;
        private ImageController lifes;
        private ImageView thumb;

        private GameOptions gameOptions;
        private CharacterModel characterModel;
        public PlayerStats PlayerStats;
        
        public PlayerStatsController(ScreenManager screen, CharacterModel characterModel, GameOptions gameOptions)
            :base(screen)
        {
            this.PlayerStats = new PlayerStats(gameOptions.Lifes, characterModel.playerIndex);
            this.characterModel = characterModel;
            this.gameOptions = gameOptions;
        }

        public override void Load(ContentManager content)
        {

            Vector2 percentPos = new Vector2(330 * characterModel.playerIndex + 30, Constants.WindowHeight - 120);
            
            thumb = new ImageView(content.Load<Texture2D>(GamePadControllers[characterModel.playerIndex].PlayerModel.SelectedCharacter.thumbnail),
                percentPos, 159, true);
            thumb.Scale = 0.6f;
            AddView(thumb);


            percentBox = new TextBox("0%", GetFont("Impact.large"), percentPos + new Vector2(180, 40), Color.White, 1f);
            percentBox.StaticPosition = true;
            percentBox.Layer = 150;
            percentBox.Origin = new Vector2(20, 0);
            AddView(percentBox);

            percentBg = new ImageView(content.Load<Texture2D>("GameStuff/PlayerPercentBg"), percentPos + new Vector2(0, 94), 149, true);
            AddView(percentBg);

            if (gameOptions.UseLifes)
            {
                lifes = new ImageController(Screen, "GameStuff/Life", 160, true);
                AddController(lifes);
                for (int i = 0; i < gameOptions.Lifes; i++)
                {
                    lifes.AddPosition(percentPos + new Vector2(i * 16 + 130, 100), 400);
                }
            }
        }

        public void DidHit(int damageDone, int playerIndexReciever)
        {
            PlayerStats.HitsDone++;
            if (PlayerStats.PlayerDamageDone.ContainsKey(playerIndexReciever))
                PlayerStats.PlayerDamageDone[playerIndexReciever] += damageDone;
            else
                PlayerStats.PlayerDamageDone.Add(playerIndexReciever, damageDone);
        }

        public void Killed(int playerIndex)
        {
            if (PlayerStats.PlayerKills.ContainsKey(playerIndex))
                PlayerStats.PlayerKills[playerIndex]++;
            else
                PlayerStats.PlayerKills.Add(playerIndex, 1);
        }

        public void GotHit(int damagePoints, int playerIndex)
        {
            PlayerStats.DamagePoints = damagePoints;
            PlayerStats.LastHitBy = playerIndex;
        }

        /// <summary>
        /// Removes one life for player
        /// and sets the killersIndex
        /// </summary>
        /// <returns><c>true</c>If character has lifes left</returns>
        public bool GotKilled(out int killerIndex)
        {
            if(gameOptions.UseLifes && PlayerStats.LifesLeft != 0){
                PlayerStats.LifesLeft--;
                lifes.RemovePosition(PlayerStats.LifesLeft);
            }
            killerIndex = PlayerStats.LastHitBy;
            return PlayerStats.LifesLeft != 0;
        }

        public override void Unload()
        {
            DisposeController(lifes);
            DisposeView(percentBg, percentBox, thumb);

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
            RemoveView(percentBg);
            RemoveView(thumb);
            RemoveController(lifes);
        }
    }
}
