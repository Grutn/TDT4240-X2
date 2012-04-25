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

namespace SmashBros.Controllers
{
    public enum MenuState
    {
        None, CharacterSelectionComplete
    }

    public class MenuController : Controller
    {
        PopupMenuController popupMenu;

        ImageTexture startScreen;
        ImageTexture characterScreen;
        TextBox tipsText;
        TextBox continueText;
        List<TextBox> playerSelect;

        List<Map> mapModels;
        List<Character> characterModels;
        List<Sprite> characterThumbs;
        List<ImageTexture> characterImages;
        List<Sprite> playerCursors;
        Body optionsBox, helpBox;

        public MenuController(ScreenController screen) : base(screen)
        {
        }

        public MenuController(ScreenController screen, MenuState state) : this(screen)
        {
        }

        public override void Load(ContentManager content) 
        {
            screen.soundController.Load(content, this);

            //Adds the popupmenu to the controllers stack
            popupMenu = new PopupMenuController(screen);
            AddController(popupMenu);

            //Loads the background for selectionmenuy and startscreen
            startScreen = new ImageTexture(content, "Menu/StartScreen", 0, 0);
            characterScreen = new ImageTexture(content, "Menu/SelectionScreen", 0, 0);

            //Initialize tips text
            tipsText = new TextBox("Press H for helpmenu", FontDefualt, 10, 690, Color.White,0.8f);
            tipsText.Layer = 100;

            //Init continiue text
            continueText = new TextBox("Press ENTER to continue", GetFont("Impact.large"), 400, 320, Color.White, 1f);
            continueText.StaticPosition = true;
            continueText.Layer = 1002;
            continueText.TextBackground = Draw.ColoredRectangle(screen.GraphicsDevice, 600, 80, Color.Red);
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
                l.TextBackground = Draw.ColoredRectangle(screen.GraphicsDevice, 210, 50, GamePadControllers[i-1].PlayerModel.Color);
                l.BackgroundOffset = new Vector2(-10, -5);
                playerSelect.Add(l);
            }

            //Loads characters
            LoadCharacters(content);
            //Loads cursor
            LoadCursors(content);
            //Loads Maps
            LoadMaps(content);
            
            //Adds collision box at help and option button
            helpBox = CreateBtn(90, 40, 180, 80);
            
            //Adds collision box at help and option button
            optionsBox = CreateBtn(Constants.WindowWidth - 100, 40, 180, 80);

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
                //Create the boundingbox for PlayerCursor collision detectuib
                sprite.BoundRect(World, Constants.ThumbWith-10, Constants.ThumbHeight-10, BodyType.Static);
                //Collision detection functions
                sprite.BoundBox.OnCollision += OnCharacterSelCollision;
                sprite.BoundBox.OnSeparation += OnCharacterSelSeparation;
                sprite.BoundBox.IsSensor = true;
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

        private void LoadCursors(ContentManager content)
        {
            this.playerCursors = new List<Sprite>();
            foreach (GamepadController pad in GamePadControllers)
            {
                Sprite cursor = new Sprite(content, "Cursors/Player" + pad.PlayerIndex, 70, 70, 280 * pad.PlayerIndex + 100, 680);
                cursor.BoundRect(World, 1, 1, BodyType.Dynamic);
                cursor.Category = Category.All;
                cursor.Layer = 1001;
                cursor.Mass = 1;
                cursor.UserData = pad.PlayerIndex;
                playerCursors.Add(cursor);

                pad.OnStartPress += OnStartPress;
                pad.OnBackPress += OnBackPress;
            }

        }

        private void LoadMaps(ContentManager content)
        {
            //Loads all the map models from json textfiles
            mapModels = Serializing.LoadMaps();
        }
        
        public override void Unload()
        {
            startScreen.Dispose();
            characterScreen.Dispose();

            foreach (var character in characterThumbs)
            {
                character.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch (CurrentState)
            {
                case GameState.SelectionMenu:

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
            RemoveView(characterScreen);
        }

        public override void OnNext(GameStateManager value)
        {
            switch (value.PreviousState)
            {
                case GameState.StartScreen:
                    //menuState = MenuState.StartCharacterTransition;
                    RemoveView(startScreen);
                    RemoveView(tipsText);
                    break;
                case GameState.SelectionMenu:
                    RemoveView(characterScreen);
                    RemoveViews(characterThumbs.ToArray());
                    RemoveViews(playerCursors.ToArray());
                    RemoveViews(characterImages.ToArray());
                    RemoveViews(playerSelect.ToArray());
                    RemoveViews(continueText);

                    foreach (var sprite in characterThumbs)
                    {
                        sprite.BoundBox.OnCollision -= OnCharacterSelCollision;
                        sprite.BoundBox.OnSeparation -= OnCharacterSelSeparation;
                    }

                    break;
            }

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    AddView(startScreen);
                    AddView(tipsText);

                    break;
                case GameState.SelectionMenu :
                    AddView(characterScreen);
                    
                    tipsText.Text = "Press ENTER to continiue";
                    tipsText.Scale = 2f;

                    foreach (var pad in GamePadControllers)
                    {
                        pad.OnNavigation += OnCursorNavigate;
                        pad.OnHitKeyPressed += OnSelectPress;
                        pad.OnSuperKeyPressed += OnDeSelectPress;
                    }

                    //characterScreen.Position(1280, 0);
                    AddViews(characterThumbs.ToArray());
                    AddViews(playerCursors.ToArray());

                    break;

                case GameState.GamePlay:
                    AddController(new GameController(screen, mapModels[0]));
                    break;
            }
        }

        private bool OnCharacterSelCollision(Fixture character, Fixture cursor, Contact contact)
        {
            Character c = (Character)character.Body.UserData;
            int playerIndex = (int)cursor.Body.UserData;

            //Sets the hovered character model to the gamepad, so it can look for selection key press
            GamePadControllers[playerIndex].HoverCharacter = c;

            //Get the characterModels index -> same index as characterThumb and characterImages
            int index = characterModels.IndexOf(c);
            
            characterThumbs[index].Scale = 1.05f;
            characterThumbs[index].Rotation = 0.05f;

            ImageTexture img = characterImages[index];
            if (!img.IsActive)
            {
                AddView(img);
            }

            img.AddPosition(playerIndex * 260, 450, playerIndex);

            return true;
        }

        private void OnCharacterSelSeparation(Fixture character, Fixture cursor)
        {
            Character c = (Character)character.Body.UserData;
            int playerIndex = (int)cursor.Body.UserData;

            //Check if the selected character model still is the same, somtimes 
            //the next collision reacts before separation function with old is run
            //Set to null if same model
            if (GamePadControllers[playerIndex].HoverCharacter == c)
            {
                GamePadControllers[playerIndex].HoverCharacter = null;
            }

            int index = characterModels.IndexOf(c);
            ImageTexture img = characterImages[index];
            img.RemoveId(playerIndex);

            if (img.PosCount == 0)
            {
                characterThumbs[index].Scale = 1f;
                characterThumbs[index].Rotation = 0f;
                RemoveView(img);
            }
        }

        private void OnCursorNavigate(float directionX, float directionY, int playerIndex, bool newDirection)
        {
            var cursor = playerCursors[playerIndex];

            cursor.PositionX = MathHelper.Clamp(cursor.PositionX + directionX * Constants.MaxCursorSpeed,
                10, Constants.WindowWidth - 25);
            cursor.PositionY = MathHelper.Clamp(cursor.PositionY + directionY * Constants.MaxCursorSpeed,
                10, Constants.WindowHeight - 30);

        }

        private void OnSelectPress(int playerIndex)
        {
            var pad = GamePadControllers[playerIndex];
            if (pad.HoverCharacter != null)
            {
                pad.OnNavigation -= OnCursorNavigate;
                RemoveView(playerCursors[playerIndex]);
                pad.SelectedCharacter = pad.HoverCharacter;
            }

            if ((bool)helpBox.UserData || (bool)optionsBox.UserData)
            {
                popupMenu.State = PopupState.show;
            }
            else
            {
                popupMenu.State = PopupState.colapsed;
            }



        }

        private void OnDeSelectPress(int playerIndex)
        {
            var pad = GamePadControllers[playerIndex];
            if (pad.SelectedCharacter != null)
            {
                pad.OnNavigation += OnCursorNavigate;
                AddView(playerCursors[playerIndex]);
                pad.SelectedCharacter = null;
            }
        }

        private void OnStartPress(int playerIndex)
        {
            if (continueText.IsActive)
            {
                CurrentState = GameState.GamePlay;
            }
        }

        private void OnBackPress(int playerIndex)
        {

        }

        private bool onBtnCol(Fixture btn, Fixture cursor, Contact contact)
        {
            btn.Body.UserData = true;
            return true;
        }

        private void onBtnSep(Fixture btn, Fixture cursor)
        {
            btn.Body.UserData = false;
        }

        private Body CreateBtn(int x, int y, int width, int height){
            Body btn = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1f);
            btn.Position = ConvertUnits.ToSimUnits(x, y);
            btn.IsSensor = true;
            btn.UserData = false;
            btn.OnCollision += onBtnCol;
            btn.OnSeparation += onBtnSep;

            return btn;
        }
    }
}
