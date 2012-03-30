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

namespace SmashBros.Controllers
{
    public class ScreenController : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        MenuController menu;
        
        List<Controller> controllers;
        List<Controller> removeController;
        List<Controller> addController;

        public List<IView> views;
        public List<SpriteFont> fonts;
        public World world;
        public KeyboardState currentKeyboardState;
        public KeyboardState oldKeyboardState;
        Texture2D t;
        public ScreenController(Game game)
            : base(game)
        {
            
            this.views = new List<IView>();
            
            this.controllers = new List<Controller>();
            this.removeController = new List<Controller>();
            this.addController = new List<Controller>();

            this.fonts = new List<SpriteFont>();
            this.menu = new MenuController(this);
            this.world = new World(Vector2.Zero);
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            content.Load<SpriteFont>("font");

            t = content.Load<Texture2D>("StartScreen");
            //content.Load<SpriteFont>("bigFont");


            menu.IsActive = true;
        }

        public void ActivateController(Controller controller)
        {
            controller.Load(Game.Content);
            this.addController.Add(controller);
        }

        public void DeactivateController(Controller controller)
        {

            removeController.Add(controller);
            controller.Unload();
        }

        protected override void UnloadContent()
        {
            foreach (var c in controllers)
            {
                c.Unload();
            }

            this.controllers = new List<Controller>();
        }

        public override void Update(GameTime gameTime)
        {
            //Save keyboard state so all controllers can access them
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (removeController.Count() != 0)
            {
                //Loop through the removeController list to see if there are any controllers to remove
                foreach (var controller in removeController)
                {
                    controllers.Remove(controller);
                }
                //Finished removing controllers
                //Reset list
                removeController.RemoveAll(a=> true);
            }

            if (addController.Count() != 0)
            {
                //Loop through the addController list to see if there are any controllers to add
                foreach (var controller in addController)
                {
                    controllers.Add(controller);
                }
                //Finished adding controllers
                //Reset list
                addController.RemoveAll(a => true);
            }

            foreach (var controller in controllers)
            {
                controller.Update(gameTime);
            }
            

        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var view in views)
            {
                view.Draw(spriteBatch, gameTime);
            }
            spriteBatch.End();
        }
    }
}
