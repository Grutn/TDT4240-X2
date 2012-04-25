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
using Microsoft.Xna.Framework.Audio;

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

        SoundEffect sound_background;
        SoundEffectInstance sound_instance;
        SoundEffect sound_hit;

        public GameController(ScreenController screen, Map selectedMap) : base(screen)
        {
            this.characters = new List<CharacterController>();
            this.map = new MapController(screen, selectedMap);
        }

        public override void Load(ContentManager content)
        {
            if (Constants.Music)
            {
                sound_background = content.Load<SoundEffect>("Sound/main");
                sound_hit = content.Load<SoundEffect>("Sound/hit");
                sound_instance = sound_background.CreateInstance();
                sound_instance.IsLooped = true;
                sound_instance.Play();
            }
            
            int i = 0;
            foreach (var pad in GamePadControllers)
            {
                if (pad.SelectedCharacter != null)
                {
                    var character = new CharacterController(screen, pad, map.CurrentMap.startingPosition[i]);
                    characters.Add(character);
                    AddController(character);

                    character.HitHappens += (a, b, c, d, e) =>
                    {
                        sound_hit.Play();
                    };
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
