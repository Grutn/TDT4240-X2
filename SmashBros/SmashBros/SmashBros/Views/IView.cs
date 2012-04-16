﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.Views
{
    public abstract class IView
    {
        public IView()
        {
            Layer = 0;
        }
        public virtual bool IsActive { get; set; }
        public float Layer { get; set; }
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void Dispose();
        /// <summary>
        /// Used to store e.g. models that belongs to the view
        /// </summary>
        public virtual object UserData { get; set; }
    }
}