using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SmashBros.Models;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SmashBros.System;

namespace SmashBros.Views
{
    class MenuView : IView
    {
        int menuWidht = 800;
        int menuHeight = 600;
        List<MenuEntry> entries;
        SpriteFont font;
        public Vector2 StartingPosition;


        public MenuView(SpriteFont font){
            this.entries = new List<MenuEntry>();
            this.font = font;
        }

        public void SetEntries(World world, params MenuEntry[] entries)
        {
            foreach (var e in entries)
            {
                if(e.boundBox != null)
                    e.boundBox.Dispose();
            }

            this.entries = new List<MenuEntry>();
            int i = 0;
            foreach (var e in entries)
            {
                e.textSize = font.MeasureString(e.text);
                e.boundBox = BodyFactory.CreateRectangle(world, 
                    ConvertUnits.ToSimUnits(menuWidht), ConvertUnits.ToSimUnits(60), 1f);
                e.boundBox.CollisionCategories = Category.Cat6;
                e.boundBox.IsSensor = true;
                e.boundBox.Position = ConvertUnits.ToSimUnits(StartingPosition.X+ menuWidht/2 +50, StartingPosition.Y + 70 * i + 40);
                e.boundBox.UserData = e;
                e.entryIndex = i;
                i++;

                this.entries.Add(e);
            }
        }

        public void EmptyEntries()
        {
            this.entries = new List<MenuEntry>();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int i = 0;
            foreach(var entry in entries){
                Color color = entry.color;
                float scale = 1.0f;
                if (entry.selected)
                {
                    color = entry.selectedColor;
                    scale = entry.selectedSize;
                }
                spriteBatch.DrawString(font, entry.text, ConvertUnits.ToDisplayUnits(entry.boundBox.Position) - entry.textSize/2, 
                    color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        public override void Dispose()
        {
            
        }


    }
}
