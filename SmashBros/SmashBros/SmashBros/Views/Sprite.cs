using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Factories;
using SmashBros.System;
using FarseerPhysics.Dynamics;
using System.Diagnostics;

namespace SmashBros.Views
{
    public class Sprite : IView
    {
        
        Texture2D texture;
        Rectangle frame;
        World world;
        Vector2 spritePos;
        Vector2 boundBoxSize;
        BodyType bodyType;
        public Vector2 size;


        public int FramesPerRow = 1;
        public Body BoundBox;

        public Sprite(ContentManager content, string assetName, int frameWidth, int frameHeight, float xPos, float yPos)
        {
            this.frame = new Rectangle(0, 0, frameWidth, frameHeight);
            this.size = ConvertUnits.ToSimUnits(frameWidth, frameHeight);
            this.texture = content.Load<Texture2D>(assetName);
            this.spritePos = ConvertUnits.ToSimUnits(xPos, yPos);
            this.Scale = 1f;
        }

        public void BoundRect(World world, float width, float height, BodyType type = BodyType.Dynamic)
        {
            this.world = world;
            this.boundBoxSize = ConvertUnits.ToSimUnits(width, height);
            this.bodyType = type;
            this.BoundBox = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1f, spritePos);
            this.BoundBox.BodyType = type;
            this.BoundBox.FixedRotation = true;
        }

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                base.IsActive = value;
                if (value)
                {
                    this.BoundBox.Enabled = true;
                }
                else
                {
                    this.BoundBox.Enabled = false;
                }
            }
        }

        public Vector2 Position {
            get { return BoundBox != null ? ConvertUnits.ToDisplayUnits(BoundBox.Position) : Vector2.Zero; }
            set { if (BoundBox != null) BoundBox.Position = ConvertUnits.ToSimUnits(value); }
        }

        public float PositionX
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float PositionY
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        private Vector2 force = Vector2.Zero;
        
        public Vector2 Force {
            get { return force; }
            set
            {
                if (BoundBox != null)
                {
                    force = value;
                    BoundBox.ApplyForce(force);
                }
            }
        }

        public float ForceX
        {
            get { return Force.X; }
            set { Force = new Vector2(value, Force.Y); }
        }

        public float ForceY
        {
            get { return Force.Y; }
            set { Force = new Vector2(Force.X, value); }
        }

        public Vector2 Impulse
        {
            set
            {
                if (BoundBox != null)
                {
                    BoundBox.ApplyLinearImpulse(value);
                }
            }
        }

        public Vector2 Velocity
        {
            get { return BoundBox != null ? BoundBox.LinearVelocity : Vector2.Zero; }
            set
            {
                if (BoundBox != null)
                {
                    BoundBox.LinearVelocity = value;
                }
            }
        }

        public float VelocityX
        {
            get { return Velocity.X; }
            set
            {
                Velocity = new Vector2(value, Velocity.Y);
            }
        }

        public float VelocityY
        {
            get { return Velocity.Y; }
            set
            {
                Velocity = new Vector2(Velocity.X, value);
            }
        }

        public float Mass { get { return this.BoundBox.Mass; } set { this.BoundBox.Mass = value; } }

        public bool AllowRotation 
        { 
            get { return this.BoundBox.FixedRotation; } 
            set { this.BoundBox.FixedRotation = value; } 
        }

        public Category CollidesWith { set { this.BoundBox.CollidesWith = value; } }

        public Category Category { set { this.BoundBox.CollisionCategories = value; } }

        public float Rotation { get; set; }
        public float Scale { get; set; }

        public override object UserData { set { this.BoundBox.UserData = value; base.UserData = value; } }

        public void StartAnimation(int frameStart, int frameEnd, bool loop)
        {

        }

        public void StopAnimation()
        {

        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(BoundBox.Position), frame, Color.White, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

        public override void Dispose()
        {
            texture.Dispose();
            BoundBox.Dispose();
        }
    }
}
