using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmashBros.Model;
using SmashBros.System;
using FarseerPhysics.Dynamics;

namespace SmashBros.Controllers
{
    public enum MenuState
    {
        StartScreen, CharacterSelection, MapSelection, Options
    }

    public class MenuController : Controller
    {
        MenuState state;
        ImageTexture startScreen;
        ImageTexture characterScreen;
        List<Map> mapModels;
        List<Character> characterModels;
        List<Sprite> characterTextures; 

        public MenuController(ScreenController screen) : base(screen)
        {

        }

        public MenuController(ScreenController screen, MenuState state) : base(screen)
        {
            this.state = state;
        }

        public override void Load(ContentManager content)
        {
            startScreen = new ImageTexture(content, "StartScreen", 0, 0);
            characterScreen = new ImageTexture(content, "SelectionScreen", 0, 0);

            LoadCharacters(content);

            mapModels = Serializing.LoadMaps();
        }

        private void LoadCharacters(ContentManager content)
        {
            characterModels = Serializing.LoadCharacters();
            characterTextures = new List<Sprite>();
            int i = 0;
            int row = 0, col = 0;
            
            foreach (Character character in characterModels)
            {
                var sprite = new Sprite(content, character.thumbnail, Constants.ThumbWith, 250);
                sprite.BoundRect(screen.world, col * Constants.ThumbWith + 200, row * Constants.ThumbHeight + 210, Constants.ThumbWith, Constants.ThumbHeight, BodyType.Static);

                characterTextures.Add(sprite);
                if (i == 4)
                {
                    row++;
                    col = 0;
                }
                else col++;
                i++;
            }
        }

        public override void Unload()
        {
            startScreen.Dispose();
            characterScreen.Dispose();

            foreach (var character in characterTextures)
            {
                character.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case MenuState.StartScreen:
                    if (!startScreen.IsActive)
                        AddView(startScreen);

                    if (IsKeyPressed(Keys.Enter))
                    {
                        RemoveView(startScreen);
                        state = MenuState.CharacterSelection;
                    }

                    break;
                case MenuState.CharacterSelection:
                    if (!characterTextures.First().IsActive)
                    {
                        AddView(characterScreen);
                        foreach(Sprite character in characterTextures){
                            AddView(character);
                        }
                    }

                    if (IsKeyPressed(Keys.Enter))
                    {
                        RemoveView(characterScreen);
                        foreach (Sprite character in characterTextures)
                        {
                            RemoveView(character);
                        }
                        state = MenuState.MapSelection;
                    }



                    break;
                case MenuState.MapSelection:
                    if (!characterScreen.IsActive)
                    {
                        AddView(characterScreen);
                        
                    }

                    if (IsKeyPressed(Keys.Enter))
                    {
                        RemoveView(characterScreen);
                        this.IsActive = false;
                        ActivateController(new GameController(screen, characterModels.GetRange(0,4), mapModels.First()));
                    }
                    break;
                
            }

            if (IsKeyPressed(Keys.Escape))
            {
                int prevState = (int)state;
                if (prevState != 0)
                {
                    prevState--;
                }

                state = (MenuState)prevState;
            }
            
        }
    }
}
