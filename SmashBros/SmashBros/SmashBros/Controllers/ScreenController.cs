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
        public KeyboardState currentKey;
        public KeyboardState oldSate;

        public ScreenController(Game game)
            : base(game)
        {
            this.views = new List<IView>();
            this.fonts = new List<SpriteFont>();
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            content.Load<SpriteFont>("font");
            content.Load<SpriteFont>("bigFont");

            throw new NotImplementedException();
        }

        protected override void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
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
            throw new NotImplementedException();
        }
    }
}
