using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SmashBros.Models;
using Microsoft.Xna.Framework.Content;

namespace SmashBros.Views
{
    public class CharacterView : Sprite
    {
        public CharacterView(ContentManager content, string assetName, int frameWidth, int frameHeight, float xPos, float yPos)
            : base(content, assetName, frameWidth, frameHeight, xPos, yPos)
        {

        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

        }
    }
}
