using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmashBros.Model;

namespace SmashBros.Models
{
    public class MenuEntry
    {
        public MenuEntry(string text)
        {
            this.text = text;
            this.color = Color.Black;
            this.selectedColor = Color.White;
            this.selectedSize = 1.3f;
        }

        public String text;
        public Texture2D bgImg;
        public Vector2 bgOffset;
        public Color selectedColor;
        public Color color;
        public float selectedSize;
        public float size;
    }
}
