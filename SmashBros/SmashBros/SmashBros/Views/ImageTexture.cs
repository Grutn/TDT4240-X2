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
    /// <summary>
    /// ImageTexture is a texture that can be drawn on multiple position
    /// and easily animated
    /// </summary>
    public class ImageTexture : IView
    {
        private Texture2D img;
        
        private Dictionary<int, ImageInfo> images;

        public float Rotation { get; set; }
        public float Scale { get; set; }
        public int ImagesCount { get { return images.Count(); } }

        public ImageTexture(ContentManager content, string assetName)
        {
            img = content.Load<Texture2D>(assetName);
            images = new Dictionary<int, ImageInfo>();
            Scale = 1f;
        }

        public ImageTexture(ContentManager content, string assetName, float xpos, float ypos) 
            :this(content,assetName)
        {
            images.Add(0, new ImageInfo(xpos, ypos));
        }

        public void Position(float x, float y)
        {
            if(ImagesCount > 0)
                this.images.First().Value.Current = new Vector2(x, y);
        }

        public void Position(Vector2 pos)
        {
            Position(pos.X, pos.Y);
        }

        public void AddDrawPosition(int entryId, float x, float y)
        {
            if (images.ContainsKey(entryId))
            {
                images[entryId].Current = new Vector2(x, y);
            }else
                images.Add(entryId, new ImageInfo(x,y));
        }

        public bool HasAnnimation(string name, int posIndex = 0)
        {
            return images.ContainsKey(posIndex) && images[posIndex].AnimationName == name; 
        }
        /// <summary>
        /// Animates texture at positionIndex
        /// </summary>
        /// <param name="xTo">new X position</param>
        /// <param name="yTo">new Y position</param>
        /// <param name="timeInMs">Time to use to animate</param>
        public void Animate(string animationName, int toX, int toY, float timeInMs, bool loop = false, int positionIndex = 0)
        {
            if(images.ContainsKey(positionIndex)){
                images[positionIndex].StartAnimation(animationName,toX, toY, timeInMs, loop);
            }
        }

        public void Animate(string animationName, Vector2 toPos, float timeInMs, bool loop = false, int positionIndex = 0)
        {
            Animate(animationName, (int)toPos.X, (int)toPos.Y, timeInMs, loop, positionIndex);
        }

        public void RemoveDrawPosition(int entryId)
        {
            if (images.ContainsKey(entryId))
                images.Remove(entryId);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var i in this.images)
            {
//                spriteBatch.Draw(img, pos.Value, null, Color.White, Rotation, Vector2.Zero, Scale, 0, 0f);
                spriteBatch.Draw(img, i.Value.GetPos(gameTime), null, Color.White, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
            }
        }

        public override void Dispose()
        {
            img.Dispose();
        }
    }

    class ImageInfo{

        public ImageInfo(float x, float y)
        {
            this.Current = new Vector2(x, y); 
        }

        public void StartAnimation(string name, int toX, int toY, float time, bool loop)
	    {
            if (Current.X != toX || Current.Y != toY)
            {
                this.start = new Vector2(Current.X, Current.Y);
                this.end = new Vector2(toX, toY);
                this.loop = loop;
                this.timeTotal = time;
                this.timeUsed = 0;
                this.AnimationName = name;
                this.animationOn = true;
            }
	    }

        public Vector2 GetPos(GameTime gameTime){
            if (animationOn)
            {
                timeUsed += gameTime.ElapsedGameTime.Milliseconds;

                float percent = timeUsed / timeTotal;
                if (percent >= 0.99)
                {
                    if (loop)
                    {
                        Current = start;
                    }
                    else
                    {
                        animationOn = false;
                        Current = end;
                    }

                }else
                    Current = start + (end - start) * percent;
            }

            return Current;
        }

        public string AnimationName {get; private set;}
        public Vector2 Current;

        private Vector2 start;
        private Vector2 end;
        private int timeUsed;
        private float timeTotal;
        private bool loop;
        private bool animationOn = false;

    }
}
