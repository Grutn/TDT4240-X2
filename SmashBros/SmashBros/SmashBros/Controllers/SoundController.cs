using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using SmashBros.System;
using Microsoft.Xna.Framework.Audio;
using SmashBros.Model;

namespace SmashBros.Controllers
{
    public enum PlayerSoundType { jump, hit, chargingHit, chargedHit, super, chargingSuper, chargedSuper }
    public enum GameSoundType { hit, death }
    public enum MenuSoundType { }

    public class SoundController : Controller
    {
        private SoundEffect _backgroundMusic;
        private SoundEffectInstance backgroundMusic;

        // Gameplay sounds:
        private Dictionary<GameSoundType, SoundEffect> gameSounds;
        private Dictionary<int, Dictionary<PlayerSoundType, SoundEffect>> playerSounds;
        
        // Menu sounds:
        private Dictionary<MenuSoundType, SoundEffect> menuSounds;

        public SoundController(ScreenController screen) : base(screen)
        {
            
        }

        public override void Load(ContentManager content) {}

        public void Load(ContentManager content, Controller controller, string background = "")
        {
            Unload();

            switch (CurrentState)
            {
                case GameState.StartScreen:
                    background = "";
                    break;
                case GameState.SelectionMenu:
                    background = "";
                    break;
                case GameState.OptionsMenu:
                    background = "";
                    break;
                case GameState.GamePlay:
                    gameSounds = new Dictionary<GameSoundType, SoundEffect>();
                    gameSounds.Add(GameSoundType.hit, content.Load<SoundEffect>("Sound/Game/hit"));
                    gameSounds.Add(GameSoundType.death, content.Load<SoundEffect>("Sound/Game/hit"));
                    ((GameController)controller).GameSound += PlayGameSound;

                    playerSounds = new Dictionary<int, Dictionary<PlayerSoundType, SoundEffect>>();

                    if (background == null) background = "";
                    break;
            }

            if (Constants.Music && background != "")
            {
                _backgroundMusic = content.Load<SoundEffect>(background);
                backgroundMusic = _backgroundMusic.CreateInstance();
                backgroundMusic.IsLooped = true;
                backgroundMusic.Play();
            }
        }

        public void LoadCharacter(ContentManager content, CharacterController controller)
        {
            playerSounds.Add(controller.playerIndex, new Dictionary<PlayerSoundType, SoundEffect>());
            
            playerSounds[controller.playerIndex].Add(PlayerSoundType.jump, content.Load<SoundEffect>(controller.model.sound_jump));
            controller.PlayerSound += PlayPlayerSound;
        }

        public override void Unload() {
            
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        public override void OnNext(System.GameStateManager value) { }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public delegate void GameSound(GameSoundType soundtype, float volume = 1, float pitch = 0);
        public void PlayGameSound(GameSoundType soundtype, float volume, float pitch)
        {
            gameSounds[soundtype].Play(volume, pitch, 0.0f);
        }
        public delegate void PlayerSound(int playerIndex, PlayerSoundType soundtype);
        public void PlayPlayerSound(int playerIndex, PlayerSoundType soundtype)
        {
            playerSounds[playerIndex][soundtype].Play();
        }
        public delegate void MenuSound(PlayerSoundType soundtype);
        public void PlayMenuSound(MenuSoundType soundtype)
        {
            menuSounds[soundtype].Play();
        }
    }
}
