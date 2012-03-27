using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;

namespace SmashBros.Controllers
{
    public class MenuController : Controller
    {
        public MenuController(ScreenController screen) : base(screen)
        {
            List<Views.MenuEntry> entrys = new List<Views.MenuEntry>();
            
            AddView(new MenuView(entrys));
        }

        public override void Load(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
