using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmashBros.Model;

namespace SmashBros.Views
{
    class MenuEntry
    {
        public SpriteFont font;
        public String text;
        public Vector2 pos;
        public Texture2D img;
        public Vector2 imgPos;
        public Color color = Color.Aquamarine;
        public string onClick;

        public MenuEntry(SpriteFont font, string text, Vector2 pos, Action onClick)
        {
            this.font = font;
            this.text = text;
            this.pos = pos;
            this.onClick += onClick;
        }
    }
}
