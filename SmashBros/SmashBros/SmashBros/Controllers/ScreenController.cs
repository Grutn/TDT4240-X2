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

namespace SmashBros.Controllers
{
    public class ScreenController : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        MenuController menu;

        public List<IView> views;
        public List<SpriteFont> fonts;
        public World world;
        public KeyboardState currentKeyboardState;
        public KeyboardState oldKeyboardState;
        Texture2D t;
        public ScreenController(Game game)
            : base(game)
        {
            this.views = new List<IView>();
            this.fonts = new List<SpriteFont>();
            this.menu = new MenuController(this);
            this.world = new World(Vector2.Zero);
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            content.Load<SpriteFont>("font");

            t = content.Load<Texture2D>("StartScreen");
            menu.Load(content);
            //content.Load<SpriteFont>("bigFont");
        }

        protected override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            menu.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var view in views)
            {
                view.Draw(spriteBatch, gameTime);
            }
            spriteBatch.End();
        }
    }
}
