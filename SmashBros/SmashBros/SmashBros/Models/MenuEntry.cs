using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace SmashBros.Models
{
    public class MenuEntry
    {
        public MenuEntry(string text, Action<int> action)
        {
            this.text = text;
            this.color = Color.Black;
            this.selectedColor = Color.White;
            this.selectedScale = 1.1f;
            this.scale = 1.0f;
            this.action = action;
        }

        public String text;
        public Texture2D bgImg;
        public Vector2 bgOffset;
        public Color selectedColor;
        public Color color;
        public float selectedScale;
        public float scale;
        public bool selected;
        public Body boundBox;
        public Vector2 textSize;
        
        public Action<int> action;
        public int entryIndex;
    }
}
