using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SmashBros.Models;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SmashBros.MySystem;

namespace SmashBros.Views
{
    class MenuView : IView
    {
        int menuWidht = 800;
        int menuHeight = 400;
        SpriteFont font;
        public List<MenuEntry> Entries;
        public Vector2 StartingPosition;


        public MenuView(SpriteFont font){
            this.Entries = new List<MenuEntry>();
            this.font = font;
        }

        public void SetEntries(World world, params MenuEntry[] entries)
        {
            foreach (var e in this.Entries)
            {
                if(e.boundBox != null)
                    e.boundBox.Dispose();
            }

            this.Entries = new List<MenuEntry>();

            float yPos = StartingPosition.Y + 40;
            int i = 0;

            foreach (var e in entries)
            {
                e.textSize = font.MeasureString(e.text);
                e.boundBox = BodyFactory.CreateRectangle(world,
                    ConvertUnits.ToSimUnits(menuWidht), ConvertUnits.ToSimUnits(50), 1f);
                e.boundBox.CollisionCategories = Category.Cat6;
                e.boundBox.IsSensor = true;
                e.boundBox.IsStatic = true;
                e.boundBox.Position = ConvertUnits.ToSimUnits(StartingPosition.X + menuWidht / 2 + 50, yPos) +
                    ConvertUnits.ToSimUnits(new Vector2(0, menuHeight / 2 - (entries.Count() * 60) / 2));

                e.boundBox.UserData = e;
                e.entryIndex = i;
                i++;
                yPos += e.textSize.Y + 20;

                this.Entries.Add(e);
            }

        }

        public void AddEntries(World world, params MenuEntry[] entries)
        {
            foreach (var e in entries)
	        {
                Entries.Add(e);
	        }

            SetEntries(world, Entries.ToArray());
        }

        public void EmptyEntries()
        {
            this.Entries = new List<MenuEntry>();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int i = 0;
            foreach(var entry in Entries){
                Color color = entry.color;
                float scale = entry.scale;
                if (entry.selected && entry.action != null)
                {
                    color = entry.selectedColor;
                    scale = entry.selectedScale;
                }
                spriteBatch.DrawString(font, entry.text, ConvertUnits.ToDisplayUnits(entry.boundBox.Position) - entry.textSize / 2, 
                    color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        public override void Dispose()
        {
            System.GC.SuppressFinalize(this);   
        }


    }
}
