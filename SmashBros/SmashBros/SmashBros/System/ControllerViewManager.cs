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
using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Content;

namespace SmashBros.System
{
    public class ControllerViewManager
    {
        public DebugViewXNA debugView;
        public Camera2D camera;

        private List<Controller> controllers;
        private ConcurrentQueue<Tuple<Controller,bool>> controllerQueue;

        private List<IView> views;
        private ConcurrentQueue<Tuple<IView,bool>> viewQueue;

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
            this.controllerQueue = new ConcurrentQueue<Tuple<Controller,bool>>();
            this.viewQueue = new ConcurrentQueue<Tuple<IView,bool>>();

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
            while (!controllerQueue.IsEmpty)
            {
                Tuple<Controller,bool> cont;
                if (controllerQueue.TryDequeue(out cont))
                {
                    if (cont.Item2)
                    {
                        if (!controllers.Contains(cont.Item1))
                            controllers.Add(cont.Item1);

                    }
                    else
                    {
                        if (controllers.Contains(cont.Item1))
                            controllers.Remove(cont.Item1);
                    }
                }
            }

            if (controllers.Count() != 0)
            {
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

            //We update the world
            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            camera.Update(gameTime);
        }

        
        public void Draw(GameTime gameTime)
        {
            while (!viewQueue.IsEmpty)
            {
                Tuple<IView, bool> cont;
                if (viewQueue.TryDequeue(out cont))
                {
                    if (cont.Item2)
                    {
                        if (!views.Contains(cont.Item1))
                        {
                            views.Add(cont.Item1);
                            views = views.OrderBy(a => a.Layer).ToList();
                        }

                    }
                    else
                    {
                        if (views.Contains(cont.Item1))
                            views.Remove(cont.Item1);
                    }
                }

            }


           
            if (views.Count() != 0)
            {

                bool staticPosLast = false;
                int viewsCount = views.Count();
                for(var i =0; i < viewsCount; i++)
                {
                    if (i != 0 && staticPosLast != views[i].StaticPosition)
                    {
                        batch.End();
                    }

                    if (i == 0 || staticPosLast != views[i].StaticPosition)
                    {
                        if (views[i].StaticPosition)
                            batch.Begin();
                        else
                            batch.Begin(0, null, null, null, null, null, camera.View);
                    }

                    views[i].Draw(batch, gameTime);


                    staticPosLast = views[i].StaticPosition;
                }
                batch.End();
            }


            if (Constants.DebugMode)
            {
                Matrix projection = camera.SimProjection;
                Matrix matrixView = camera.SimView;

                debugView.RenderDebugData(ref projection, ref matrixView);
            }
        }

        public void AddView(IView view)
        {
            if (!views.Contains(view))
            {
                view.IsActive = true;
                viewQueue.Enqueue(new Tuple<IView, bool>(view, true));
            }
            else
            {
                Debug.WriteLine("Active views already contains this view");
            }
        }

        public void RemoveView(IView view)
        {
            if (views.Contains(view))
            {
                view.IsActive = false;
                viewQueue.Enqueue(new Tuple<IView,bool>(view,false));
            }
            else
            {
                Debug.WriteLine("Active views don't contain this view");
            }
        }

        public void AddController(Controller controller)
        {
            if (!controllers.Contains(controller))
            {
                controllerQueue.Enqueue(new Tuple<Controller,bool>(controller,true));
            }
            else
            {
                Debug.WriteLine("Active controllers already contains this controller");
            }
        }

        public void RemoveController(Controller controller)
        {
            if (controllers.Contains(controller))
            {
                controllerQueue.Enqueue(new Tuple<Controller,bool>(controller, false));
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
