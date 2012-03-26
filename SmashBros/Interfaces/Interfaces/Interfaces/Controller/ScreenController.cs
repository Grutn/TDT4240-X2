using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Interfaces
{
    class ScreenController : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        List<IDrawable> views;
        MenuController menu;

        public ScreenController(Game game)
            : base(game)
        {
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
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.End();

            throw new NotImplementedException();
        }
    }
}
