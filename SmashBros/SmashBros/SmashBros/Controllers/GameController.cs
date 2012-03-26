using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace SmashBros.Controllers
{
    class GameController : Controller
    {
        MapController map;
        List<CharacterController> characters;

        public GameController(ScreenController screen) : base(screen)
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
            throw new NotImplementedException();
        }
    }
}
