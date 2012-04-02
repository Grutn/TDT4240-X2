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
        
        public Dictionary<int, Vector2> Positions;
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public ImageTexture(ContentManager content, string assetName)
        {
            img = content.Load<Texture2D>(assetName);
            Positions = new Dictionary<int, Vector2>();
            Scale = 1f;
        }

        public ImageTexture(ContentManager content, string assetName, int xpos, int ypos) 
            :this(content,assetName)
        {
            Positions.Add(0, new Vector2(xpos, ypos));
        }

        public void Position(float x, float y)
        {
            this.Positions[0] = new Vector2(x, y);
        }

        public void AddDrawPosition(int entryId, float x, float y)
        {
            if (Positions.ContainsKey(entryId))
            {
                Positions[entryId] = new Vector2(x, y);
            }else
                Positions.Add(entryId, new Vector2(x, y));
        }

        public void RemoveDrawPosition(int entryId)
        {
            if (Positions.ContainsKey(entryId))
                Positions.Remove(entryId);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (var pos in this.Positions)
            {

//                spriteBatch.Draw(img, pos.Value, null, Color.White, Rotation, Vector2.Zero, Scale, 0, 0f);
                spriteBatch.Draw(img, pos.Value, Color.White);
            }
        }

        public override void Dispose()
        {
            img.Dispose();
        }
    }
}
