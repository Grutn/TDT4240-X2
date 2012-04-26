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
    public enum PlayerSoundType { jump, hit, chargingHit, chargedHit, super, chargingSuper, chargedSuper, kill }
    public enum GameSoundType { hit, death, deathFar }
    public enum MenuSoundType { choose, characterSelected, toMapSelection, optionsInOut }

    public class SoundController : Controller
    {
        private SoundEffect _backgroundMusic;
        private SoundEffectInstance backgroundMusic;

        // Gameplay sounds:
        private Dictionary<GameSoundType, SoundEffect> gameSounds;
        private Dictionary<int, Dictionary<PlayerSoundType, SoundEffect>> playerSounds;
        
        // Menu sounds:
        private Dictionary<MenuSoundType, object> menuSounds;

        public SoundController(ScreenManager screen) : base(screen)
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
                case GameState.CharacterMenu:
                    background = "";
                    menuSounds = new Dictionary<MenuSoundType, object>();
                    menuSounds.Add(MenuSoundType.choose, content.Load<SoundEffect>("Sound/Menu/chooseCharacter"));
                    //menuSounds.Add(MenuSoundType.optionsInOut, content.Load<SoundEffect>("Sound/"));
                    //menuSounds.Add(MenuSoundType.toMapSelection, content.Load<SoundEffect>("Sound/"));
                    
                    menuSounds.Add(MenuSoundType.characterSelected, new Dictionary<Character, SoundEffect>());
                    foreach (Character character in ((MenuController)controller).characterModels)
                        try { ((Dictionary<Character, SoundEffect>)menuSounds[MenuSoundType.characterSelected]).Add(character, content.Load<SoundEffect>(character.sound_selected)); }
                        catch { }
                    ((MenuController)controller).MenuSound += PlayMenuSound;
                    break;
                case GameState.MapsMenu:
                    background = "";
                    break;
                case GameState.GamePlay:
                    gameSounds = new Dictionary<GameSoundType, SoundEffect>();
                    gameSounds.Add(GameSoundType.hit, content.Load<SoundEffect>("Sound/Wolwerine/pain"));
                    gameSounds.Add(GameSoundType.death, content.Load<SoundEffect>("Sound/Game/hit"));
                    ((GamePlayController)controller).GameSound += PlayGameSound;

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

            SubscribeToGameState = true;
        }

        public void LoadCharacter(ContentManager content, CharacterController controller)
        {
            playerSounds.Add(controller.playerIndex, new Dictionary<PlayerSoundType, SoundEffect>());

            try { playerSounds[controller.playerIndex].Add(PlayerSoundType.jump, content.Load<SoundEffect>(controller.model.sound_jump)); }
            catch { }
            try { playerSounds[controller.playerIndex].Add(PlayerSoundType.kill, content.Load<SoundEffect>(controller.model.sound_kill)); }
            catch { }
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
            SoundEffect effect;
            if(gameSounds.TryGetValue(soundtype, out effect)) effect.Play(volume, pitch, 0.0f);
        }
        public delegate void PlayerSound(int playerIndex, PlayerSoundType soundtype);
        public void PlayPlayerSound(int playerIndex, PlayerSoundType soundtype)
        {
            SoundEffect effect;
            if(playerSounds[playerIndex].TryGetValue(soundtype, out effect)) effect.Play();
        }
        public delegate void MenuSound(MenuSoundType soundtype, Character character = null);
        public void PlayMenuSound(MenuSoundType soundtype, Character character)
        {
            if (soundtype == MenuSoundType.characterSelected)
            {
                SoundEffect effect;
                if (((Dictionary<Character, SoundEffect>)menuSounds[soundtype]).TryGetValue(character, out effect)) effect.Play();
            }
            else
            {
                object effect;
                if (menuSounds.TryGetValue(soundtype, out effect)) ((SoundEffect)effect).Play();
            }
        }
    }
}
