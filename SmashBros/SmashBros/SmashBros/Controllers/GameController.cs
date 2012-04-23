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
    /// <summary>
    /// Start and controlls the main gameplay
    /// </summary>
    class GameController : Controller
    {
        MapController map;
        CameraController camera;
        List<CharacterController> characters;

        public GameController(ScreenController screen, Map selectedMap) : base(screen)
        {
            this.characters = new List<CharacterController>();
            this.map = new MapController(screen, selectedMap);
        }

        public override void Load(ContentManager content)
        {
            foreach (var pad in GamePadControllers)
            {
                if (pad.SelectedCharacter != null)
                {
                    var character = new CharacterController(screen, pad);
                    characters.Add(character);
                    AddController(character);
                }
            }
            
            World.Gravity = new Vector2(0, Constants.GamePlayGravity);

            this.camera = new CameraController(screen);
            AddController(camera);

            AddController(this.map);
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
