using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization;
using SmashBros.System;

namespace SmashBros.Model
{
    [DataContract]
    public class Box
    {
        public Box()
        {

        }

        public Box(float width, float height, float x, float y, float rotation = 0f)
        {
            this.Width = width;
            this.Height = height;
            this.X = x;
            this.Y = y;
            this.Rotation = rotation;
        }

        [DataMember]
        public float Width;
        [DataMember]
        public float Height;
        [DataMember]
        public float X;
        [DataMember]
        public float Y;
        [DataMember]
        public float Rotation;

        public Body CreateBody(World world)
        {
            Body b = BodyFactory.CreateRectangle(
                world, 
                ConvertUnits.ToSimUnits(Width), 
                ConvertUnits.ToSimUnits(Height), 
                1f, 
                ConvertUnits.ToSimUnits(X, Y));

            b.Rotation = Rotation;

            return b;
        }
    }
}
