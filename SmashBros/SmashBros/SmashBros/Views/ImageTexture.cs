using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace SmashBros.Views
{
    /// <summary>
    /// ImageTexture is a texture that can be drawn on multiple position
    /// and easily animated
    /// </summary>
    public class ImageTexture : IView
    {
        private Texture2D img;
        
        private List<ImageInfo> imagesPos;
        private ConcurrentQueue<Tuple<bool, ImageInfo>> imageQueue;
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public int PosCount { get { return imagesPos.Count(); } }

        public ImageTexture(ContentManager content, string assetName)
        {
            img = content.Load<Texture2D>(assetName);
            imageQueue = new ConcurrentQueue<Tuple<bool, ImageInfo>>();
            imagesPos = new List<ImageInfo>();
            Scale = 1f;
            Origin = Vector2.Zero;
        }

        public ImageTexture(ContentManager content, string assetName, float xpos, float ypos) 
            :this(content,assetName)
        {
            imagesPos.Add(new ImageInfo(xpos, ypos, Scale));
        }

        public ImageTexture(ContentManager content, string assetName, Vector2 pos)
            : this(content, assetName)
        {
            imagesPos.Add(new ImageInfo(pos, Scale));
        }

        public void Position(float x, float y)
        {
            if(PosCount > 0)
                this.imagesPos.First().CurrentPos = new Vector2(x, y);
        }

        public void Position(Vector2 pos)
        {
            Position(pos.X, pos.Y);
        }

        public ImageInfo AddPosition(float x, float y, int posId = -1)
        {
            return AddPosition(new Vector2(x,y), posId);
        }

        public ImageInfo AddPosition(Vector2 pos, int posId = -1)
        {
            ImageInfo i = null;
            //Sjekker om pos id eksisterer fra før
            if (posId != -1)
            {
                var c = imagesPos.Where(a => a.Id == posId);
                i = c.Count() != 0 ? c.First() : null;
                i.CurrentPos = pos;
            }

            if(i == null)
            {
                i = new ImageInfo(pos, Scale);
                imageQueue.Enqueue(Tuple.Create(true, i));
            }
            return i;
        }

        /// <summary>
        /// Animates texture at positionIndex
        /// </summary>
        /// <param name="xTo">new X position</param>
        /// <param name="yTo">new Y position</param>
        /// <param name="timeInMs">Time to use to animate</param>
        public void Animate(float toX, float toY, float timeInMs, bool loop = false, float scale = 1f)
        {
            Animate(imagesPos.First(), toX, toY, timeInMs, loop);
        }

        public void Animate(int posIndex, float toX, float toY, float timeInMs, bool loop = false, float scale = 1f)
        {
            Animate(imagesPos[posIndex], toX, toY, timeInMs, loop, scale);
        }

        public void Animate(ImageInfo info, float toX, float toY, float timeInMs, bool loop = false, float scale = 1f)
        {

            info.StartAnimation(toX, toY, timeInMs, loop, scale);
        }

        public void Animate(int posIndex, Vector2 toPos, float timeInMs, bool loop = false, float scale = 1f)
        {
            Animate(posIndex,toPos.X,toPos.Y, timeInMs, loop, scale);
        }

        public void Animate(int posIndex, float scale, float time, bool loop = false)
        {
            Animate(posIndex, imagesPos[posIndex].CurrentPos, time, loop, scale);
        }

        public void RemovePosition(ImageInfo pos)
        {
            if(pos != null)
                imageQueue.Enqueue(Tuple.Create(false, pos));
        }

        public void RemovePosition(int index)
        {
            RemovePosition(imagesPos[index]);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            while (!imageQueue.IsEmpty)
            {
                Tuple<bool,ImageInfo> queued;
                if (imageQueue.TryDequeue(out queued))
                {
                    if (queued.Item1)
                    {
                        imagesPos.Add(queued.Item2);
                    }
                    else
                    {
                        imagesPos.Remove(queued.Item2);
                    }
                }
            }

            foreach (var i in this.imagesPos)
            {

                //If animation done invoke event
                if (i.UpdateAnimation(gameTime) && OnAnimationDone != null)
                {
                    OnAnimationDone.Invoke(this, i);
                }
//                spriteBatch.Draw(img, pos.Value, null, Color.White, Rotation, Vector2.Zero, Scale, 0, 0f);
                spriteBatch.Draw(img, i.CurrentPos, null, Color.White, Rotation, Origin, i.CurrentScale, SpriteEffects.None, 0);
            }
        }

        public override void Dispose()
        {
            img.Dispose();
        }

        internal delegate void AnimationDone(ImageTexture target, ImageInfo imagePosition);
        internal event AnimationDone OnAnimationDone;
    }

    public class ImageInfo{

        public ImageInfo(float x, float y, float scale)
        {
            this.CurrentPos = new Vector2(x, y);
            this.CurrentScale = scale;
            this.endScale = scale;
            this.startScale = scale;
        }

        public ImageInfo(Vector2 pos, float scale)
        {
            this.CurrentPos = pos;
            this.CurrentScale = scale;
        }

        public void StartAnimation(float toX, float toY, float time, bool loop, float endScale = 1f)
	    {
            StartAnimation(new Vector2(toX, toY), time, loop, endScale);
	    }

        public void StartAnimation(Vector2 endPos, float time, bool loop, float endScale = 1f)
        {
            this.startPos = new Vector2(CurrentPos.X, CurrentPos.Y);
            this.endPos = endPos;
            this.startScale = this.CurrentScale;
            this.endScale = endScale;
            this.loop = loop;
            this.timeTotal = time;
            this.timeUsed = 0;
            this.animationOn = true;
            this.endScale = endScale;
        }

        /// <summary>
        /// Update the animation if animationIs on
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns>true if animation was done</returns>
        public bool UpdateAnimation(GameTime gameTime){
            if (animationOn)
            {
                timeUsed += gameTime.ElapsedGameTime.Milliseconds;

                float percent = timeUsed / timeTotal;

                if (percent >= 0.99)
                {
                    if (loop)
                    {
                        CurrentPos = startPos;
                    }
                    else
                    {
                        CurrentPos = endPos;
                        CurrentScale = endScale;
                        return true;
                    }

                }
                else
                {
                    CurrentScale = startScale + (endScale - startScale) * percent;
                    CurrentPos = startPos + (endPos - startPos) * percent;
                }
            }

            return false;
        }

        public Vector2 CurrentPos;
        public float CurrentScale;
        public int Id = -1;
        private Vector2 startPos;
        private Vector2 endPos;
        private float startScale;
        private float endScale;
        private int timeUsed;
        private float timeTotal;
        private bool loop;
        private bool animationOn = false;
    }
}
