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
using SmashBros.System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace SmashBros.Controllers
{
    public abstract class Controller : IObserver<GameStateManager>
    {
        protected ScreenController screen;

        public bool IsActive = false;

        /// <summary>
        /// A controller must take in a screen because the 
        /// screen is used to essential features of the controller
        /// </summary>
        /// <param name="screen"></param>
        public Controller(ScreenController screen)
        {
            this.screen = screen;
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

        /// <summary>
        /// Adds the view to the views the screenController need to draw
        /// Marks the view active
        /// </summary>
        /// <param name="view">View to activate</param>
        protected void AddView(IView view)
        {
            if (!view.IsActive)
            {
                view.IsActive = true;
                Thread newThread = new Thread(new ParameterizedThreadStart(ControllerViewManager.AddView));
                newThread.Start(view);
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
                view.IsActive = false;
                Thread newThread = new Thread(new ParameterizedThreadStart(ControllerViewManager.RemoveView));
                newThread.Start(view);
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

        /// <summary>
        /// Add controller to the active controller list
        /// </summary>
        /// <param name="controller">controller to activate</param>
        protected void AddController(Controller controller)
        {
            Thread newThread = new Thread(new ParameterizedThreadStart(ControllerViewManager.AddController));
            newThread.Start(controller);
        }

        /// <summary>
        /// Removes controller from active controller list
        /// </summary>
        /// <param name="controller">controller to deactivate</param>
        protected void RemoveController(Controller controller)
        {
            screen.gameStateManager.Add(controller);
            Thread newThread = new Thread(new ParameterizedThreadStart(ControllerViewManager.RemoveController));
            newThread.Start(controller);
        }
        /// <summary>
        /// The farseer worl object
        /// </summary>
        public World World 
        { 
            get { return screen.controllerViewManager.world; } 
        }
        /// <summary>
        /// Which main state the whole game i in now. 
        /// When updated, all the active controllers are notified about the update
        /// </summary>
        public GameState CurrentState { get { return screen.gameStateManager.CurrentState; } set { screen.gameStateManager.CurrentState = value; } }

        public SpriteFont GetFont(string fontName)
        {
            return screen.fonts[fontName];
        }

        public SpriteFont FontDefualt { get { return screen.fonts["Impact"]; } }

        public bool SubscribeToGameState
        {
            set
            {
                if (value)
                {
                    screen.gameStateManager.Add(this);
                }
                else
                {
                    screen.gameStateManager.Remove(this);
                }
            }
        }

        /// <summary>
        /// The gamepad controllers
        /// </summary>
        public List<GamepadController> GamePadControllers { get { return screen.gamePads; } }

        #region Keyboard Functions
        
        /// <summary>
        /// Checks the old keyboard state agianst the current keyboard state to 
        /// determin if a key was pressed
        /// </summary>
        /// <param name="key">Which key was pressed</param>
        /// <returns></returns>
        protected bool IsKeyPressed(Keys key)
        {
            return screen.oldKeyboardState.IsKeyDown(key) && screen.currentKeyboardState.IsKeyUp(key);
        }

        protected bool IsKeyDown(Keys key)
        {
            return screen.currentKeyboardState.IsKeyDown(key);
        }

        protected bool IsKeyUp(Keys key)
        {
            return screen.currentKeyboardState.IsKeyUp(key);
        } 

        #endregion

        public void OnCompleted()
        {
            //throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

    }
}
