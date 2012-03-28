using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SmashBros.Controllers
{
    enum MenuState
    {
        StartScreen, CharacterSelection, MapSelection, Options
    }

    public class MenuController : Controller
    {
        MenuState state;
        ImageTexture startScreen;
        ImageTexture characterScreen;

        public MenuController(ScreenController screen) : base(screen)
        {

            List<Views.MenuEntry> entrys = new List<Views.MenuEntry>();
            
            AddView(new MenuView(entrys));
        }

        public override void Load(ContentManager content)
        {
            startScreen = new ImageTexture(content, "StartScreen", 0, 0);
            characterScreen = new ImageTexture(content, "CharacterSelection", 0, 0);
        }

        public override void Unload()
        {

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
                    if (!characterScreen.IsActive)
                    {
                        AddView(characterScreen);
                    }



                    break;
                case MenuState.MapSelection:

                    break;
                
            }
            
        }
    }
}
