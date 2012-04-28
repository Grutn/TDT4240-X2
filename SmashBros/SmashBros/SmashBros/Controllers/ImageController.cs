﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SmashBros.System;
using SmashBros.Models;
using SmashBros.Views;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Graphics;

namespace SmashBros.Controllers
{
    /// <summary>
    /// ImageController Controls one image but can have many positions
    /// And different animations on differente positions
    /// </summary>
    public class ImageController : Controller
    {

        #region Private Varaiblaes
        private List<ImageModel> imageModels;
        private ImageView imageView;
        private ConcurrentQueue<Tuple<bool, ImageModel>> imageQueue;
        private bool emptyList;
        private string assetName;
        private bool visible = true, loaded = false;
        #endregion

        #region Public variables
        /// <summary>
        /// Default scale when add a position
        /// </summary>
        public float ScaleDefault = 1f;
        /// <summary>
        /// View layer
        /// </summary>
        public float Layer { get { return imageView.Layer; } set { imageView.Layer = value; } }
        /// <summary>
        /// The views static position
        /// </summary>
        public bool StaticPosition { get { return imageView.StaticPosition; } set { imageView.StaticPosition = value; } }
        /// <summary>
        /// If the image is visieble or not,
        /// this meaning that the draw is not run at any positions of the image
        /// </summary>
        public bool IsVisible
        {
            get { return visible; }
            set
            {
                visible = value;
                if (visible && loaded)
                {
                    AddView(imageView);
                }else if(!visible) RemoveView(imageView);
            }
        }

        public Rectangle? FrameRectangle
        {
            get { return imageView.FrameRectangle; }
            set { imageView.FrameRectangle = value;}
        }

        public int FramesPerRow;

        #endregion


        public ImageController(ScreenManager screen, string assetName, int layer = 0, bool staticPosition = false)
            :base(screen)
        {
            this.assetName = assetName;
            this.imageModels = new List<ImageModel>();
            
            this.imageView = new ImageView(imageModels);
            this.imageView.Layer = layer;
            this.imageView.StaticPosition = staticPosition;

            this.imageQueue = new ConcurrentQueue<Tuple<bool, ImageModel>>();
        }

        public ImageController(ScreenManager screen, string assetName, Vector2 pos, int layer = 0, bool staticPosition = false)
            : this(screen, assetName, layer, staticPosition)
        {
            SetPosition(pos);
        }

        public override void Load(ContentManager content)
        {
            imageView.Image = content.Load<Texture2D>(assetName);
            loaded = true;
            if(visible)
                AddView(imageView);
        }

        /// <summary>
        /// Remove all other image models and create new model with position
        /// </summary>
        public ImageModel SetPosition(int x, int y)
        {
            return SetPosition(new Vector2(x, y));
        }

        /// <summary>
        /// Remove all other image models and create new model with position
        /// </summary>
        public ImageModel SetPosition(Vector2 pos)
        {
            this.imageModels = new List<ImageModel>();
            this.imageView.imageModels = imageModels;
            var model = new ImageModel(pos);
            this.imageModels.Add(model);

            return model;
        }

        public void SetFrameRectangle(int widht, int height)
        {
            FrameRectangle = new Rectangle(0, 0, widht, height);
        }

        public void EmptyList()
        {
            this.emptyList = true;
        }

        /// <summary>
        /// Gets the model at index
        /// </summary>
        /// <param name="index"></param>
        public ImageModel GetAt(int index)
        {
            if(imageModels.Count() > index)
                return imageModels[index];
            return null;
        }
        #region Add & Remove Models

        public ImageModel AddPosition(float x, float y, int posId = -1)
        {
            return AddPosition(new Vector2(x, y), posId);
        }

        public ImageModel AddPosition(Vector2 pos, int posId = -1)
        {

            ImageModel i = null;
            //Sjekker om pos id eksisterer fra før
            if (posId != -1)
            {
                var c = imageModels.Where(a => a.Id == posId);
                i = c.Count() != 0 ? c.First() : null;
                if (i != null)
                    i.CurrentPos = pos;
            }

            if (i == null)
            {
                i = new ImageModel(pos, ScaleDefault) { Id = posId };
                imageQueue.Enqueue(Tuple.Create(true, i));
            }
            return i;
        }

        public bool IsAnimating
        {
            get
            {
                return imageModels.Any(a => a.AnimationOn);
            }
        }

        public void RemovePosition(ImageModel pos)
        {
            if (pos != null)
                imageQueue.Enqueue(Tuple.Create(false, pos));
        }

        public void RemovePosition(int index)
        {
            RemovePosition(imageModels[index]);
        }

        public void RemoveId(int id)
        {
            if (imageModels.Exists(a => a.Id == id))
            {
                foreach (var i in imageModels.Where(a => a.Id == id))
                {
                    imageQueue.Enqueue(Tuple.Create(false, i));
                }
            }
        }
        
        #endregion

        #region AnimationFunctions

        public void Animate(ImageModel model, Vector2 toPos, float timeInMs, bool loop, float toScale = 1f)
        {
            model.startPos = new Vector2(model.CurrentPos.X, model.CurrentPos.Y);
            model.endPos = toPos;
            model.startScale = model.CurrentScale;
            model.endScale = toScale;
            model.loop = loop;
            model.timeTotal = timeInMs;
            model.timeUsed = 0;

            //Check what kind of animation that is started
            model.animatePos = model.endPos.X != model.startPos.X || model.endPos.Y != model.startPos.Y;
            model.animateScale = model.endScale != model.startScale;
        }

        /// <summary>
        /// Animates texture at positionIndex
        /// </summary>
        /// <param name="xTo">new X position</param>
        /// <param name="yTo">new Y position</param>
        /// <param name="timeInMs">Time to use to animate</param>
        public void Animate(ImageModel model, float toX, float toY, float timeInMs, bool loop = false, float toScale = 1f)
        {
            Animate(model, new Vector2(toX, toY), timeInMs, loop, toScale);
        }

        public void Animate(float toX, float toY, float timeInMs, bool loop = false, float scale = 1f)
        {
            Animate(imageModels.First(), toX, toY, timeInMs, loop, scale);
        }

        public void Animate(int posIndex, float toX, float toY, float timeInMs, bool loop = false, float scale = 1f)
        {
            Animate(imageModels[posIndex], toX, toY, timeInMs, loop, scale);
        }

        public void Animate(int posIndex, Vector2 toPos, float timeInMs, bool loop = false, float scale = 1f)
        {
            Animate(posIndex, toPos.X, toPos.Y, timeInMs, loop, scale);
        }

        public void AnimateScale(int posIndex, float scale, float time, bool loop = false)
        {
            Animate(posIndex, imageModels[posIndex].CurrentPos, time, loop, scale);
        }

        /// <summary>
        /// Annimates the given model to the given scale
        /// </summary>
        public void AnimateScale(ImageModel model, float toScale, float timeInMs, bool loop = false)
        {
            Animate(model, model.CurrentPos, timeInMs, loop, toScale);
        }

        /// <summary>
        /// Anniates the first model(if exists) instance to given scale
        /// </summary>
        public void AnimateScale(float toScale, float timeInMs, bool loop = false)
        {
            if (imageModels.Count() != 0)
                AnimateScale(imageModels.First(), toScale, timeInMs, loop);
        }

        /// <summary>
        /// Annimates the given model to the given position(x,y)
        /// </summary>
        public void AnimatePos(ImageModel model, float toX, float toY, float timeInMs, bool loop = false){
            this.Animate(model, new Vector2(toX,toY), timeInMs, loop, model.CurrentScale);
        }
        /// <summary>
        /// Anniates the first model (if exists) instance to given position(x,y)
        /// </summary>
        public void AnimatePos(float toX, float toY, float timeInMs, bool loop = false)
        {
            if (imageModels.Count() != 0)
                AnimatePos(imageModels.First(), toX, toY, timeInMs, loop);
        }

        private bool updateAnimation(GameTime gameTime, ImageModel model)
        {
            model.timeUsed += gameTime.ElapsedGameTime.Milliseconds;

            float percent = model.timeUsed / model.timeTotal;

            if (percent >= 0.99)
            {
                if (model.loop)
                {
                    model.CurrentPos = model.startPos;
                    model.CurrentScale = model.endScale;
                }
                else
                {
                    model.CurrentPos = model.endPos;
                    model.CurrentScale = model.endScale;
                    model.AnimationOn = false;
                    return true;
                }

            }
            else
            {
                if (model.animateScale)
                    model.CurrentScale = model.startScale + (model.endScale + -model.startScale) * percent;
                if (model.animatePos)
                    model.CurrentPos = model.startPos + (model.endPos - model.startPos) * percent;
            }

            return false;
        }

        //public void StartAnimation(float toX, float toY, float time, bool loop, float endScale = 1f)
        //{
        //    //StartAnimation(imagenew Vector2(toX, toY), time, loop, endScale);
        //}

        #endregion


        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            while (!imageQueue.IsEmpty)
            {
                Tuple<bool, ImageModel> queued;
                if (imageQueue.TryDequeue(out queued))
                {
                    if (queued.Item1)
                    {
                        imageModels.Add(queued.Item2);
                    }
                    else
                    {
                        imageModels.Remove(queued.Item2);
                    }
                }
            }

            foreach(ImageModel model in imageModels){
                if (model.AnimationOn)
                {
                    if (updateAnimation(gameTime, model) && OnAnimationDone != null)
                        OnAnimationDone.Invoke(this, model);
                }
            }
        }


        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        public event AnimationDone OnAnimationDone;

    }

    public delegate void AnimationDone(ImageController target, ImageModel imagePosition);
}