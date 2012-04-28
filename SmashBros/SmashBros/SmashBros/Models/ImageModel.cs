using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SmashBros.MySystem;

namespace SmashBros.Models
{
    public class ImageModel
    {
        public ImageModel(Vector2 pos, float scale = 1f)
        {
            this.CurrentPos = pos;
            this.CurrentScale = scale;
        }

        #region Model Properties

        /// <summary>
        /// Positions for model
        /// </summary>
        public Vector2 CurrentPos, StartPos, EndPos;
        /// <summary>
        /// Scales used for model
        /// </summary>
        public float CurrentScale, StartScale, EndScale;
        /// <summary>
        /// Origin of target
        /// </summary>
        public Vector2 Origin;
        /// <summary>
        /// Rotation
        /// </summary>
        public float CurrentRotation, StartRotation, EndRotation;

        /// <summary>
        /// An id for ImageModel
        /// </summary>
        public int Id { get; set; }

        public bool AnimationOn { get { return animatePos || animateScale; } set { animateScale = false; animatePos = false; } }

     
        /// <summary>
        /// loop animation
        /// </summary>
        public bool loop { get; set; }

        /// <summary>
        /// time total for animation
        /// </summary>
        public float timeTotal { get; set; }

        /// <summary>
        /// Time used in animation
        /// </summary>
        public int timeUsed { get; set; }

        /// <summary>
        /// Boolean that is true if pos animation is on
        /// </summary>
        public bool animatePos { get; set; }

        /// <summary>
        /// Boolean that is true if scale animation is on
        /// </summary>
        public bool animateScale { get; set; }

        /// <summary>
        /// BOund box for this image
        /// </summary>
        public Body BoundBox { get; set; }

        /// <summary>
        /// Frames pr second for this istances if it runs frame animation
        /// </summary>
        public int FPS;

        /// <summary>
        /// params for frame animation
        /// </summary>
        public int CurrentFrame, StartFrame, EndFrame;

        /// <summary>
        /// Callback that is run every time the animation reaches a end point
        /// event if loop
        /// </summary>
        public Action<ImageModel> Callback;
        #endregion


        internal void SetBoundBox(World World, int width, int height, Vector2 offset,
            Category category = Category.None, Category collidesWith = Category.None, bool isStatic = false)
        {
            BoundBox = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height), 1f);

            BoundBox.Position = ConvertUnits.ToSimUnits(CurrentPos) +
                ConvertUnits.ToSimUnits(offset);

            BoundBox.CollidesWith = collidesWith;
            BoundBox.CollisionCategories = category;
            BoundBox.IsStatic = isStatic;

            Origin = new Vector2(width / 2, height / 2);
        }

        internal void BoundBoxDimension(World world, int width, int height, Vector2 offset)
        {
            DisposBoundBox();
            SetBoundBox(world, width, height, offset);
        }

        internal void DisposBoundBox()
        {
            if (BoundBox != null)
                BoundBox.Dispose();
        }
    }
}
