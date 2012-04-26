using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SmashBros.Models;

namespace SmashBros.Views
{
    class MenuView : IView
    {
        List<MenuEntry> entrys;
        public int SelectedIndex = 0;
        SpriteFont font;

        public MenuView(SpriteFont font){
            this.entrys = new List<MenuEntry>();
            this.font = font;
        }

        public void SetEntries(params MenuEntry[] entries)
        {
            this.entrys = entries.ToList();
        }

        public void AddEntry(MenuEntry entry)
        {

        }

        public void EmptyEntries()
        {
            this.entrys = new List<MenuEntry>();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int i = 0;
            foreach(var entry in entrys){
                Color color = entry.color;
                float scale = 1.0f;
                if (i == SelectedIndex)
                {
                    color = entry.selectedColor;
                    scale = entry.selectedSize;
                }
                spriteBatch.DrawString(font, entry.text, Vector2.Zero, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                i++;
            }
        }

        public override void Dispose()
        {
            
        }


    }
}
