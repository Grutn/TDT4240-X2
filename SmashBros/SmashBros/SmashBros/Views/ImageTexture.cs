using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SmashBros.Views
{
    public class ImageTexture : IView
    {
        private Texture2D img;
        private Vector2 pos;
        public ImageTexture(ContentManager content, string assetName, int xpos, int ypos)
        {
            img = content.Load<Texture2D>(assetName);
            pos = new Vector2(xpos, ypos);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Draw(img, pos, Color.White);
        }

        public override void Dispose()
        {
            img.Dispose();
        }
    }
}
