using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;
using Microsoft.Xna.Framework;

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
            screen.views.Add(view);
        }

        protected void Remove(IView view)
        {
            screen.views.Remove(view);
        }
    }
}
