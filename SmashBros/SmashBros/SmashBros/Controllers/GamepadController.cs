using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using SmashBros.System;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using FarseerPhysics.Dynamics.Contacts;

namespace SmashBros.Controllers
{
    /// <summary>
    /// The gamepad controller uses the gamestate to determine which controllers to use
    /// </summary>
    public class GamepadController : Controller
    {
        public int playerIndex;
        CharacterController character;
        MenuController menu;
        Sprite cursor;

        public GamepadController(ScreenController screen, int playerIndex, MenuController menu) : base(screen)
        {
            this.playerIndex = playerIndex;
        }

        public override void Load(ContentManager content)
        {
            cursor = new Sprite(content, "Cursors/Player" + playerIndex, 70, 70, 280 * playerIndex + 100, 680);
            cursor.BoundRect(World, 1, 1, BodyType.Dynamic);
            cursor.Category = Category.All;
            cursor.Layer = 10;
            cursor.Mass = 1;
            cursor.UserData = playerIndex;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            switch (CurrentState)
            {
                case GameState.StartScreen :
                    if (screen.currentKeyboardState.GetPressedKeys().Count() != 0)
                    {
                        CurrentState = GameState.SelectionMenu;

                    }
                    break;
                case GameState.SelectionMenu:
                    if (!cursor.IsActive)
                    {
                        AddView(cursor);

                    }
                    int directionX = 0, directionY = 0;
                    if (playerIndex == 0)
                    {
                        if (IsKeyDown(Keys.A)) directionX = -1;
                        else if (IsKeyDown(Keys.D)) directionX = 1;

                        if (IsKeyDown(Keys.W)) directionY = -1;
                        else if (IsKeyDown(Keys.S)) directionY = 1;
                    }

                    if (playerIndex == 1)
                    {
                        if (IsKeyDown(Keys.Left)) directionX = -1;
                        else if (IsKeyDown(Keys.Right)) directionX = 1;

                        if (IsKeyDown(Keys.Up)) directionY = -1;
                        else if (IsKeyDown(Keys.Down)) directionY = 1;
                    }

                    MoveCursor(directionX, directionY);
                    break;
            }
        }

        public override void Deactivate()
        {
        }

        public override void OnNext(GameStateManager value)
        {
        }

        /// <summary>
        /// Moves cursor in direction
        /// </summary>
        /// <param name="directionX">direction in X of cursor -1=left 0=no dir, 1=right</param>
        /// <param name="directionY">direction in Y of cursor -1=up 0=no dir, 1=down</param>
        private void MoveCursor(int directionX, int directionY)
        {
            float forceX, forceY;
            if (CalculateCursorMove(directionX, cursor.VelocityX, out forceX))
                cursor.VelocityX = 0;

            if (CalculateCursorMove(directionY, cursor.VelocityY, out forceY))
                cursor.VelocityY = 0;

            cursor.ForceX = forceX;
            cursor.ForceY = forceY;
        }

        /// <summary>
        /// Calculates the force to apply on cursor
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="velocity"></param>
        /// <param name="force"></param>
        /// <returns>true if velocity needs to be set to 0</returns>
        private bool CalculateCursorMove(int direction, float velocity, out float force){
            
            if (direction != 0)
            {
                //If cursor has velocity in different direction than the direction tells it
                //We multiply the force with a bigger number to make it chante direction faster
                if ((velocity > 0 && direction < 0) ||
                    (velocity < 0 && direction > 0))
                {
                    force = 25f * direction;
                }
                //If velocity is is less then max speed apply force in direction
                else if (Math.Abs(velocity) < Constants.MaxCursorSpeed)
                {
                    force = 5f * direction;
                }
                else force = 0;
            }
            else
            {
                //If velocity is grater than 1 apply force in opposite direction of speed
                //Else force = 0 and return false to set speed to 0
                if (Math.Abs(velocity) >= 1)
                {
                    direction = velocity <= -1 ? 1 : -1;
                    force = 20 * direction;
                }
                else
                {
                    force = 0;
                    return true;
                }

            }

            return false;
        }
    }
}
