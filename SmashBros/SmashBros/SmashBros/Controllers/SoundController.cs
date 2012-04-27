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
    public enum GameSoundType { hit, explotion, death, deathFar }
    public enum PlayerSoundType { Jump, Hit, ChargingHit, ChargedHit, Super, ChargingSuper, ChargedSuper, Kill }
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

        public SoundController(ScreenController screen) : base(screen)
        {
            
        }

        public override void Load(ContentManager content) {}

        public void LoadSelectionMenuSounds(ContentManager content, MenuController controller)
        {
            menuSounds = new Dictionary<MenuSoundType, object>();
            menuSounds.Add(MenuSoundType.choose, content.Load<SoundEffect>("Sound/Menu/chooseCharacter"));
            //menuSounds.Add(MenuSoundType.optionsInOut, content.Load<SoundEffect>("Sound/"));
            //menuSounds.Add(MenuSoundType.toMapSelection, content.Load<SoundEffect>("Sound/"));

            menuSounds.Add(MenuSoundType.characterSelected, new Dictionary<Character, SoundEffect>());
            foreach (Character character in controller.characterModels)
                try { ((Dictionary<Character, SoundEffect>)menuSounds[MenuSoundType.characterSelected]).Add(character, content.Load<SoundEffect>(character.sound_selected)); }
                catch { }
            ((MenuController)controller).MenuSound += PlayMenuSound;
        }
        
        void LoadGameSounds(ContentManager content, GameController controller, string background)
        {
            gameSounds = new Dictionary<GameSoundType, SoundEffect>();
            gameSounds.Add(GameSoundType.hit, content.Load<SoundEffect>("Sound/Wolwerine/pain"));
            gameSounds.Add(GameSoundType.death, content.Load<SoundEffect>("Sound/Game/hit"));
            ((GameController)controller).GameSound += PlayGameSound;

            playerSounds = new Dictionary<int, Dictionary<PlayerSoundType, SoundEffect>>();

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

            try { playerSounds[controller.playerIndex].Add(PlayerSoundType.Jump, content.Load<SoundEffect>(controller.model.sound_jump)); }
            catch { }
            try { playerSounds[controller.playerIndex].Add(PlayerSoundType.Kill, content.Load<SoundEffect>(controller.model.sound_kill)); }
            catch { }
            controller.PlayerSound += PlayPlayerSound;
        }

        public override void Unload()
        {
            
        }

        public void UnloadGameSounds()
        {

        }

        public void UnloadMenuSounds()
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        public override void OnNext(System.GameStateManager value) 
        {
            string background = "";
            switch (value.PreviousState)
            {
                case GameState.StartScreen:
                    break;
                case GameState.SelectionMenu:
                    break;
                case GameState.OptionsMenu:
                    break;
                case GameState.GamePlay:
                    break;
            }

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    break;
                case GameState.SelectionMenu:
                    break;
                case GameState.OptionsMenu:
                    break;
                case GameState.GamePlay:
                    
                    break;
            }

            if (Constants.Music && background != "")
            {
                //_backgroundMusic = content.Load<SoundEffect>(background);
                backgroundMusic = _backgroundMusic.CreateInstance();
                backgroundMusic.IsLooped = true;
                backgroundMusic.Play();
            }
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public delegate void GameSound(GameSoundType soundtype);
        public void PlayGameSound(GameSoundType soundtype)
        {
            SoundEffect effect;
            if(gameSounds.TryGetValue(soundtype, out effect)) effect.Play();
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
