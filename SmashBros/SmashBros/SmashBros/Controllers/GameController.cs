using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.Model;

namespace SmashBros.Controllers
{
    class GameController : Controller
    {
        MapController map;
        List<CharacterController> characters;


        public GameController(ScreenController screen, List<Character> selectedCharacters, Map selectedMap) : base(screen)
        {
        }

        public override void Load(ContentManager content)
        {
            throw new Exception("GameLoaded");
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
