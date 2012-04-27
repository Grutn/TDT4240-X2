using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.System;
using SmashBros.Views;
using Microsoft.Xna.Framework.Input;
using SmashBros.Models;

namespace SmashBros.Controllers
{
    public enum PopupState
    {
        Removed, Colapsed, GamePause, Options, Help
    }

    class PopupMenuController : Controller
    {
        ImageTexture bg;
        MenuView menuView;
        MenuEntry[] gamePauseMenu, optionsMenu, helpMenu;

        private PopupState state;
        public PopupState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    this.state = value;
                    UpdateBgPos();
                }
            }
        }

        public PopupMenuController(ScreenManager screen) : base(screen)
        {

        }

        public override void Load(ContentManager content)
        {
            menuView = new MenuView(FontDefualt);

            bg = new ImageTexture(content, "Menu/PopupMenu", 175, -700);
            bg.Layer = 1000;
            bg.StaticPosition = true;
            bg.OnAnimationDone += OnAnimationDone;
            
            //Create the menu models
            gamePauseMenu = CreateMenu("Go to Character Selection", "Go to Map Selection", "Exit Game");
            optionsMenu = CreateMenu("Game Time 3 min");
            helpMenu = CreateMenu("Game controlls");


            //GamePadControllers.ForEach(a => a.OnStartPress += OnStartPress);
            SubscribeToGameState = true;
        }

        private MenuEntry[] CreateMenu(params string[] labels)
        {
            var m = new MenuEntry[labels.Length];

            for (int i = 0; i < labels.Length; i++)
            {
                m[i] = new MenuEntry(labels[i]);
            }

            return m;
        }


        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void OnNext(GameStateManager value)
        {

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    State = PopupState.Removed;
                    break;
                case GameState.CharacterMenu:
                    State = PopupState.Colapsed;
                    break;
                case GameState.MapsMenu:
                    State = PopupState.Colapsed;
                    break;
                case GameState.GamePlay:
                    State = PopupState.Removed;
                    break;
                case GameState.GamePause:
                    State = PopupState.GamePause;
                    break;
            }

            //UpdateBgPos();
        }

        public override void Deactivate()
        {
        }

        public void DisableCursor(int playerIndex)
        {

        }

        private void OnAnimationDone(ImageTexture img, ImageInfo pos)
        {
            switch (State)
            {
                case PopupState.Colapsed:
                    break;
                case PopupState.GamePause:
                    ShowGamePauseMenu();
                    break;
                case PopupState.Removed:
                    break;
                case PopupState.Options:
                    break;
                case PopupState.Help:
                    break;
                default:
                    break;
            }
        }

        private void ShowGamePauseMenu()
        {
            menuView.SetEntries(gamePauseMenu);
            AddView(menuView);
        }

        private void ShowHelpMenu()
        {

        }

        private void ShowOptionsMenu(){

        }

        private void UpdateBgPos()
        {
            if (bg != null)
            {

                int y = 0;
                switch (state)
                {
                    case PopupState.Removed:
                        y = -700;
                        break;
                    case PopupState.Colapsed:
                        y = -610;
                        break;
                    case PopupState.GamePause:
                        y = -40;
                        break;
                }

                if(!bg.IsActive)
                    AddView(bg);

                bg.Animate(175, y, 500, false, 1);
            }
        }
    }
}
