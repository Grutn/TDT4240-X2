using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.Views
{
    class MenuView : IView
    {
        List<MenuEntry> entrys;
        MenuEntry selected;

        public MenuView(List<MenuEntry> entrys){
            this.entrys = entrys;
        }

        public void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach(var entry in entrys){
                Color color = entry.color;
                float scale = 1.0f;
                if (entry.Equals(selected))
                {
                    color = Color.Red;
                    scale = 1.2f;
                }
                spriteBatch.DrawString(entry.font, entry.text, entry.pos, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }


    }
}
