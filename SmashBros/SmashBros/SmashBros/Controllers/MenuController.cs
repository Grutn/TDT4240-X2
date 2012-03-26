using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SmashBros.Controllers
{
    public class MenuController : Controller
    {
        GameController game;

        public MenuController(ScreenController screen) : base(screen)
        {

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
            if (game != null)
            {
                game.Update(gameTime);
            }
        }
    }
}
