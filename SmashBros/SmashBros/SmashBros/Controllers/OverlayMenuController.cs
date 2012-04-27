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
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace SmashBros.Controllers
{
    public enum PopupState
    {
        Removed, Colapsed, GamePause, Options, Help
    }

    class OverlayMenuController : Controller
    {
        ImageTexture bg;
        MenuView menuView;
        MenuEntry[] gamePauseMenu, optionsMenu, helpMenu;
        CursorController cursors;
        PopupState state;
        public PopupState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    this.state = value;
                    UpdateStatePops();
                }
            }
        }

        public OverlayMenuController(ScreenManager screen, CursorController cursors) : base(screen)
        {
            this.cursors = cursors;
        }

        public override void Load(ContentManager content)
        {
            menuView = new MenuView(FontDefualt);
            menuView.Layer = 1001;
            menuView.StartingPosition = new Vector2(175, 0);
            menuView.StaticPosition = true;

            bg = new ImageTexture(content, "Menu/PopupMenu", 175, -700);
            bg.Layer = 1000;
            bg.StaticPosition = true;
            bg.OnAnimationDone += OnAnimationDone;
            
            //Create the menu models
            createGamePauseMenu(FontDefualt);
            createOptionsMenu(FontDefualt);
            createHelpMenu(FontDefualt);
            //optionsMenu = CreateMenu(FontDefualt, "Game Time 3 min");
            //helpMenu = CreateMenu(FontDefualt, ShowShiit, "Game controlls");


            cursors.OnCursorClick += OnCursorClick;
            cursors.OnCursorCollision += OnCursorCollision;
            cursors.OnCursorSeparation += OnCursorSeparation;

            //GamePadControllers.ForEach(a => a.OnStartPress += OnStartPress);
            SubscribeToGameState = true;
        }

        private void createGamePauseMenu(SpriteFont font)
        {
            gamePauseMenu = new MenuEntry[]
            {
                new MenuEntry("Go to Character Selection", gameMenuClick),
                new MenuEntry("Go to Map Selection", gameMenuClick),
                new MenuEntry("Help", gameMenuClick),
                new MenuEntry("Exit", gameMenuClick)
            };
        }

        private void createOptionsMenu(SpriteFont font)
        {
            optionsMenu = new MenuEntry[]
            {
                new MenuEntry("Time limit : On", gameMenuClick),
                new MenuEntry("Player 2 controllers", gameMenuClick),
                new MenuEntry("Player 3 controllers", gameMenuClick),
                new MenuEntry("Player 4 controllers", gameMenuClick),
                new MenuEntry("Exit Game", exitGame)
            };
        }

        private void createHelpMenu(SpriteFont font)
        {
            helpMenu = new MenuEntry[]
            {
                new MenuEntry("Player 1 controllers", gameMenuClick),
                new MenuEntry("Player 2 controllers", gameMenuClick),
                new MenuEntry("Player 3 controllers", gameMenuClick),
                new MenuEntry("Player 4 controllers", gameMenuClick),
                new MenuEntry("Exit Game", exitGame)
            };
        }

        private void gameMenuClick(int index)
        {
            menuView.SetEntries(World,
                new MenuEntry("Coool", gameMenuClick),
                new MenuEntry("Helo", gameMenuClick),
                new MenuEntry("Help", gameMenuClick),
                new MenuEntry("Exit", gameMenuClick)
            );
        }

        private void exitGame(int index)
        {

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

        private void OnCursorClick(int playerIndex, object targetData, CursorModel cursor, bool selectKey)
        {
            if (cursor.TargetCategory == Category.Cat6)
            {
                MenuEntry m = (MenuEntry)targetData;
                m.action.Invoke(m.entryIndex);
            }
        }

        private void OnCursorSeparation(int playerIndex, object targetData, CursorModel cursor)
        {
            if (cursor.TargetCategory == Category.Cat6)
            {
                UpdateEntry(targetData, false);
            }
        }

        private void OnCursorCollision(int playerIndex, object targetData, CursorModel cursor)
        {
            if (cursor.TargetCategory == Category.Cat6)
            {
                UpdateEntry(targetData, true);
            }
        }

        private void UpdateEntry(object meny, bool select)
        {
            if (meny != null && typeof(MenuEntry) == meny.GetType())
            {
                MenuEntry m = (MenuEntry)meny;
                m.selected = select;
            }
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
                    ShowGamePauseMenu();
                    break;
                case PopupState.Help:
                    ShowGamePauseMenu();
                    break;
                default:
                    break;
            }
        }

        private void ShowGamePauseMenu()
        {
            menuView.SetEntries(World, gamePauseMenu);
            AddView(menuView);
        }

        private void ShowHelpMenu()
        {
            menuView.SetEntries(World, optionsMenu);
            AddView(menuView);
        }

        private void ShowOptionsMenu(){
            menuView.SetEntries(World, helpMenu);
            AddView(menuView);
        }

        private void UpdateStatePops()
        {
            if (bg != null)
            {
                Category cursorCategory = Category.Cat5;
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
                        cursorCategory = Category.Cat6;
                        y = -40;
                        break;
                    case PopupState.Options:
                        cursorCategory = Category.Cat6;
                        break;
                    case PopupState.Help:
                        cursorCategory = Category.Cat6;
                        break;

                }

                if(!bg.IsActive)
                    AddView(bg);

                bg.Animate(175, y, 500, false, 1);
                cursors.SetCursorCollisionCategory(cursorCategory);
            }
        }
    }
}
