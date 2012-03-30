using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SmashBros.Controllers
{
    public abstract class Controller
    {
        protected ScreenController screen;

        private bool _isActive = false;

        /// <summary>
        /// The set methods adds the controller to active controllers on the screen,
        /// or removes it if isActive = false
        /// </summary>
        public bool IsActive { 
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (_isActive)
                {
                    screen.ActivateController(this);
                }
                else
                {
                    screen.DeactivateController(this);
                }
            }
        }

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
        /// Adds the view to the views the screenController need to draw
        /// Marks the view active
        /// </summary>
        /// <param name="view">View to activate</param>
        protected void AddView(IView view)
        {
            view.IsActive = true;
            screen.views.Add(view);
        }

        /// <summary>
        /// Removes the view so the screen stops drawing this view
        /// Marks the view not active
        /// </summary>
        /// <param name="view">View to deactivate</param>
        protected void RemoveView(IView view)
        {
            screen.views.Remove(view);
            view.IsActive = false;
        }

        /// <summary>
        /// Another way to activate a controller
        /// only thing it does is to set isActive on the controller to true
        /// </summary>
        /// <param name="controller">controller to activate</param>
        protected void ActivateController(Controller controller)
        {
            controller.IsActive = true;
        }

        /// <summary>
        /// Another way to deactivate a controller
        /// only thing it does is to set isActive on the controller to false
        /// </summary>
        /// <param name="controller">controller to deactivate</param>
        protected void DeactivateController(Controller controller)
        {
            controller.IsActive = false;
        }


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
    }
}
