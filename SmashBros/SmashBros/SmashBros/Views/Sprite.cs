using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Factories;
using SmashBros.MySystem;
using FarseerPhysics.Dynamics;
using System.Diagnostics;

namespace SmashBros.Views
{
    class SpriteAnimation
    {
        public SpriteAnimation(int fromFrame, int toFrame, int duration, bool loop)
        {
            this.CurrentFrame = fromFrame;
            this.FromFrame = fromFrame;
            this.ToFrame = toFrame;
            this.FPS = duration == -1 ? -1 : (toFrame - fromFrame + 1) * 1000f / duration;
            this.Loop = loop;
        }

        public string Name;
        public int FromFrame;
        public int ToFrame;
        public float FPS;
        public int CurrentFrame;
        public bool Loop;

        /// <summary>
        /// Does animation if elapsed time is big enought
        /// returnes true if animation has reach the end frame
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns>true if animation reach end frame</returns>
        public bool DoAnimation(ref float elapsedTime, int fps, int framesPerRow, ref Rectangle frame)
        {
            if ((CurrentFrame <= ToFrame) || (Loop && FromFrame < ToFrame))
            {
                //Set animateCurrent frame to animateFrom frame, 
                //if animate has reached animateTo frame
                if (CurrentFrame == ToFrame)
                {
                    //If not loop set aniamteTo = animateFrom so animations stops
                    if (!Loop)
                    {
                        ToFrame = FromFrame;
                    }
                    CurrentFrame = FromFrame;
                }
                //Check if elapsed time is large enough for the gams fps
                if (elapsedTime >= 1000 / (FPS == -1? fps : FPS == 0? 0.1 : FPS))
                {
                    elapsedTime = 0;
                    int row = (int)Math.Floor((double)CurrentFrame / framesPerRow);

                    frame.Y = frame.Height * row;
                    frame.X = frame.Width * (CurrentFrame - row * framesPerRow);

                    CurrentFrame++;
                }
            }

            return CurrentFrame == ToFrame + 1;
        }
    }

    public class Sprite : IView
    {
        
        Texture2D texture;
        Rectangle frame;
        World world;
        Vector2 spritePos;
        BodyType bodyType;
        float elapsedTime;
        List<SpriteAnimation> animations;
        public Vector2 size;
        public int fps = Constants.FPS;
        public float Opacity = 1;
        public bool Blinking = false;
        public bool Red = false;
        private bool blinkUp = false;

        private Vector2 FreezedVelocity;
        private bool _freezed;
        public bool Freezed
        {
            get { return _freezed; }
            set
            {
                if (value)
                {
                    FreezedVelocity = Velocity;
                    Velocity = new Vector2(0, 0);
                }
                else
                {
                    Velocity = FreezedVelocity;
                }
                _freezed = value;
            }
        }

        public int FramesPerRow = 1;
        public Body BoundBox;

        public Sprite(ContentManager content, string assetName, int frameWidth, int frameHeight, float xPos, float yPos)
        {
            this.frame = new Rectangle(0, 0, frameWidth, frameHeight);
            this.texture = content.Load<Texture2D>(assetName);
            this.spritePos = ConvertUnits.ToSimUnits(xPos, yPos);
            this.Scale = 1f;
            this.animations = new List<SpriteAnimation>();
            this.Origin = new Vector2(frame.Width, frame.Height) / 2;
        }

        public void BoundRect(World world, float width, float height, BodyType type = BodyType.Dynamic)
        {
            this.world = world;
            this.size = ConvertUnits.ToSimUnits(width, height);
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
                if (!Freezed && BoundBox != null)
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

        public SpriteEffects SpriteEffect { get; set; }

        public override object UserData { set { this.BoundBox.UserData = value; base.UserData = value; } }


        /// <summary>
        /// Start animation, from frame start to frameEnd
        /// and loops if loop == true
        /// </summary>
        /// <param name="frameStart"></param>
        /// <param name="frameEnd"></param>
        /// <param name="loop"></param>
        public void StartAnimation(int frameStart, int frameEnd, bool loop = false, int duration = -1)
        {
            this.animations = new List<SpriteAnimation>();
            AddAnimation(frameStart, frameEnd, loop, duration);
        }

        public void AddAnimation(int frameStart, int frameEnd, bool loop = false, int duration = -1)
        {
            this.animations.Add(new SpriteAnimation(frameStart, frameEnd, duration, loop));
        }

        public void ClearAnimations()
        {
            this.animations = new List<SpriteAnimation>();
        }
        /*
        /// <summary>
        /// Stops the running animation
        /// If atFrame = -1 it stops at current fram, else if atFrame > 1 it animates to the given frame and stops
        /// </summary>
        /// <param name="atFrame"></param>
        public void StopAnimation(int atFrame = -1)
        {
            animateLoop = false;
            animateTo = animateCurrent;
        }
        */

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (animations.Count() != 0)
            {
                SpriteAnimation ani = animations.First();
                //Updates the elapsed time
                if(!Freezed) elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (ani.DoAnimation(ref elapsedTime, fps, FramesPerRow, ref frame))
                {
                    if (!ani.Loop || (animations.Last() != ani))
                    {
                        animations.Remove(ani);
                    }
                }
            }
            
            Vector2 origin = new Vector2(frame.Width / 2f, frame.Height / 2f);

            if (Blinking)
            {
                if (blinkUp)
                {
                    Opacity += 0.03f;
                    if (Opacity > 1)
                    {
                        blinkUp = false;
                        Opacity = 1;
                    }
                }
                else
                {
                    Opacity -= 0.03f;
                    if (Opacity < 0.1) blinkUp = true;
                }
            }
            else Opacity = 1;

            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(BoundBox.Position), frame, Red? new Color(255,0,0,Opacity) : new Color(255,255,255, Opacity), Rotation, Origin, Scale, SpriteEffect, 0f);
        }

        public override void Dispose()
        {
            BoundBox.Dispose();
            System.GC.SuppressFinalize(this);
        }
    }
}
