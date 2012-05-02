using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization;
using SmashBros.MySystem;

namespace SmashBros.Models
{
    /// <summary>
    /// Same as rectangle only with floats instead of ints, and is Serializable
    /// </summary>
    public class Box
    {
        public Box()
        {

        }

        public Box(float width, float height, float x, float y)
        {
            this.Width = width;
            this.Height = height;
            this.X = x;
            this.Y = y;
        }


        public float Width;
        public float Height;
        public float Friction;
        public float X;
        public float Y;


        //Creates a body out of this box
        public Body CreateBody(World world, Category collisionCategory = Category.All)
        {
            Body b = BodyFactory.CreateRectangle(
                world, 
                ConvertUnits.ToSimUnits(Width), 
                ConvertUnits.ToSimUnits(Height), 
                1f, 
                ConvertUnits.ToSimUnits(X, Y));

            b.CollisionCategories = collisionCategory;
            b.IsStatic = true;
            b.Friction = 0.7f;
            b.UserData = ConvertUnits.ToSimUnits(Height);

            return b;
        }
    }
}
