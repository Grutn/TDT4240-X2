using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.Views
{
    public abstract class IView
    {
        public bool IsActive { get; set; }
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void Dispose();
    }
}
