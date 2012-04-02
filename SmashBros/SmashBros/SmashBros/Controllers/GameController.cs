using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.Model;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SmashBros.System;
using Microsoft.Xna.Framework.Input;

namespace SmashBros.Controllers
{
    class GameController : Controller
    {
        MapController map;
        List<CharacterController> characters;
        Sprite spiderMan;

        public GameController(ScreenController screen, List<Character> selectedCharacters, Map selectedMap) : base(screen)
        {
        }

        public override void Load(ContentManager content)
        {
            spiderMan = new Sprite(content, "spiderman", 100, 100, 200, 200);
            spiderMan.BoundRect(World, 100, 100);
            AddView(spiderMan);
            World.Gravity = new Vector2(0, 10);

            var s = new Sprite(content, "spiderman", 900, 20, 600, 700);
            s.BoundRect(World, 900, 200, BodyType.Static);
            AddView(s);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (IsKeyDown(Keys.D))
            {
            }else if (IsKeyDown(Keys.A))
            {
            }else if (IsKeyDown(Keys.W))
            {
            }
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public override void OnNext(GameStateManager value)
        {
            throw new NotImplementedException();
        }
    }
}
