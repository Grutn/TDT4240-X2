﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Collections.Concurrent;
using SmashBros.Models;
using SmashBros.System;

namespace SmashBros.Views
{
    /// <summary>
    /// ImageTexture is a texture that can be drawn on multiple position
    /// and easily animated
    /// </summary>
    public class ImageView : IView
    {
        public Texture2D Image;
        
        public List<ImageModel> imageModels;
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public Rectangle? FrameRectangle;
        public int FramesPrRow = 0;
        public int PosCount { get { return imageModels.Count(); } }

        public ImageView(List<ImageModel> imageModels)
        {
            this.imageModels = imageModels;
            Scale = 1f;
            Origin = Vector2.Zero;
        }

        public ImageView(Texture2D image, Vector2 position, int layer = 0, bool staticPos = false){
            this.imageModels = new List<ImageModel>();
            this.imageModels.Add(new ImageModel(position));

            this.Image = image;
            this.StaticPosition = staticPos;
            this.Layer = layer;
        }
        

        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            foreach (var i in this.imageModels)
            {
                Rectangle? r = FrameRectangle;

                if (FrameRectangle.HasValue)
                {
                    int row = (int)Math.Floor((double)i.CurrentFrame / this.FramesPrRow);

                    r = new Rectangle(FrameRectangle.Value.Width * (i.CurrentFrame - row * FramesPrRow),
                        row * FrameRectangle.Value.Height, 
                        FrameRectangle.Value.Width, FrameRectangle.Value.Height);
                }

                //Gets the position of image by checking if imagemodel has 
                //bound box that is not static or sensor
                //If not then used defined curent position
                Vector2 pos = i.CurrentPos;
                float rotation = i.Rotation;
                if (i.BoundBox != null && (!i.BoundBox.IsStatic || !i.BoundBox.FixtureList.First().IsSensor))
                {
                    pos = ConvertUnits.ToDisplayUnits(i.BoundBox.Position);
                    rotation = i.BoundBox.Rotation;
                }
                
                spriteBatch.Draw(Image, pos, r, Color.White, rotation, i.Origin, i.CurrentScale, SpriteEffects.None, 0);
            }
        }

        public override void Dispose()
        {
            Image.Dispose();
        }

        
    }
}