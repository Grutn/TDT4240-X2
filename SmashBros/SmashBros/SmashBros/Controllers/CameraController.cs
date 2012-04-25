﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.System;
using SmashBros.Views;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using SmashBros.Model;
using FarseerPhysics.Collision.Shapes;

namespace SmashBros.Controllers
{
    class CameraController : Controller
    {
        Camera2D camera;
        List<CharacterController> characters{ get; set; }
        Box zoomBox;
            
        public CameraController(ScreenController screen, Box zoomBox) : base(screen)
        {
            this.characters = new List<CharacterController>();
            this.camera = screen.controllerViewManager.camera;
            this.zoomBox = zoomBox;
        }

        public override void Load(ContentManager content)
        {

        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            float minX = 9999, minY = 9999, maxX = 0, maxY = 0;
            foreach (var chara in characters)
            {
                var t = chara.character;
                if (t != null)
                {
                    if (minX > t.PositionX) minX = t.PositionX;
                    if (minY > t.PositionY) minY = t.PositionY;

                    if (maxX < t.PositionX + 200) maxX = t.PositionX + 200;
                    if (maxY < t.PositionY + 200) maxY = t.PositionY + 200;
                }
            }
            minX -= 200;
            maxX += 200;

            minY -= 200;
           // maxY += 200;

            //maxY = MathHelper.Clamp(maxY, 0, mapSize.Height);
            //maxX = MathHelper.Clamp(maxX, 0, mapSize.Width);
            //minX = MathHelper.Clamp(minX,mapSize.X, 10000);
            //minY = MathHelper.Clamp(minY, mapSize.Y, 10000);


            float zoom =MathHelper.Clamp(MathHelper.Min(
                Constants.WindowWidth / (maxX - minX),
                Constants.WindowHeight / (maxY - minY)
            ), Constants.MinZoom, Constants.MaxZoom);

            if (zoomBox != null)
            {
                Vector2 halfWindow = new Vector2(Constants.WindowWidth, Constants.WindowHeight) / (2 * zoom);
                camera.MaxPosition = new Vector2(zoomBox.X + zoomBox.Width - halfWindow.X, zoomBox.Y + zoomBox.Height - halfWindow.Y);
                camera.MinPosition = new Vector2(zoomBox.X + halfWindow.X, zoomBox.Y + halfWindow.Y);
            }
            camera.Position = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
            camera.Zoom = zoom;
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        internal void AddCharacterTarget(CharacterController character)
        {
            this.characters.Add(character);
        }
    }
}
