using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using System.Threading;
using SmashBros.MySystem;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace SmashBros.Controllers
{
    public abstract class Controller : IObserver<GameStateManager>
    {
        protected ScreenManager Screen;

        public bool IsActive = false;

        /// <summary>
        /// A controller must take in a screen because the 
        /// screen is used to essential features of the controller
        /// </summary>
        /// <param name="screen"></param>
        public Controller(ScreenManager screen)
        {
            this.Screen = screen;
        }

        /// <summary>
        /// Here the controller should load its content
        /// Runs when the controller is added for first time
        /// When Controller is disposed the Load is run once more
        /// </summary>
        public abstract void Load(ContentManager content);
        
        /// <summary>
        /// Runs when the controller is disoposed
        /// Her all the controllers and views the controller holds should be disposed
        /// </summary>
        public abstract void Unload();
        
        /// <summary>
        /// Runs on game update
        /// </summary>
        public abstract void Update(GameTime gameTime);
       
        /// <summary>
        /// Runs when the current state of game is updated
        /// Tells which main state the game is in
        /// </summary>
        public abstract void OnNext(GameStateManager value);

        /// <summary>
        /// Is run when the controller is removed from the active controller list
        /// Should be used de remove other controller from active list
        /// And if wanted remove other view
        /// </summary>
        public abstract void Deactivate();

        #region Add & Remove Views
        /// <summary>
        /// Adds the view to the views the screenController need to draw
        /// Marks the view active
        /// </summary>
        /// <param name="view">View to activate</param>
        protected void AddView(params IView[] view)
        {
            foreach (var v in view)
            {
                if (!v.IsActive)
                    Screen.ControllerViewManager.AddView(v);
            }
        }

        /// <summary>
        /// Removes the view so the screen stops drawing this view
        /// Marks the view not active
        /// </summary>
        /// <param name="view">View to deactivate</param>
        protected void RemoveView(params IView[] view)
        {
            foreach (var v in view)
            {
                if (v.IsActive)
                    Screen.ControllerViewManager.RemoveView(v);
            }
        }
        /// <summary>
        /// Disposes the view
        /// removes it from draw list and runs dispose on the view
        /// </summary>
        public void DisposeView(params IView[] views)
        {
            foreach (var view in views)
            {
                Screen.ControllerViewManager.DisposeView(view);                
            }
        }
        
        #endregion

        #region Add & Remove Controllers
        /// <summary>
        /// Add controller to the active controller list
        /// </summary>
        /// <param name="controller">controller to activate</param>
        protected void AddController(Controller controller)
        {
            if (!controller.IsActive)
            {
                Screen.ControllerViewManager.AddController(controller);
            }
        }

        /// <summary>
        /// Removes controller from active controller list
        /// </summary>
        /// <param name="controller">controller to deactivate</param>
        protected void RemoveController(params Controller[] controller)
        {
            foreach (var c in controller)
            {
                Screen.ControllerViewManager.RemoveController(c);
            }
        }

        /// <summary>
        /// Dispose the controller
        /// </summary>
        /// <param name="controller">controller to unload</param>
        protected void DisposeController(params Controller[] controller)
        {
            foreach (var c in controller)
            {
                Screen.ControllerViewManager.DisposeController(c);
            }
        } 
        #endregion
        
        /// <summary>
        /// The farseer worl object
        /// </summary>
        public World World 
        { 
            get { return Screen.ControllerViewManager.world; } 
        }
        /// <summary>
        /// Which main state the whole game i in now. 
        /// When updated, all the active controllers are notified about the update
        /// </summary>
        public GameState CurrentState { get { return Screen.gameStateManager.CurrentState; } set { Screen.gameStateManager.CurrentState = value; } }

        /// <summary>
        /// Get font by it's name
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        public SpriteFont GetFont(string fontName)
        {
            return Screen.fonts[fontName];
        }

        /// <summary>
        /// Returns the default font
        /// </summary>
        public SpriteFont FontDefualt { get { return Screen.fonts["Impact"]; } }


        /// <summary>
        /// If set to true the controller is subscirbed to the observable gamestate
        /// And OnNext is triggered if gamestate is changed
        /// </summary>
        public bool SubscribeToGameState
        {
            set
            {
                if (value)
                {
                    Screen.gameStateManager.Add(this);
                }
                else
                {
                    Screen.gameStateManager.Remove(this);
                }
            }
        }

        /// <summary>
        /// The gamepad controllers
        /// </summary>
        public List<GamepadController> GamePadControllers { get { return Screen.gamePads; } }


        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void DebugWrite(string name, params object[] values)
        {
            if (Constants.DebugMode)
            {
                if (Screen.ControllerViewManager.debugView.DebugStuff.ContainsKey(name))
                    Screen.ControllerViewManager.debugView.DebugStuff[name] = string.Join(" & ", values);
                else
                    Screen.ControllerViewManager.debugView.DebugStuff.Add(name, string.Join(" & ", values));
            }   
        }
    }
}
