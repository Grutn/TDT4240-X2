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

namespace SmashBros.Controllers
{
    public class ScreenController : DrawableGameComponent
    {
        MenuController menu;
        
        public List<SpriteFont> fonts;
        public KeyboardState currentKeyboardState;
        public KeyboardState oldKeyboardState;
        public ControllerViewManager controllerViewManager;
        public List<GamepadController> gamePads;
        public GameStateManager gameStateManager;

        
        public ScreenController(Game game)
            : base(game)
        {
            this.gamePads = new List<GamepadController>();   
            this.fonts = new List<SpriteFont>();
            this.menu = new MenuController(this);

            this.gameStateManager = new GameStateManager();
            this.gameStateManager.CurrentState = GameState.StartScreen;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            controllerViewManager = new ControllerViewManager(Game.GraphicsDevice, content);

            content.Load<SpriteFont>("font");

            menu.Init();

            for (int i = 0; i < 4; i++)
            {
                var gamepad = new GamepadController(this, i, menu);
                gamePads.Add(gamepad);
                ControllerViewManager.AddController(gamepad);
            }

            //controllerViewManager.AddController(menu);
            //controllerViewManager.AddController(new GamepadController(this,1));
        }

        public override void Update(GameTime gameTime)
        {
            //Save keyboard state so all controllers can access them
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            controllerViewManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            controllerViewManager.Draw(gameTime);
        }
    }
}
