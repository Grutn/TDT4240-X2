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

        public Controller(ScreenController screen)
        {
            this.screen = screen;
        }

        public abstract void Load(ContentManager content);
        public abstract void Unload();
        public abstract void Update(GameTime gameTime);

        protected void AddView(IView view)
        {
            view.IsActive = true;
            screen.views.Add(view);
        }

        protected bool IsKeyPressed(Keys key){
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

        protected void RemoveView(IView view)
        {
            screen.views.Remove(view);
            view.IsActive = false;
        }
    }
}
