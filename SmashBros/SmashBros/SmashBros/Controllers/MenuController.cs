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

namespace SmashBros.Controllers
{
    public enum MenuState
    {
        None, StartCharacterTransition
    }

    public class MenuController : Controller
    {
        MenuState menuState;
        ImageTexture startScreen;
        ImageTexture characterScreen;
        List<Map> mapModels;
        List<Character> characterModels;
        List<Sprite> characterThumbs;
        List<ImageTexture> characterImages { get; set; }

        public MenuController(ScreenController screen) : base(screen)
        {

        }

        public MenuController(ScreenController screen, MenuState state) : base(screen)
        {
        }

        public void Init()
        {
            AddController(this);
        }

        public override void Load(ContentManager content)
        {
            startScreen = new ImageTexture(content, "StartScreen", 0, 0);
            characterScreen = new ImageTexture(content, "SelectionScreen", 0, 0);

            LoadCharacters(content);

            mapModels = Serializing.LoadMaps();

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
                sprite.BoundRect(World, Constants.ThumbWith, Constants.ThumbHeight, BodyType.Static);
                //Collision detection functions
                sprite.BoundBox.OnCollision += OnCharacterSelCollision;
                sprite.BoundBox.OnSeparation += OnCharacterSelSeparation;
                sprite.BoundBox.IsSensor = true;
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
            switch (menuState)
            {
                case MenuState.StartCharacterTransition:
                    
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
            Debug.WriteLine("Current state chagend");
            switch (value.PreviousState)
            {
                case GameState.StartScreen:
                    //menuState = MenuState.StartCharacterTransition;
                    RemoveView(startScreen);
                    break;
                case GameState.SelectionMenu:
                    RemoveView(characterScreen);
                    foreach (Sprite character in characterThumbs)
                    {
                        RemoveView(character);
                    }
                    break;
            }

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    AddView(startScreen);
                    break;
                case GameState.SelectionMenu :
                    AddView(characterScreen);
                    //characterScreen.Position(1280, 0);
                    foreach(Sprite character in characterThumbs){
                        AddView(character);
                    }
                    break;
            }
        }

        private bool OnCharacterSelCollision(Fixture character, Fixture cursor, Contact contact)
        {
            Character c = (Character)character.Body.UserData;
            int playerIndex = (int)cursor.Body.UserData;

            //Get the characterModels index -> same index as characterThumb and characterImages
            int index = characterModels.IndexOf(c);
            
            characterThumbs[index].Scale = 1.05f;
            characterThumbs[index].Rotation = 0.05f;
            Debug.WriteLine("Kake");

            ImageTexture img = characterImages[index];
            if (!img.IsActive)
            {
                AddView(img);
            }

            img.AddDrawPosition(playerIndex, playerIndex * 250, 400);

            return true;
        }

        private void OnCharacterSelSeparation(Fixture character, Fixture cursor)
        {
            Character c = (Character)character.Body.UserData;
            int playerIndex = (int)cursor.Body.UserData;
            int index = characterModels.IndexOf(c);
            ImageTexture img = characterImages[index];
            img.RemoveDrawPosition(playerIndex);

            if (img.Positions.Count() == 0)
            {
                characterThumbs[index].Scale = 1f;
                characterThumbs[index].Rotation = 0f;
                RemoveView(img);
            }
        }

    }
}
