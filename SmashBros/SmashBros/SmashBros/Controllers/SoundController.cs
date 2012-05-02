using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using SmashBros.MySystem;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using SmashBros.Models;
using System.Diagnostics;

namespace SmashBros.Controllers
{

    public class SoundController : Controller
    {
        private SoundEffectInstance backgroundMusic;
        List<SoundEffect> soundEffects;
        Dictionary<string, Tuple<SoundEffect, SoundEffectInstance>> soundLookUp;


        //Used to fade bg sound in
        float toVolume;

        public SoundController(ScreenManager screen) : base(screen)
        {
            this.soundEffects = new List<SoundEffect>();
            this.soundLookUp = new Dictionary<string, Tuple<SoundEffect, SoundEffectInstance>>();
        }

        public override void Load(ContentManager content) {
            var i = getSound(content, "Menu/btn");
            i.Volume = 0.3f;
            i.Pitch = 1f;
        }

        public void LoadSelectionMenuSounds(ContentManager content, MenuController controller, List<CharacterStats> characters)
        {
            getSound(content, "Menu/chooseCharacter");

            foreach (CharacterStats character in characters)
            {
                try
                {
                    getSound(content, character.sound_selected);
                }
                catch (Exception)
                {
                    
                }
            }

            SetBacgroundMusic(content, "Menu/MenuMusic", 0.2f);
        }
        
        public void LoadGameSounds(ContentManager content, GamePlayController controller, string background)
        {
            getSound(content, "Game/hit");
            SetBacgroundMusic(content, background, 0.8f);
        }

        public void LoadCharacterGameSounds(ContentManager content, CharacterStats character)
        {
            getSound(content, character.sound_jump);
            getSound(content, character.sound_kill);
            getSound(content, character.sound_won);
            
            if(character.x != null)
                getSound(content, character.x.hitSound);
            if (character.xLR != null)
                getSound(content, character.xLR.hitSound);
            if (character.xDown != null)
                getSound(content, character.xDown.hitSound);
            if (character.xUp != null)
                getSound(content, character.xUp.hitSound);

            if (character.a != null)
                getSound(content, character.a.hitSound);
            if (character.aLR != null)
                getSound(content, character.aLR.hitSound);
            if (character.aDown != null)
                getSound(content, character.aDown.hitSound);
            if (character.aUp != null)
                getSound(content, character.aUp.hitSound);

        }

        public void SetBacgroundMusic(ContentManager content, string source, float volume)
        {
            if (Constants.Music && !string.IsNullOrEmpty(source))
            {
                if (backgroundMusic != null) backgroundMusic.Stop();

                toVolume = volume;
                backgroundMusic = getSound(content, source);
                backgroundMusic.Stop();
                //backgroundMusic.IsLooped = true;
                backgroundMusic.Volume = 0f;
                backgroundMusic.Play();
            }
        }

        public override void Unload()
        {
            foreach (var s in soundLookUp)
            {
                s.Value.Item2.Dispose();
                s.Value.Item1.Dispose();
            }

            System.GC.SuppressFinalize(this);
        }

        public void Unload(List<string> sounds)
        {
            foreach (var s in sounds)
            {
                if (!string.IsNullOrEmpty(s) && soundLookUp.ContainsKey(s))
                {
                    soundLookUp[s].Item2.Dispose();
                    soundLookUp[s].Item1.Dispose();
                    soundLookUp.Remove(s);
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

        public override void OnNext(GameStateManager value) 
        {
        }

        public override void Deactivate()
        {
        }

        public void PlaySound(string id)
        {
            if (!string.IsNullOrEmpty(id) && soundLookUp.ContainsKey(id))
            {
                soundLookUp[id].Item2.Play();
            }
        }

        private SoundEffectInstance getSound(ContentManager content, string source)
        {
            if (string.IsNullOrEmpty(source)) return null;

            if(soundLookUp.ContainsKey(source))
            {
                return soundLookUp[source].Item2;
            }
            try
            {
                SoundEffect soundEffect = content.Load<SoundEffect>("Sound/" + source);
                soundLookUp.Add(source, Tuple.Create(soundEffect, soundEffect.CreateInstance()));
                return soundLookUp.Last().Value.Item2;
            }
            catch (Exception)
            {

                Debug.WriteLine("Missing sound " + source);
            }

            return null;
            
        }
    }
}
