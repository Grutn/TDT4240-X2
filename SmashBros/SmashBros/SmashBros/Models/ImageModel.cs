using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SmashBros.System;

namespace SmashBros.Models
{
    public class ImageModel
    {
        public ImageModel(Vector2 pos, float scale = 1f)
        {
            this.CurrentPos = pos;
            this.CurrentScale = scale;
        }

        public Vector2 CurrentPos;
        public float CurrentScale;
        public int CurrentFrame;
        public Vector2 Origin;
        public float Rotation;



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

        internal void BoundBoxDimension(World world, int width, int height, Vector2 offset){
            DisposBoundBox();
            SetBoundBox(world, width, height, offset);
        }

        internal void DisposBoundBox()
        {
            if (BoundBox != null)
                BoundBox.Dispose();
        }

        public int Id { get; set; }

        public bool AnimationOn { get { return animatePos || animateScale; } set { animateScale = false; animatePos = false; } }

        public Vector2 startPos { get; set; }

        public Vector2 endPos { get; set; }

        public float startScale { get; set; }

        public float endScale { get; set; }

        public bool loop { get; set; }

        public float timeTotal { get; set; }

        public int timeUsed { get; set; }

        public bool animatePos { get; set; }

        public bool animateScale { get; set; }

        public Body BoundBox { get; set; }
    }
}
