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
using SmashBros.MySystem;
using FarseerPhysics.Dynamics;
using System.Diagnostics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using SmashBros.Models;

namespace SmashBros.Controllers
{
    public enum MenuState
    {
        StartScreen,CharacterSelection,MapSelection
    }

    public class MenuController : Controller
    {

        #region Private variables
        //Models tha controller holds
        Map selectedMap;
        List<Map> mapModels;
        /// <summary>
        /// Background for startscreen
        /// </summary>
        ImageController startScreen;
        /// <summary>
        /// Background for selection screen
        /// </summary>
        ImageController selectionScreen;
        /// <summary>
        /// Helping tips to user
        /// </summary>
        TextBox tipsText;
        /// <summary>
        /// ContiniueText box, shown  when enough characters is selected
        /// or map is selected
        /// </summary>
        TextBox continueText;
        /// <summary>
        /// TextFields for player selected
        /// </summary>
        List<TextBox> playerSelect;
        /// <summary>
        /// Index of character that players hover
        /// </summary>
        Dictionary<int, ImageModel> characterHover;
        /// <summary>
        /// Character thumbnail images
        /// </summary>
        List<ImageController> characterThumbs;
        /// <summary>
        ///Map thumbnail sprites
        /// </summary>
        List<ImageController> mapThumbs;
        Dictionary<int,int> playerHoversMap;
        /// <summary>
        ///List wiht images of character poses
        /// </summary>
        List<ImageController> characterImages;
        /// <summary>
        /// Gamestats from a gameplay
        /// </summary>
        List<PlayerStats> gameStats;
        /// <summary>
        /// OptionsButton and helpButton
        /// </summary>
        Body optionsBox, helpBox;

        List<TextBox> gameStatsText;
        #endregion

        List<CharacterStats> characterModels;

        public MenuController(ScreenManager screen) : base(screen)
        {
            this.characterHover = new Dictionary<int, ImageModel>();
            this.playerHoversMap = new Dictionary<int, int>();
        }

        /// <summary>
        /// Constructor is used when game play is over and GamePlayController creates the menu controller agian
        /// </summary>
        public MenuController(ScreenManager screen, List<PlayerStats> gameStats) 
            : this(screen)
        {
            this.gameStats = gameStats;
            this.gameStatsText = new List<TextBox>();
        }

        public override void Load(ContentManager content) 
        {

            Screen.cursorsController.OnCursorClick += OnCursorClick;
            Screen.cursorsController.OnCursorCollision += OnCursorCollision;
            Screen.cursorsController.OnCursorSeparation += OnCursorSeparation;

            //Loads the background for selectionmenuy and startscreen
            startScreen = new ImageController(Screen, "Menu/StartScreen", 1, true);
            startScreen.OnAnimationDone += BgImageAnimationDone;
            
            selectionScreen = new ImageController(Screen, "Menu/SelectionScreen", 1, true);
            selectionScreen.OnAnimationDone += BgImageAnimationDone;

            //Initialize tips text
            tipsText = new TextBox("Press H for helpmenu", FontDefualt, 10, 690, Color.White,0.8f);
            tipsText.Layer = 100;

            //Init continiue text
            continueText = new TextBox("Press ENTER to continue", GetFont("Impact.large"), 250, 320, Color.White, 1f);
            continueText.StaticPosition = true;
            continueText.Layer = 900;
            continueText.TextBackground = Draw.ColoredRectangle(Screen.GraphicsDevice, 900, 80, Color.Red);
            continueText.BackgroundOffset = new Vector2(-70, -5);

//Change tip text if xbox
#if XBOX
            tipsText.Text = "Press back button for help";
#endif
            //Inits the player select labels
            playerSelect = new List<TextBox>(); 

            for (int i = 1; i < 5; i++)
            {
                var l = new TextBox("Player " + i + " DONE!", FontDefualt, 270*(i-1)+60, 680, Color.White, 1.1f);
                l.Layer = 100;
                l.StaticPosition = true;
                l.TextBackground = Draw.ColoredRectangle(Screen.GraphicsDevice, 210, 50, GamePadControllers[i-1].PlayerModel.Color);
                l.BackgroundOffset = new Vector2(-10, -5);
                playerSelect.Add(l);
            }

            //Loads Maps
            LoadMaps(content);
            //Loads characters
            LoadCharacters(content);
            
            //Creates the help and options button
            helpBox = CreateBtn(Constants.WindowWidth - 100, 40, 180, 80, "help");
            optionsBox = CreateBtn(90, 40, 180, 80, "options");

            //Loads the sounds for the menu
            Screen.soundController.LoadSelectionMenuSounds(content, this, characterModels);
            Screen.soundController.PlayMenuSound(MenuSoundType.choose);

            if (gameStats != null)
            {
                foreach (var stats in gameStats)
                {
                    TextBox textBox = new TextBox(stats.ToString(), FontDefualt, new Vector2(300 * stats.PlayerIndex, 400), Color.White);
                    textBox.TextBackground = Draw.ColoredRectangle(Screen.GraphicsDevice, 250, Constants.WindowHeight -400, new Color(237,27,36));
                    textBox.Layer = 4;
                    textBox.StaticPosition = true;
                    gameStatsText.Add(textBox);
                }
            }

            GamePadControllers.ForEach(a => a.OnStartPress += OnStartPress);
            SubscribeToGameState = true;
        }

        private void LoadCharacters(ContentManager content)
        {
            //Loads the characters from the xml files
            characterModels = Serializing.LoadCharacters();
            
            characterThumbs = new List<ImageController>();
            characterImages = new List<ImageController>();

            int i = 0;
            int row = 0, col = 0;

            //Create textures for thumb and image for every character model
            foreach (CharacterStats character in characterModels)
            {

                ImageController thumb = new ImageController(Screen, character.thumbnail, 3, true);
                thumb.IsVisible = false;
                thumb.ScaleDefault = 0.1f;

                ImageModel thumbModel = thumb.SetPosition(col * Constants.ThumbWith + 200, row * Constants.ThumbHeight + 210);
                thumbModel.SetBoundBox(World, Constants.ThumbWith - 20, Constants.ThumbHeight - 20, new Vector2(10, 0),
                    Category.Cat5, Category.Cat4, true);
                thumbModel.BoundBox.IsSensor = true;
                thumbModel.BoundBox.Enabled = false;
                thumbModel.BoundBox.UserData = i;

                AddController(thumb);
                characterThumbs.Add(thumb);

                //Check if new row
                if (i == 4)
                {
                    row++;
                    col = 0;
                }
                else col++;
                i++;

                //Creates the images for every texture
                var img = new ImageController(Screen, character.image, 4, true);
                AddController(img);
                characterImages.Add(img);
            }
        }

        private void LoadMaps(ContentManager content)
        {
            //Loads all the map models from json textfiles
            mapModels = Serializing.LoadMaps();
            mapThumbs = new List<ImageController>();

            int row = 0, column = 0, i = 0;
            foreach (var map in mapModels)
            {
                ImageController thumb = new ImageController(Screen, map.thumbImage, 3, true);
                thumb.IsVisible = false;
                thumb.ScaleDefault = 0.1f;

                ImageModel img = thumb.SetPosition(300 * column + 200, row * 230 + 180);
                img.SetBoundBox(World, 300, 210, Vector2.Zero, Category.Cat5, Category.Cat4, true);
                img.BoundBox.IsSensor = true;
                img.BoundBox.Enabled = false;
                img.BoundBox.UserData = i;

                AddController(thumb);
                mapThumbs.Add(thumb);

                if (column == 3)
                {
                    row++;
                    column = 0;
                }else
                    column++;

                i++;
            }

        }
        
        public override void Unload()
        {

            SubscribeToGameState = false;
            //Removes Controllers
            DisposeController(startScreen, selectionScreen);

            foreach (var item in characterImages)
            {
                DisposeController(item);
            }

            foreach (var thumbs in characterThumbs)
            {
                DisposeController(thumbs);
            }


            foreach (var item in mapThumbs)
            {
                DisposeController(item);
            }

            //Removes Views
            DisposeView(tipsText);
            DisposeView(continueText);

            foreach (var item in playerSelect)
            {
                DisposeView(item);
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch (CurrentState)
            {
                case GameState.CharacterMenu:

                    int playersSelected = 0;
                    foreach (var pad in GamePadControllers)
                    {

                        var playerText = playerSelect[pad.PlayerIndex];
                        if (pad.SelectedCharacter != null)
                        {
                            AddView(playerText);
                            playersSelected++;
                        }
                        else
                        {
                            RemoveView(playerText);
                        }
                    }

                    if (playersSelected >= 2)
                    {
                        AddView(continueText);
                    }
                    else
                    {
                        RemoveView(continueText);
                    }
                    break;
            }
            
        }

        public override void Deactivate()
        {
        }

        #region Observers

        public override void OnNext(GameStateManager value)
        {
            ImageController animateIn = null, animateOut = null;
            switch (value.PreviousState)
            {
                case GameState.StartScreen:
                    if (value.CurrentState != GameState.StartScreen)
                    {
                        animateOut = startScreen;
                        RemoveView(tipsText);
                    }
                    break;
                case GameState.CharacterMenu:
                    CharacterSelectionVisible = false;
                    break;

                case GameState.MapsMenu:
                    MapSelectionVisible = false;
                    break;
                case GameState.GameOver:
                    GameStatsVisible = false;
                    break;
            }

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    AddController(startScreen);
                    animateIn = startScreen;

                    AddView(tipsText);
                    break;
                case GameState.CharacterMenu:
                    AddController(selectionScreen);
                    if (value.PreviousState != GameState.MapsMenu)
                        animateIn = selectionScreen;

                    CharacterSelectionVisible = true;
                    break;

                case GameState.MapsMenu:
                    MapSelectionVisible = true;
                    break;
                case GameState.GamePlay:
                    AddController(new GamePlayController(Screen, selectedMap));
                    DisposeController(this);

                    break;
                case GameState.GamePause:
                    break;
                case GameState.GameOver:
                    AddController(selectionScreen);
                    animateIn = selectionScreen;
                    GameStatsVisible = true;
                    break;
            }

            if (animateIn != null)
            {
                animateIn.Layer = 2;
                animateIn.SetPosition(-1280, 0);
                animateIn.AnimatePos(0, 0, 600, false);
            }
            if (animateOut != null)
            {
                animateOut.Layer = 1;
                animateOut.SetPosition(0, 0);
                animateOut.AnimatePos(1280, 0, 600, false);
            }
        }

        private void OnCursorClick(int playerIndex, object targetData, CursorModel cursor, bool selectKey)
        {
            if (cursor.TargetCategory == Category.Cat5)
            {
                if (targetData.GetType() == typeof(string))
                {
                    string s = targetData.ToString();
                    if (s == "options")
                    {
                        Screen.popupMenuController.State = PopupState.Options;
                    }
                    else if (s == "help")
                    {
                        Screen.popupMenuController.State = PopupState.Help;
                    }
                }
                else
                {
                    switch (CurrentState)
                    {
                        case GameState.CharacterMenu:
                            if (selectKey)
                            {
                                GamePadControllers[playerIndex].SelectedCharacter = characterModels[(int)targetData];
                                Screen.cursorsController.DisableCursor(playerIndex);
                            }
                            else
                            {
                                GamePadControllers[playerIndex].SelectedCharacter = null;
                                Screen.cursorsController.EnableCursor(playerIndex);
                            }
                            break;
                        case GameState.MapsMenu:
                            if (selectKey)
                            {
                                selectedMap = mapModels[(int)targetData];
                                Screen.cursorsController.EnableCursors = false;
                                AddView(continueText);
                            }
                            else
                            {
                                RemoveView(continueText);
                                Screen.cursorsController.EnableCursors = true;
                            }

                            break;
                    }
                }
            }
        }

        private void OnCursorCollision(int playerIndex, object targetData, CursorModel cursor)
        {
            if (cursor.TargetCategory == Category.Cat5)
            {
                if (targetData == null) return;
                //If data is character then show the character pose at the playersIndex pos
                if (targetData.GetType() == typeof(int))
                {
                    switch (CurrentState)
                    {
                        case GameState.CharacterMenu:
                            hoverCharacter(playerIndex, (int)targetData);
                            break;
                        case GameState.MapsMenu:
                            var map = mapThumbs[(int)targetData];
                            map.GetAt(0).CurrentRotation = 0.01f;
                            map.AnimateScale(1.1f, 300, false);

                            if(!playerHoversMap.ContainsKey(playerIndex))
                                playerHoversMap.Add(playerIndex, (int)targetData);
                            else
                                playerHoversMap[playerIndex] = (int)targetData;
                            break;
                    }
                }
            }
        }

        private void OnCursorSeparation(int playerIndex, object targetData, CursorModel cursor)
        {
            if (cursor.TargetCategory == Category.Cat5)
            {
                if (targetData.GetType() == typeof(int))
                {
                    switch (CurrentState)
                    {
                        case GameState.CharacterMenu:
                            hoverOutCharacter(playerIndex, (int)targetData);
                            break;
                        case GameState.MapsMenu:
                            int mapIndex = (int)targetData;

                            //If no other players hovers this map thumb then animate back
                            if (!playerHoversMap.Any(a => a.Value == mapIndex && a.Key != playerIndex))
                            {
                                var map = mapThumbs[(int)targetData];
                                map.GetAt(0).CurrentRotation = 0f;
                                map.AnimateScale(1f, 300, false);
                            }
                            playerHoversMap.Remove(playerIndex);
                            break;
                    }
                }
            }
        }

        private void OnStartPress(int playerIndex)
        {
            if (continueText.IsActive)
            {
                CurrentState = (GameState)(int)CurrentState + 1;
            }
        }

        private void OnBackPress(int playerIndex)
        {
            CurrentState = (GameState)MathHelper.Clamp((int)CurrentState - 1, 0, 5);
        } 

        #endregion

        /// <summary>
        /// Show the character at bottom of screen when cursor hovers a characterThumb
        /// </summary>
        private void hoverCharacter(int playerIndex, int characterIndex)
        {
            //Get the character model
            CharacterStats chModel = characterModels[characterIndex];

            //Get character pose by using index
            ImageController img = characterImages[characterIndex];

            //Checks if player already has an entry for hovered character
            //Removes the hovered character
            if (characterHover.ContainsKey(playerIndex))
            {
                img.RemovePosition(characterHover[playerIndex]);
            }
                //If not hovered character add playerIndex to dictionary
            else characterHover.Add(playerIndex, null);

            ImageModel model = img.AddPosition(playerIndex * 260, 720, playerIndex);
            model.Id = characterIndex;
            img.AnimatePos(model, playerIndex * 260, 450, 300);
            characterHover[playerIndex] = model;

            //characterThumbs[index].Scale = 1.05f;
            characterThumbs[characterIndex].AnimateScale(1.1f, 500);
            characterThumbs[characterIndex].GetAt(0).CurrentRotation = 0.05f;

            img.IsVisible = true;
        }

        private void hoverOutCharacter(int playerIndex, int characterIndex)
        {
            if (characterHover.ContainsKey(playerIndex))
            {
                ImageModel imgModel = characterHover[playerIndex];
                characterImages[characterIndex].RemovePosition(imgModel);
                
                if (!characterHover.Any(a => a.Value.Id == characterIndex && a.Key != playerIndex))
                {
                    characterThumbs[characterIndex].GetAt(0).CurrentRotation = 0;
                    characterThumbs[characterIndex].AnimateScale(1f, 300);
                }

                characterHover[playerIndex] = null;
            }
        }

        private void BgImageAnimationDone(ImageController target, ImageModel imagePosition)
        {
            if (imagePosition.CurrentPos.Y <= -10)
                target.IsVisible = false;

            switch (CurrentState)
            {
                case GameState.StartScreen:
                    break;
                case GameState.CharacterMenu:
                    characterThumbs.ForEach(a => AddController(a));
                    break;
                case GameState.MapsMenu:
                    break;
                case GameState.GamePlay:
                    break;
                default:
                    break;
            }

        }

        private Body CreateBtn(int x, int y, int width, int height, string btnName){
            Body btn = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1f);
            btn.Position = ConvertUnits.ToSimUnits(x, y);
            btn.CollisionCategories = Category.Cat5;
            btn.IsSensor = true;
            btn.UserData = btnName;

            return btn;
        }

        private bool MapSelectionVisible
        {
            set
            {
                if (value)
                {
                    foreach (var map in mapThumbs)
                    {
                        map.IsVisible = true;
                        map.GetAt(0).BoundBox.Enabled = true;
                        map.AnimateScale(1, 300);
                    }
                    continueText.Text = "Press ENTER to start game";
                    continueText.Scale = 1.2f;
                }
                else
                {
                    foreach (var map in mapThumbs)
                    {
                        map.IsVisible = false;
                        map.GetAt(0).BoundBox.Enabled = false;
                    }
                    RemoveView(continueText);
                }
            }
        }

        private bool CharacterSelectionVisible
        {
            set
            {
                if (value)
                {
                    foreach (var thumb in characterThumbs)
                    {
                        thumb.IsVisible = true;
                        thumb.GetAt(0).BoundBox.Enabled = true;
                        thumb.AnimateScale(1, 300);
                    }

                    continueText.Text = "Press ENTER or START button to continiue";
                }
                else
                {
                    foreach (var character in characterThumbs)
                    {
                        character.IsVisible = false;
                        character.GetAt(0).BoundBox.Enabled = false;
                    }
                    characterImages.ForEach(a => a.IsVisible = false);
                    RemoveViews(playerSelect.ToArray());
                    RemoveView(continueText);
                }
            }
        }

        private bool GameStatsVisible
        {
            set{
                if (value)
                {
                    AddViews(gameStatsText.ToArray());
                    continueText.Text = "Press ENTRER or START button to continue";
                    AddView(continueText);
                }
                else
                {
                    RemoveViews(gameStatsText.ToArray());
                }
            }
        }

    }
}
