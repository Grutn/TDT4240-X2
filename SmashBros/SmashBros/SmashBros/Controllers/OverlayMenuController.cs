using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.MySystem;
using SmashBros.Views;
using Microsoft.Xna.Framework.Input;
using SmashBros.Models;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace SmashBros.Controllers
{
    [Flags]
    public enum PopupState
    {
        Removed=1, Colapsed=2, GamePause=3, Options=4, Help=5
    }

    public class OverlayMenuController : Controller
    {
        ImageController bg;
        MenuView menuView;
        MenuEntry[] gamePauseMenu, optionsMenu, helpMenu;
        PopupState state;

        public PopupState State
        {
            get { return state; }
            set
            {
                if (state != value && !Disabled)
                {
                    this.state = value;
                    UpdateAccordingToState();
                }
            }
        }

        public bool Disabled;

        public OverlayMenuController(ScreenManager screen) : base(screen)
        {
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
            Screen.cursorsController.OnCursorClick += OnCursorClick;
            Screen.cursorsController.OnCursorCollision += OnCursorCollision;
            Screen.cursorsController.OnCursorSeparation += OnCursorSeparation;

            SubscribeToGameState = true;
        }
        
        #region Different menus

        private void createGamePauseMenu(SpriteFont font)
        {
            gamePauseMenu = new MenuEntry[]
            {
                new MenuEntry("Go to Character Selection", gameMenuClick),
                new MenuEntry("Go to Map Selection", gameMenuClick),
                new MenuEntry("Help", helpMenuShow),
                new MenuEntry("Close Menu", closeMenuBox),
                new MenuEntry("Exit Game", exitGame)
            };
        }

        private void createOptionsMenu(SpriteFont font)
        {
            var l = Screen.GameOptions.CreateMenu(optionsMenuClick);
            l.Add(new MenuEntry("Close Menu", closeMenuBox));
            l.Add(new MenuEntry("Exit Game", exitGame));

            optionsMenu = l.ToArray();
        }

        /// <summary>
        /// Create the help menu is clicked on
        /// </summary>
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

        /// <summary>
        /// Runs when game menu is clicked on
        /// </summary>
        /// <param name="index">entry Index</param>
        private void gameMenuClick(int index)
        {
            switch (index)
            {
                case 0:
                    CurrentState = GameState.CharacterMenu;
                    //Deselects players character so menu don't think it's selected
                    foreach (var pad in GamePadControllers)
                    {
                        pad.PlayerModel.SelectedCharacter = null;
                    }
                    break;
                case 1:
                    CurrentState = GameState.MapsMenu;
                    break;
            }

            Disabled = false;
            State = PopupState.Colapsed;
            AddController(new MenuController(Screen));

        }
        /// <summary>
        /// Runs when help menu is clicked on
        /// Pints the playser controller
        /// </summary>
        /// <param name="index">entry Index</param>
        private void helpMenuClick(int index)
        {
            menuView.SetEntries(World,
                new MenuEntry(GamePadControllers[index].PlayerModel.HelpToString(), null) { scale = 0.7f },
                new MenuEntry("Back", helpMenuShow),
                new MenuEntry("Close Menu", closeMenuBox)
                );
        }

        /// <summary>
        /// Runs when back button in help is clicked on
        /// Prints the first help menu
        /// </summary>
        /// <param name="index">entry Index</param>
        private void helpMenuShow(int index)
        {
            menuView.SetEntries(World, helpMenu);
        }

        /// <summary>
        /// Runs when options menu is clicked on
        /// </summary>
        /// <param name="index">entry Index</param>
        private void optionsMenuClick(int index)
        {
            if (index == 1)
            {
                if (Screen.GameOptions.UseLifes)
                {
                    Screen.GameOptions.Lifes = Screen.GameOptions.Lifes == 9 ? 1 : Screen.GameOptions.Lifes + 1;
                }
                else
                {
                    Screen.GameOptions.Minutes = Screen.GameOptions.Minutes == 9 ? 1 : Screen.GameOptions.Minutes + 1;
                }
            }
            else if (index == 0)
            {
                Screen.GameOptions.UseLifes = !Screen.GameOptions.UseLifes;
            }

            Serializing.SaveGameOptions(Screen.GameOptions);
            menuView.SetEntries(World,Screen.GameOptions.CreateMenu(optionsMenuClick).ToArray());
            menuView.AddEntries(World, new MenuEntry("Close Menu", closeMenuBox),
            new MenuEntry("Exit Game", exitGame));
        }

        
        /// <summary>
        /// Runs when a exit game button is clicked
        /// </summary>
        /// <param name="index">entry Index</param>
        private void exitGame(int index)
        {
            Screen.Exit();
        }
        
        #endregion

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

                menuView.Entries.ForEach(a => a.selected = false);
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

            if (CurrentState == GameState.MapsMenu || CurrentState ==  GameState.CharacterMenu)
                this.State = PopupState.Colapsed;
            else
                this.State = PopupState.Removed;

            //Unpauses the game if in gamepuase
            if (CurrentState == GameState.GamePause)
            {
                CurrentState = GameState.GamePlay;
            }
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

        /// <summary>
        /// Updates the menu according to which stat it is in
        /// </summary>
        private void UpdateAccordingToState()
        {
            //Only run if bt is loaded
            if (bg != null)
            {
                Category cursorCategory = Category.Cat5;
                int y = -100;
                switch (state)
                {
                    case PopupState.Removed:
                        y = -700;
                        break;
                    case PopupState.Colapsed:
                        y = -610;
                        break;
                    case PopupState.GamePause:
                        break;
                    case PopupState.Options:
                        break;
                    case PopupState.Help:
                        break;

                }

                //If y == -100  then we know the menubox is open
                if (y == -100)
                {
                    cursorCategory = Category.Cat6;
                    //Adds back button listener on gamepad
                    foreach (var pad in GamePadControllers)
                    {
                        pad.OnBackPress += closeMenuBox;
                        pad.OnSuperKeyPressed += closeMenuBox;
                        pad.OnStartPress += closeMenuBox;
                    }
                }
                else
                {
                    menuView.DisposeEntries();
                    foreach (var pad in GamePadControllers)
                    {
                        pad.OnBackPress -= closeMenuBox;
                        pad.OnSuperKeyPressed -= closeMenuBox;
                        pad.OnStartPress -= closeMenuBox;

                    }

                    RemoveView(menuView);
                }

                bg.IsVisible = true;
                bg.AnimatePos(175, y, 500, false);
                Screen.cursorsController.SetCursorCollisionCategory(cursorCategory);
            }
        }
    }
}
