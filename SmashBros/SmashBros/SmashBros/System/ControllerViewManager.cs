using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using SmashBros.Views;
using System.Diagnostics;
using SmashBros.Controllers;
using FarseerPhysics;
using Microsoft.Xna.Framework.Content;

namespace SmashBros.System
{
    public class ControllerViewManager
    {
        public DebugViewXNA debugView;
        public Camera2D camera;

        private static List<Controller> controllers;
        private static bool controllersListAvailable = true;
        
        private static List<IView> views;
        private static bool viewListAvailable = true;

        GraphicsDevice graphicsDevice;
        SpriteBatch batch;


        public World world;
        public ContentManager content;

        public ControllerViewManager(GraphicsDevice graphicsDevice, ContentManager content)
        {
            controllers = new List<Controller>();
            views = new List<IView>();

            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.batch = new SpriteBatch(graphicsDevice);
            this.world = new World(Vector2.Zero);

            if (Constants.DebugMode && debugView == null)
            {
                debugView = new DebugViewXNA(world);
                debugView.AppendFlags(DebugViewFlags.DebugPanel);
                debugView.DefaultShapeColor = Color.Red;
                debugView.SleepingShapeColor = Color.Green;
                debugView.LoadContent(graphicsDevice, content);
            }

            if (camera == null)
            {
                Viewport v = graphicsDevice.Viewport;
                camera = new Camera2D(graphicsDevice);
                camera.Position = new Vector2(v.Width / 2, v.Height / 2);
            }
            else camera.ResetCamera();
        }

        public void Update(GameTime gameTime)
        {
            if (controllers.Count() != 0)
            {
                controllersListAvailable = false;
                foreach (var controller in controllers)
                {
                    if (controller.IsActive)
                    {
                        controller.Update(gameTime);
                    }
                    else
                    {
                        controller.Load(content);
                        controller.IsActive = true;
                    }
                }
            }
            controllersListAvailable = true;

            //We update the world
            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            camera.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            batch.Begin(0,null, null, null, null, null, camera.View);
            
            if (views.Count() != 0)
            {
                viewListAvailable = false;
                views.OrderBy(a=> a.Layer).ToList().ForEach(a => a.Draw(batch, gameTime));
            }
            viewListAvailable = true;

            batch.End();

            if (Constants.DebugMode)
            {
                Matrix projection = camera.SimProjection;
                Matrix matrixView = camera.SimView;

                debugView.RenderDebugData(ref projection, ref matrixView);
            }
        }

        public static void AddView(object obj)
        {
            IView view = (IView)obj;
            if (!views.Contains(view))
            {
                while (!viewListAvailable) ;
                view.IsActive = true;
                views.Add(view);
            }
            else
            {
                Debug.WriteLine("Active views already contains this view");
            }
        }

        public static void RemoveView(object obj)
        {
            IView view = (IView)obj;
            if (views.Contains(view))
            {
                while (!viewListAvailable) ;
                view.IsActive = false;
                views.Remove(view);
            }
            else
            {
                Debug.WriteLine("Active views don't contain this view");
            }
        }

        public static void AddController(object obj)
        {
            Controller controller = (Controller)obj;
            if (!controllers.Contains(controller))
            {
                while (!controllersListAvailable) ;
                controllers.Add(controller);
            }
            else
            {
                Debug.WriteLine("Active controllers already contains this controller");
            }
        }

        public static void RemoveController(object obj)
        {
            Controller controller = (Controller)obj;
            if (controllers.Contains(controller))
            {
                while (!controllersListAvailable) ;
                controllers.Remove(controller);
                controller.Deactivate();
            }
            else
            {
                Debug.WriteLine("Active controllers don't contain this controller");
            }
        }

        internal void UnloadControllers()
        {
            foreach (var controller in controllers)
            {
                controller.Unload();
            }

            controllers = new List<Controller>();
        }
    }
}
