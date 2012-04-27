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

        //Controllers that menucontroller holds
        OverlayMenuController popupMenu;
        CursorController cursors;
        GamePlayController gameController;
        
        //Models tha controller holds
        Map selectedMap;
        GameOptions gameOptions;
        List<Map> mapModels;
        public List<Character> characterModels;

        ImageTexture startScreen;
        ImageTexture selectionScreen;
        TextBox tipsText;
        TextBox continueText;
        List<TextBox> playerSelect;

        //Index of character that players hover
        public Dictionary<int, int> characterHover;
        
        //Character images
        List<Sprite> characterThumbs;
        List<ImageTexture> characterImages;

        //Mapimages
        List<Sprite> mapThumbs;

        Body optionsBox, helpBox;

        public MenuController(ScreenManager screen) : base(screen)
        {
            this.characterHover = new Dictionary<int, int>();
        }

        public MenuController(ScreenManager screen, MenuState state) : this(screen)
        {
        }

        public override void Load(ContentManager content) 
        {
            gameOptions = Serializing.LoadGameOptions();
            //Creates the controller for the cursor
            cursors = new CursorController(Screen);
            cursors.OnCursorClick += OnCursorClick;
            cursors.OnCursorCollision += OnCursorCollision;
            cursors.OnCursorSeparation += OnCursorSeparation;
            AddController(cursors);

            //Adds the popupmenu to the controllers stack
            popupMenu = new OverlayMenuController(Screen, cursors, gameOptions);
            AddController(popupMenu);

            

            //Loads the background for selectionmenuy and startscreen
            startScreen = new ImageTexture(content, "Menu/StartScreen", 0, 0);
            startScreen.StaticPosition = true;
            startScreen.OnAnimationDone += BgImageAnimationDone;
            selectionScreen = new ImageTexture(content, "Menu/SelectionScreen", 0, 0);
            selectionScreen.StaticPosition = true;
            selectionScreen.OnAnimationDone += BgImageAnimationDone;

            //Initialize tips text
            tipsText = new TextBox("Press H for helpmenu", FontDefualt, 10, 690, Color.White,0.8f);
            tipsText.Layer = 100;

            //Init continiue text
            continueText = new TextBox("Press ENTER to continue", GetFont("Impact.large"), 400, 320, Color.White, 1f);
            continueText.StaticPosition = true;
            continueText.Layer = 1002;
            continueText.TextBackground = Draw.ColoredRectangle(Screen.GraphicsDevice, 600, 80, Color.Red);
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

            Screen.soundController.Load(content, this);

            MenuSound.Invoke(MenuSoundType.choose);

            GamePadControllers.ForEach(a => a.OnStartPress += OnStartPress);
            SubscribeToGameState = true;
        }

        private void LoadCharacters(ContentManager content)
        {
            //Loads the characters from the xml files
            characterModels = Serializing.LoadCharacters();
            
            characterThumbs = new List<Sprite>();
            characterImages = new List<ImageTexture>();

            int i = 0;
            int row = 0, col = 0;

            //Create textures for thumb and image for every character model
            foreach (Character character in characterModels)
            {
                //Create the character selection thumbnail, placing it at row and col
                var sprite = new Sprite(content, character.thumbnail, Constants.ThumbWith, 250, col * Constants.ThumbWith + 200, row * Constants.ThumbHeight + 210);
                sprite.StaticPosition = true;
                //Create the boundingbox for PlayerCursor collision detectuib
                sprite.BoundRect(World, Constants.ThumbWith-10, Constants.ThumbHeight-10, BodyType.Static);
                //Collision detection functions
                sprite.BoundBox.IsSensor = true;
                sprite.BoundBox.Enabled = false;
                sprite.Category = Category.Cat5;

                sprite.Layer = 2;
                //Add the model as user data
                sprite.UserData = character;
                //Add the thumb to the thumb list
                characterThumbs.Add(sprite);

                //Check if new row
                if (i == 4)
                {
                    row++;
                    col = 0;
                }
                else col++;
                i++;

                //Creates the images for every texture
                var img = new ImageTexture(content, character.image);
                img.Layer = 5;
                img.UserData = new List<int>();
                characterImages.Add(img);
            }
        }

        private void LoadMaps(ContentManager content)
        {
            //Loads all the map models from json textfiles
            mapModels = Serializing.LoadMaps();
            mapThumbs = new List<Sprite>();

            int row = 0, column = 0;
            foreach (var map in mapModels)
            {
                Sprite s = new Sprite(content, map.thumbImage, 300, 210, 300 * column + 200, row * 230 + 180);
                s.BoundRect(World, 300, 210, BodyType.Static);
                s.BoundBox.IsSensor = true;
                s.Layer = 3;
                s.Category = Category.Cat5;
                s.StaticPosition = true;
                s.BoundBox.Enabled = false;
                s.UserData = map;
                

                mapThumbs.Add(s);
                if (column == 3)
                {
                    row++;
                    column = 0;
                }else
                    column++;
            }

        }
        
        public override void Unload()
        {
            startScreen.Dispose();
            selectionScreen.Dispose();

            foreach (var character in characterThumbs)
            {
                character.Dispose();
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
            RemoveView(startScreen);
            RemoveView(selectionScreen);
        }

        public override void OnNext(GameStateManager value)
        {
            ImageTexture animateIn = null, animateOut = null;
            switch (value.PreviousState)
            {
                case GameState.StartScreen:
                    animateOut = startScreen;
                    RemoveView(tipsText);
                    break;
                case GameState.CharacterMenu:
                    CharacterSelectionVisible = false;
                    break;

                case GameState.MapsMenu :
                    MapSelectionVisible = false;
                    break;
            }

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    AddView(startScreen);
                    animateIn = startScreen;

                    AddView(tipsText);
                    break;
                case GameState.CharacterMenu :
                    AddView(selectionScreen);
                    if(value.PreviousState != GameState.MapsMenu)
                        animateIn = selectionScreen;

                    CharacterSelectionVisible = true;
                    break;

                case GameState.MapsMenu:
                    MapSelectionVisible = true;
                    break;
                case GameState.GamePlay:
                    if (gameController == null)
                    {
                        gameController = new GamePlayController(Screen, selectedMap);
                        AddController(gameController);
                        RemoveView(selectionScreen);
                    }
                    break;
                case GameState.GamePause:
                    break;
            }

            if (animateIn != null)
            {
                animateIn.Layer = 2;
                animateIn.Position(-1280, 0);
                animateIn.Animate(0, 0, 600, false, 1f);
            }
            if (animateOut != null)
            {
                animateOut.Layer = 1;
                animateOut.Position(0, 0);
                animateOut.Animate(1280, 0, 600, false, 1f);
            }
        }

        private void OnCursorClick(int playerIndex, object targetData, CursorModel cursor, bool selectKey)
        {
            if (cursor.TargetCategory == Category.Cat5 || cursor.TargetCategory == Category.Cat6)
            {
                if (targetData.GetType() == typeof(string))
                {
                    string s = targetData.ToString();
                    if (s == "options")
                    {
                        popupMenu.State = PopupState.Options;
                    }
                    else if (s == "help")
                    {
                        popupMenu.State = PopupState.Help;
                    }
                }
                else
                {
                    switch (CurrentState)
                    {
                        case GameState.StartScreen:
                            break;
                        case GameState.CharacterMenu:
                            if (selectKey)
                            {
                                GamePadControllers[playerIndex].SelectedCharacter = (Character)targetData;
                                cursors.DisableCursor(playerIndex);
                            }
                            else
                            {
                                GamePadControllers[playerIndex].SelectedCharacter = null;
                                cursors.EnableCursor(playerIndex);
                            }
                            break;
                        case GameState.MapsMenu:
                            if (selectKey)
                            {
                                selectedMap = (Map)targetData;
                                cursors.EnableCursors = false;
                                AddView(continueText);
                            }
                            else
                            {
                                RemoveView(continueText);
                                cursors.EnableCursors = true;
                            }

                            break;
                        case GameState.GamePlay:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void OnCursorCollision(int playerIndex, object targetData, CursorModel cursor)
        {
            if (targetData == null) return;
            if (targetData.GetType() == typeof(Character))
            {
                Character c = (Character)targetData;
                int index = characterModels.IndexOf(c);

                if (characterHover.ContainsKey(playerIndex))
                    characterHover[playerIndex] = index;
                else characterHover.Add(playerIndex, index);

                characterThumbs[index].Scale = 1.05f;
                characterThumbs[index].Rotation = 0.05f;
                ImageTexture img = characterImages[index];
                if (!img.IsActive)
                {
                    AddView(img);
                }

                img.AddPosition(playerIndex * 260, 450, playerIndex);
            }else if(targetData.GetType() == typeof(Map)){
                var map = mapThumbs[mapModels.IndexOf((Map)targetData)];
                map.Rotation = 0.01f;
                map.Scale = 1.1f;
            }
        }

        private void OnCursorSeparation(int playerIndex, object targetData, CursorModel cursor)
        {
            if (cursor.TargetCategory == Category.Cat5)
            {
                if(characterHover.ContainsKey(playerIndex)){
                    var i = characterHover[playerIndex];
                    if (i != -1)
                    {
                        characterImages[i].RemoveId(playerIndex);

                        if (!characterHover.Any(a => a.Value == i && a.Key != playerIndex))
                        {
                            characterThumbs[i].Rotation = 0;
                            characterThumbs[i].Scale = 1f;
                        }

                        characterHover[playerIndex] = -1;
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
            CurrentState = (GameState)MathHelper.Clamp((int)CurrentState -1, 0, 5);
        }

        private void BgImageAnimationDone(ImageTexture target, ImageInfo imagePosition)
        {
            if(imagePosition.CurrentPos.Y <= -10)
                RemoveView(target);

            switch (CurrentState)
            {
                case GameState.StartScreen:
                    break;
                case GameState.CharacterMenu:
                    AddViews(characterThumbs.ToArray());
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

        public event SmashBros.Controllers.SoundController.MenuSound MenuSound;

        private bool MapSelectionVisible
        {
            set
            {
                if (value)
                {
                    foreach (var map in mapThumbs)
                    {
                        AddView(map);
                        map.BoundBox.Enabled = true;
                    }
                    continueText.Text = "Press ENTER to start game";
                    continueText.Scale = 1.2f;
                }
                else
                {
                    foreach (var map in mapThumbs)
                    {
                        map.BoundBox.Enabled = false;
                        RemoveView(map);
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
                    if (!selectionScreen.IsAnimating)
                        characterThumbs.ForEach(a => AddView(a));

                    characterThumbs.ForEach(a => a.BoundBox.Enabled = true);
                    continueText.Text = "Press ENTER to continiue";
                }
                else
                {
                    foreach (var character in characterThumbs)
                    {
                        RemoveView(character);
                        character.BoundBox.Enabled = false;
                    }
                    RemoveViews(characterImages.ToArray());
                    RemoveViews(playerSelect.ToArray());
                    RemoveView(continueText);
                }
            }
        }

    }
}
