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

        public abstract void Load(ContentManager content);
        
        public abstract void Unload();
        
        public abstract void Update(GameTime gameTime);
        /// <summary>
        /// Is run when the current state of game is updated
        /// </summary>
        /// <param name="value"></param>
        public abstract void OnNext(GameStateManager value);

        /// <summary>
        /// Is run when the controller is removed from the active controller list
        /// Should be used to remove the controllers views
        /// </summary>
        public abstract void Deactivate();

        #region Add & Remove Views
        /// <summary>
        /// Adds the view to the views the screenController need to draw
        /// Marks the view active
        /// </summary>
        /// <param name="view">View to activate</param>
        protected void AddView(IView view)
        {
            if (!view.IsActive)
            {
                Screen.ControllerViewManager.AddView(view);
            }
        }

        /// <summary>
        /// Removes the view so the screen stops drawing this view
        /// Marks the view not active
        /// </summary>
        /// <param name="view">View to deactivate</param>
        protected void RemoveView(IView view)
        {
            if (view.IsActive)
            {
                Screen.ControllerViewManager.RemoveView(view);

            }
        }

        public void AddViews(params IView[] views)
        {
            foreach (var view in views)
            {
                AddView(view);
            }
        }

        public void RemoveViews(params IView[] views)
        {
            foreach (var view in views)
            {
                RemoveView(view);
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
        protected void RemoveController(Controller controller)
        {
            Screen.ControllerViewManager.RemoveController(controller);
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

        #region Keyboard Functions
        
        /// <summary>
        /// Checks the old keyboard state agianst the current keyboard state to 
        /// determin if a key was pressed
        /// </summary>
        /// <param name="key">Which key was pressed</param>
        /// <returns></returns>
        protected bool IsKeyPressed(Keys key)
        {
            return Screen.oldKeyboardState.IsKeyDown(key) && Screen.currentKeyboardState.IsKeyUp(key);
        }

        protected bool IsKeyPressedReversed(Keys key)
        {
            return Screen.oldKeyboardState.IsKeyUp(key) && Screen.currentKeyboardState.IsKeyDown(key);
        }

        protected bool IsKeyDown(Keys key)
        {
            return Screen.currentKeyboardState.IsKeyDown(key);
        }

        protected bool IsKeyUp(Keys key)
        {
            return Screen.currentKeyboardState.IsKeyUp(key);
        } 

        #endregion

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
