using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using SmashBros.MySystem;
using Microsoft.Xna.Framework.Audio;
using SmashBros.Model;
using Microsoft.Xna.Framework;

namespace SmashBros.Controllers
{
    public enum GameSoundType { hit, explotion, death, deathFar }
    public enum PlayerSoundType { Jump, Hit, ChargingHit, ChargedHit, Super, ChargingSuper, ChargedSuper, Kill }
    public enum MenuSoundType { choose, characterSelected, toMapSelection, optionsInOut, btnHover }

    public class SoundController : Controller
    {
        private SoundEffect _backgroundMusic;
        private SoundEffectInstance backgroundMusic;

        // Gameplay sounds:
        private Dictionary<GameSoundType, SoundEffect> gameSounds;
        private Dictionary<int, Dictionary<PlayerSoundType, SoundEffect>> playerSounds;
        
        // Menu sounds:
        private Dictionary<MenuSoundType, object> menuSounds;
        
        List<SoundEffect> soundEffects;

        //Used to fade bg sound in
        float toVolume;

        public SoundController(ScreenManager screen) : base(screen)
        {
            this.soundEffects = new List<SoundEffect>();
        }

        public override void Load(ContentManager content) {}

        public void LoadSelectionMenuSounds(ContentManager content, MenuController controller, List<CharacterStats> characters)
        {
            menuSounds = new Dictionary<MenuSoundType, object>();

            SoundEffectInstance i = createSound(content, "Sound/Menu/chooseCharacter");
            menuSounds.Add(MenuSoundType.choose,i);

            i = createSound(content, "Sound/Menu/btn");
            i.Volume = 0.3f;
            i.Pitch = 1f;
            menuSounds.Add(MenuSoundType.btnHover, i);

            menuSounds.Add(MenuSoundType.characterSelected, new Dictionary<CharacterStats, SoundEffect>());
            foreach (CharacterStats character in characters)
                try { ((Dictionary<CharacterStats, SoundEffect>)menuSounds[MenuSoundType.characterSelected]).Add(character, content.Load<SoundEffect>(character.sound_selected)); }
                catch { }

            SetBacgroundMusic(content, "Sound/Menu/MenuMusic", 0.2f);
        }
        
        public void LoadGameSounds(ContentManager content, GamePlayController controller, string background)
        {
            gameSounds = new Dictionary<GameSoundType, SoundEffect>();
            //gameSounds.Add(GameSoundType.hit, content.Load<SoundEffect>("Sound/Game/hit"));
            //gameSounds.Add(GameSoundType.death, content.Load<SoundEffect>("Sound/Game/hit"));

            playerSounds = new Dictionary<int, Dictionary<PlayerSoundType, SoundEffect>>();

            SetBacgroundMusic(content, background, 0.8f);
            
        }

        public void LoadCharacter(ContentManager content, CharacterController controller)
        {
            playerSounds.Add(controller.model.playerIndex, new Dictionary<PlayerSoundType, SoundEffect>());

            try { playerSounds[controller.model.playerIndex].Add(PlayerSoundType.Jump, content.Load<SoundEffect>(controller.stats.sound_jump)); }
            catch { }
            try { playerSounds[controller.model.playerIndex].Add(PlayerSoundType.Kill, content.Load<SoundEffect>(controller.stats.sound_kill)); }
            catch { }
        }

        public void SetBacgroundMusic(ContentManager content, string source, float volume)
        {
            if (Constants.Music && !string.IsNullOrEmpty(source))
            {
                if (backgroundMusic != null) backgroundMusic.Stop();

                toVolume = volume;
                _backgroundMusic = content.Load<SoundEffect>(source);
                backgroundMusic = _backgroundMusic.CreateInstance();
                backgroundMusic.IsLooped = true;
                backgroundMusic.Volume = 0f;
                backgroundMusic.Play();
            }
        }

        public override void Unload()
        {
            
        }

        public void UnloadGameSounds()
        {

        }

        public void UnloadMenuSounds()
        {
            foreach (var sound in menuSounds)
            {
                object effect;
                if (menuSounds.TryGetValue(sound.Key, out effect)){
                    if (effect.GetType() == typeof(SoundEffectInstance))
                        ((SoundEffectInstance)effect).Stop();    
                }
            }
        }


        float elapsedTime = 0;
        public override void Update(GameTime gameTime)
        {
            if (Constants.Music)
            {
                if (backgroundMusic.Volume != toVolume)
                {
                    elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                    //Uses 1.5 sec to fade in so percent is elapsed/1500
                    if (elapsedTime >= 5000)
                    {
                        elapsedTime = 0;
                        backgroundMusic.Volume = toVolume;
                    }
                    else
                        backgroundMusic.Volume = toVolume * elapsedTime / 5000;

                }
            }
        }

        public override void OnNext(MySystem.GameStateManager value) 
        {
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void PlayGameSound(GameSoundType soundtype)
        {
            SoundEffect effect;
            if(gameSounds.TryGetValue(soundtype, out effect)) effect.Play();
        }

        public void PlayPlayerSound(int playerIndex, PlayerSoundType soundtype)
        {
            SoundEffect effect;
            if(playerSounds[playerIndex].TryGetValue(soundtype, out effect)) effect.Play();
        }

        public void PlayMenuSound(MenuSoundType soundtype, CharacterStats character = null)
        {
            if (soundtype == MenuSoundType.characterSelected)
            {
                SoundEffect effect;
                if (((Dictionary<CharacterStats, SoundEffect>)menuSounds[soundtype]).TryGetValue(character, out effect)) effect.Play();
            }
            else
            {
                object effect;
                if (menuSounds.TryGetValue(soundtype, out effect))
                {
                    SoundEffectInstance s = (SoundEffectInstance)effect;
                    s.Play();
                }      
            }
        }

        private SoundEffectInstance createSound(ContentManager content, string source)
        {
            SoundEffect soundEffect = content.Load<SoundEffect>(source);
            soundEffects.Add(soundEffect);
            return soundEffect.CreateInstance();
        }
    }
}
