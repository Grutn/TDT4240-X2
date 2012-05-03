using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using SmashBros.MySystem;
using FarseerPhysics.DebugViews;
using FarseerPhysics;
using System.Threading;
using SmashBros.Controllers;
using SmashBros.Models;
using System.IO;

namespace SmashBros.MySystem
{

    /// <summary>
    /// Inits the controllers needed to start the game
    /// And holds the controllerviewmanager and updates it
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        
        public Dictionary<string,SpriteFont> fonts;
        public ControllerViewManager ControllerViewManager;
        public GameStateManager gameStateManager;

        //Controllers
        public SoundController soundController;
        public List<GamepadController> gamePads;
        public CursorController cursorsController;
        public OverlayMenuController popupMenuController;
        MenuController menu;

        public GameOptions GameOptions;
        
        public ScreenManager(Game game)
            : base(game)
        {
            this.gamePads = new List<GamepadController>();
            this.fonts = new Dictionary<string, SpriteFont>();

            this.gameStateManager = new GameStateManager();
            this.gameStateManager.CurrentState = GameState.StartScreen;
            this.soundController = new SoundController(this);
        }

        public void Exit()
        {
            ControllerViewManager.Dispose();
            Exit();
        }

        Texture2D Image;
        protected override void LoadContent()
        {


            ContentManager content = Game.Content;

            //Loads the gameoptions from last time
            GameOptions = Serializing.LoadGameOptions();

            ControllerViewManager = new ControllerViewManager(Game.GraphicsDevice, content);
            //Adds the sound controller
            ControllerViewManager.AddController(soundController);


            //Loads and add the fonts to the a list so controllers easily can reach this just by the name of the string
            fonts.Add("Impact", content.Load<SpriteFont>("Fonts/Impact"));
            fonts.Add("Impact.large", content.Load<SpriteFont>("Fonts/Impact.large"));

            //Loads the player controllers from file
            List<Player> players = Serializing.LoadPlayerControllers();
            // Init each player by creating a gamepadcontroller foreach player
            foreach (Player player in players)
            {
                GamepadController gamepad = new GamepadController(this, player);
                gamePads.Add(gamepad);
                ControllerViewManager.AddController(gamepad);
            }

            //Creates the controller for the cursor
            cursorsController = new CursorController(this);
            ControllerViewManager.AddController(cursorsController);

            //Adds the popupmenu to the controllers stack
            popupMenuController = new OverlayMenuController(this);
            ControllerViewManager.AddController(popupMenuController);


            //if startgameplay is true then the game goes straight in to game play
            if (Constants.StartGameplay)
            {
                //Set the right state
                gameStateManager.CurrentState = GameState.GamePlay;
                var chars = Serializing.LoadCharacters();
                var maps = Serializing.LoadMaps();
                gamePads[0].PlayerModel.SelectedCharacter = chars[1];
                gamePads[0].PlayerModel.CharacterIndex = 0;
                gamePads[1].PlayerModel.SelectedCharacter = chars[2];
                gamePads[1].PlayerModel.CharacterIndex = 2;

                GamePlayController game = new GamePlayController(this, maps[1]);
                ControllerViewManager.AddController(game);
            }
            else
            {
                gameStateManager.CurrentState = GameState.StartScreen;
                this.menu = new MenuController(this);
                ControllerViewManager.AddController(menu);
            }
        }


        float elapsedTime = 0;
        public override void Update(GameTime gameTime)
        {

            //If in debugmode reload the serialized files,
            //then it's possible to do runtime updates to map and characters
            if (Constants.DebugMode)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                //Updates every 1,5 sec
                if (elapsedTime >= 1500)
                {
                    Serializing.Reload();
                    elapsedTime = 0;
                }
            }

            ControllerViewManager.Update(gameTime);

           
        }

        public override void Draw(GameTime gameTime)
        {
            ControllerViewManager.Draw(gameTime);
        }
    }
}
