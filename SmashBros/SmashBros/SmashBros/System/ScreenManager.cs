using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using SmashBros.System;
using FarseerPhysics.DebugViews;
using FarseerPhysics;
using System.Threading;
using SmashBros.Model;
using SmashBros.Controllers;

namespace SmashBros.System
{
    public class ScreenManager : DrawableGameComponent
    {
        MenuController menu;
        
        public Dictionary<string,SpriteFont> fonts;
        public KeyboardState currentKeyboardState;
        public KeyboardState oldKeyboardState;
        public ControllerViewManager ControllerViewManager;
        public List<GamepadController> gamePads;
        public GameStateManager gameStateManager;
        public SoundController soundController;

        
        public ScreenManager(Game game)
            : base(game)
        {
            this.gamePads = new List<GamepadController>();
            this.fonts = new Dictionary<string, SpriteFont>();

            this.gameStateManager = new GameStateManager();
            this.gameStateManager.CurrentState = GameState.StartScreen;
            this.soundController = new SoundController(this);
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            ControllerViewManager = new ControllerViewManager(Game.GraphicsDevice, content);

            fonts.Add("Impact", content.Load<SpriteFont>("Fonts/Impact"));
            fonts.Add("Impact.large", content.Load<SpriteFont>("Fonts/Impact.large"));


            List<Player> players = Serializing.LoadPlayerControllers();
            foreach (Player player in players)
            {
                GamepadController gamepad = new GamepadController(this, player);
                gamePads.Add(gamepad);
                ControllerViewManager.AddController(gamepad);
            }

            if (Constants.StartGameplay)
            {
                gameStateManager.CurrentState = GameState.GamePlay;
                var chars = Serializing.LoadCharacters();
                var maps = Serializing.LoadMaps();
                gamePads[0].SelectedCharacter = chars[0];
                gamePads[1].SelectedCharacter = chars[2];

                GamePlayController game = new GamePlayController(this, maps[0]);
                ControllerViewManager.AddController(game);
            }
            else
            {
                gameStateManager.CurrentState = GameState.CharacterMenu;
                this.menu = new MenuController(this);
                ControllerViewManager.AddController(menu);
            }
        }

        float elapsedTime = 0;
        public override void Update(GameTime gameTime)
        {
            if (Constants.DebugMode)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= 800)
                {
                    Serializing.Reload();
                    elapsedTime = 0;
                }

                if (currentKeyboardState.IsKeyUp(Keys.D6) && oldKeyboardState.IsKeyDown(Keys.D6))
                {
                    Serializing.Reload();
                }
            }
            //Save keyboard state so all controllers can access them
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            ControllerViewManager.Update(gameTime);

           
        }

        public override void Draw(GameTime gameTime)
        {
            ControllerViewManager.Draw(gameTime);
        }
    }
}
