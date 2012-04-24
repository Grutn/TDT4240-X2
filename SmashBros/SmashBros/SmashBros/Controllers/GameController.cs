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
            int i = 0;
            foreach (var pad in GamePadControllers)
            {
                if (pad.SelectedCharacter != null)
                {
                    var character = new CharacterController(screen, pad, map.CurrentMap.startingPosition[i]);
                    characters.Add(character);
                    AddController(character);

                    i++;
                }

            }
            
            World.Gravity = new Vector2(0, Constants.GamePlayGravity);

            this.camera = new CameraController(screen);
            foreach (var c in characters)
            {
                this.camera.AddTarget(c);
            }
            AddController(camera);

            AddController(this.map);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Deactivate()
        {
        }

        public override void OnNext(GameStateManager value)
        {
        }
    }
}
