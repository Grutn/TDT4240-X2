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
        ImageController bg;
        MenuView menuView;
        MenuEntry[] gamePauseMenu, optionsMenu, helpMenu;
        CursorController cursors;
        PopupState state;
        GameOptions gameOptions;

        public PopupState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    this.state = value;
                    UpdateAccordingToState();
                }
            }
        }

        public OverlayMenuController(ScreenManager screen, CursorController cursors, GameOptions gameOptions) : base(screen)
        {
            this.cursors = cursors;
            this.gameOptions = gameOptions;
        }

        public override void Load(ContentManager content)
        {
            //Initialises the view for the menu entries
            menuView = new MenuView(FontDefualt);
            menuView.Layer = 1001;
            menuView.StartingPosition = new Vector2(175, 0);
            menuView.StaticPosition = true;

            //Loads the menu background
            bg = new ImageController(Screen, "Menu/PopupMenu", 1000, true);
            bg.SetPosition(175, -700);
            //content, "Menu/PopupMenu", 175, -700);
            bg.OnAnimationDone += OnAnimationDone;
            AddController(bg);
            
            //Create the menu models
            createGamePauseMenu(FontDefualt);
            createOptionsMenu(FontDefualt);
            createHelpMenu(FontDefualt);

            //Adds listeners to cursors
            cursors.OnCursorClick += OnCursorClick;
            cursors.OnCursorCollision += OnCursorCollision;
            cursors.OnCursorSeparation += OnCursorSeparation;

            SubscribeToGameState = true;
        }

        private void createGamePauseMenu(SpriteFont font)
        {
            gamePauseMenu = new MenuEntry[]
            {
                new MenuEntry("Go to Character Selection", gameMenuClick),
                new MenuEntry("Go to Map Selection", gameMenuClick),
                new MenuEntry("Help", gameMenuClick),
                new MenuEntry("Exit Game", exitGame)
            };
        }

        private void createOptionsMenu(SpriteFont font)
        {
            var l = gameOptions.CreateMenu(optionsMenuClick);
            l.Add(new MenuEntry("Close Menu", closeMenuBox));
            l.Add(new MenuEntry("Exit Game", exitGame));

            optionsMenu = l.ToArray();
        }

        private void createHelpMenu(SpriteFont font)
        {
            helpMenu = new MenuEntry[]
            {
                new MenuEntry("Player 1 controllers", helpMenuClick),
                new MenuEntry("Player 2 controllers", helpMenuClick),
                new MenuEntry("Player 3 controllers", helpMenuClick),
                new MenuEntry("Player 4 controllers", helpMenuClick),
                new MenuEntry("Close Menu", closeMenuBox),
                new MenuEntry("Exit Game", exitGame)
            };
        }

        private void gameMenuClick(int index)
        {
            switch (index)
            {
                case 0 :
                    CurrentState = GameState.CharacterMenu;
                    break;
                case 1:
                    CurrentState = GameState.MapsMenu;
                    break;
            }
        }

        private void helpMenuClick(int index)
        {
            menuView.SetEntries(World,
                new MenuEntry("Gampad: \n Navigation : D-Pad or Left Stick \n Normal Attack : A button \n Supper Attack : X button", null) { scale = 0.7f},
                new MenuEntry("Back", helpMenuShow),
                new MenuEntry("Close Menu", closeMenuBox)
                );
        }

        private void helpMenuShow(int index)
        {
            menuView.SetEntries(World, helpMenu);
        }

        private void optionsMenuClick(int index)
        {
            if (index == 1)
            {
                if (gameOptions.UseLifes)
                {
                    gameOptions.Lifes = gameOptions.Lifes == 9 ? 1 : gameOptions.Lifes + 1;
                }
                else
                {
                    gameOptions.Minutes = gameOptions.Minutes == 9 ? 1 : gameOptions.Minutes+ 1;
                }
            }
            else if (index == 0)
            {
                gameOptions.UseLifes = !gameOptions.UseLifes;
            }

            Serializing.SaveGameOptions(gameOptions);
            menuView.SetEntries(World, gameOptions.CreateMenu(optionsMenuClick).ToArray());
        }

        private void exitGame(int index)
        {
            Screen.Exit();
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

        private void closeMenuBox(int i)
        {
            if (CurrentState == GameState.MapsMenu || CurrentState == GameState.CharacterMenu)
                this.State = PopupState.Colapsed;
            else
                this.State = PopupState.Removed;
        }

        private void OnAnimationDone(ImageController img, ImageModel pos)
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
                    ShowOptionsMenu();
                    break;
                case PopupState.Help:
                    ShowHelpMenu();
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

        private void UpdateAccordingToState()
        {
            if (bg != null)
            {
                Category cursorCategory = Category.Cat5;
                int y = -100;
                switch (state)
                {
                    case PopupState.Removed:
                        cursorCategory = Category.Cat5;
                        y = -700;
                        break;
                    case PopupState.Colapsed:
                        cursorCategory = Category.Cat5;
                        y = -610;
                        break;
                    case PopupState.GamePause:
                        cursorCategory = Category.Cat6;
                        break;
                    case PopupState.Options:
                        cursorCategory = Category.Cat6;
                        break;
                    case PopupState.Help:
                        cursorCategory = Category.Cat6;
                        break;

                }


                if (y == -100)
                {
                    foreach (var pad in GamePadControllers)
                    {
                        pad.OnBackPress += closeMenuBox;
                        pad.OnSuperKeyPressed += closeMenuBox;
                    }
                }
                else
                {
                    foreach (var pad in GamePadControllers)
                    {
                        pad.OnBackPress -= closeMenuBox;
                        pad.OnSuperKeyPressed -= closeMenuBox;
                    }

                    RemoveView(menuView);
                }

                bg.IsVisible = true;
                bg.AnimatePos(175, y, 500, false);
                cursors.SetCursorCollisionCategory(cursorCategory);
            }
        }
    }
}
