using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.Views
{
    public interface IView
    {
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
