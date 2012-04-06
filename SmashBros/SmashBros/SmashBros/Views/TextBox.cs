using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.Views
{
    class TextBox : IView
    {
        public TextBox()
        {

        }

        public TextBox(string text, SpriteFont font, float x, float y, Color color, float scale = 1f)
        {
            this.Text = text;
            this.Font = font;
            this.Position = new Vector2(x, y);
            this.TextColor = color;
            this.Scale = scale;
            this.BackgroundOffset = Vector2.Zero;
        }

        public string Text { get; set; }
        public Color TextColor { get; set; }
        public float Scale { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Texture2D TextBackground { get; set; }
        public Vector2 BackgroundOffset { get; set; }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (TextBackground != null)
            {
                spriteBatch.Draw(TextBackground, Position + BackgroundOffset, Color.White);
            }
            spriteBatch.DrawString(Font, Text, Position, TextColor, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }

        public override void Dispose()
        {
        }
    }
}
