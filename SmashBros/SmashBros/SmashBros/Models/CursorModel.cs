using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Views;
using SmashBros.Controllers;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SmashBros.Models
{
    public class CursorModel
    {
        public CursorModel(ContentManager content, World world, GamepadController pad, 
            SmashBros.Controllers.GamepadController.NavigationKey navigationMethod,
           OnCollisionEventHandler col, OnSeparationEventHandler sep,bool enabled = false)
        {
            Cursor = new Sprite(content, "Cursors/Player" + pad.PlayerIndex, 70, 70, 280 * pad.PlayerIndex + 100, 680);
            Cursor.BoundRect(world, 1, 1, BodyType.Dynamic);
            Cursor.StaticPosition = true;
            Cursor.Category = Category.Cat4;
            Cursor.CollidesWith = Category.Cat5;
            Cursor.Layer = 1002;
            Cursor.Mass = 1;
            Cursor.UserData = pad.PlayerIndex;
            Cursor.BoundBox.IgnoreGravity = true;
            Cursor.Origin = new Vector2(35, 35);

            this.Pad = pad;
            this.Navigation = navigationMethod;
            this.OnCollision = col;
            this.OnSeparation = sep;
            this.Enabled = enabled;
        }

        public void SetMinMaxPos(float minX, float maxX, float minY, float maxY){
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
        private float minX,maxX, minY,maxY;

        public Sprite Cursor;
        public GamepadController Pad;
        public SmashBros.Controllers.GamepadController.NavigationKey Navigation;
        public OnCollisionEventHandler OnCollision;
        public OnSeparationEventHandler OnSeparation;
        public Body CurrentTarget;
        public Category TargetCategory
        {
            get
            {
                if (CurrentTarget != null && CurrentTarget.FixtureList != null && CurrentTarget.FixtureList.Count() != 0)
                {
                    return CurrentTarget.FixtureList.First().CollisionCategories;
                }
                return Category.All;
            }
        }

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;

                    Cursor.BoundBox.Enabled = enabled;
                    //Pad.OnNavigation -= Navigation;
                    //Cursor.BoundBox.OnCollision -= OnCollision;
                    //Cursor.BoundBox.OnSeparation -= OnSeparation;
                    if (enabled)
                    {
                        Pad.OnNavigation += Navigation;
                        Cursor.BoundBox.OnCollision += OnCollision;
                        Cursor.BoundBox.OnSeparation += OnSeparation;

                    }
                    else
                    {
                        Pad.OnNavigation -= Navigation;
                        Cursor.BoundBox.OnCollision -= OnCollision;
                        Cursor.BoundBox.OnSeparation -= OnSeparation;
                    }
                }

            }
        }

        public void SetPosition(float x, float y)
        {
            this.Cursor.PositionX = MathHelper.Clamp(x, minX, maxX);
            this.Cursor.PositionY = MathHelper.Clamp(y, minY, maxY);
        }
    }
}
