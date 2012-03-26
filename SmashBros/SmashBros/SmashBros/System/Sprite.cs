using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Factories;
using SmashBros.System;

namespace FarseerPhysics.Dynamics
{
    public class Sprite
    {
        private Texture2D texture;
        private Rectangle frame;

        public int FramesPerRow = 1;
        public Body BoundBox;

        public Sprite(ContentManager content, string assetName, int frameWidth, int frameHeight)
        {
            frame = new Rectangle(0, 0, frameWidth, frameHeight);
            texture = content.Load<Texture2D>(assetName);
        }

        public void BoundRect(World world, float xPos, float yPos, float width, float height, BodyType type = BodyType.Dynamic)
        {
            BoundBox = BodyFactory.CreateRectangle(
                world,
                ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height),
                1f,
                ConvertUnits.ToSimUnits(xPos, yPos));

            BoundBox.BodyType = type;
        }

        public void ApplyForce(float x, float y)
        {
            BoundBox.ApplyForce(new Vector2(x, y));
        }

        public void StartAnimation(int frameStart, int frameEnd, bool loop)
        {

        }

        public void StopAnimation()
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(BoundBox.Position), frame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
