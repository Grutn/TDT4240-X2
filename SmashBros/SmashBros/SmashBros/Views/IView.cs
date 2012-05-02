using System;
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
            Origin = Vector2.Zero;
            Layer = 0;
        }

        /// <summary>
        /// Is true if the view is int the queue for deawing,
        /// Variable is set by the ControllerViewManager
        /// </summary>
        public virtual bool IsActive { get; set; }
        /// <summary>
        /// Is true if the view don't uses the camera position
        /// </summary>
        public bool StaticPosition { get; set; }
        /// <summary>
        /// Tells which layer this view should be drawn on
        /// </summary>
        public float Layer { get; set; }
        /// <summary>
        /// The origin for the texture or textbox  etc
        /// </summary>
        public Vector2 Origin { get; set; }
        /// <summary>
        /// Runs when the view is put up for drawing
        /// It isn't necessary to spritBatch.Begin() & end(), this is handeled
        /// by the ControllerViewManager so theres as few begin end as possible
        /// </summary>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        /// <summary>
        /// Is run when the view gets disposed
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// Used to store e.g. models that belongs to the view
        /// </summary>
        public virtual object UserData { get; set; }
    }
}
