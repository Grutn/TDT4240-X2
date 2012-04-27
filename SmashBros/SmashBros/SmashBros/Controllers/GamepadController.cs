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
using SmashBros.Model;

namespace SmashBros.Controllers
{
    /// <summary>
    /// The gamepad controller uses the gamestate to determine which controllers to use
    /// </summary>
    public class GamepadController : Controller
    {
        
        private float HitUpTimer = 0, HitDownTimer=0, 
            ShieldUpTimer = 0,ShieldDownTimer=0, SuperUpTimer = 0, SuperDownTimer=0;

        //public int playerIndex;
        public Player PlayerModel { get; set; }
        public int PlayerIndex { get; set; }
       

        /// <summary>
        /// The slected character can only be set by the gamepad itself, it uses the HoverCharacter and checks if Selection key is pressed
        /// </summary>
        public Character SelectedCharacter { get; set; }
        /// <summary>
        /// Assigned when the players cursor hovers a player
        /// </summary>
        public Character HoverCharacter { get; set; }

        public GamepadController(ScreenManager screen, Player playerModel) : base(screen)
        {
           // this.playerIndex = playerIndex;
            this.PlayerModel = playerModel;
            this.PlayerIndex = PlayerModel.PlayerIndex;
        }

        public override void Load(ContentManager content)
        {
            this.SubscribeToGameState = true;
        }


        public override void Unload()
        {
        }

        private void UpdateTimer(Keys key, float directionX, float directionY, float elapsedTime, ButtonDown downAction, ButtonUp upAction, ButtonPressed pressAction, ref float downTimer, ref float upTimer)
        {
            if (IsKeyDown(key))
            {
                if(downAction != null && downTimer == 0)
                    downAction.Invoke(directionX, directionY, upTimer, PlayerIndex);

                downTimer += elapsedTime;
                upTimer = 0;
            }
            else if (IsKeyUp(key))
            {
                //Check if it was a press of key
                if (pressAction != null && Screen.oldKeyboardState.IsKeyDown(key))
                    pressAction.Invoke(PlayerIndex);

                if(upAction != null && Screen.oldKeyboardState.IsKeyDown(key))
                    upAction.Invoke(downTimer, PlayerIndex);
                upTimer += elapsedTime;
                downTimer = 0;
            }

        }
        
        public override void Update(GameTime gameTime)
        {
            //Update the position of the cursor
            float directionX = 0, directionY = 0;
            bool newDirection = false;
            if (IsKeyDown(PlayerModel.KeyboardLeft))
            {
                directionX = -1;
                newDirection = IsKeyPressedReversed(PlayerModel.KeyboardLeft);
            }

            else if (IsKeyDown(PlayerModel.KeyboardRight))
            {
                directionX = 1;
                newDirection = IsKeyPressedReversed(PlayerModel.KeyboardRight);
            }

            if (IsKeyDown(PlayerModel.KeyboardUp))
            {
                directionY = -1;
                newDirection = IsKeyPressedReversed(PlayerModel.KeyboardUp);
            }
            else if (IsKeyDown(PlayerModel.KeyboardDown))
            {
                directionY = 1;
                newDirection = IsKeyPressedReversed(PlayerModel.KeyboardDown);
            }
            
            if (OnNavigation != null)
                OnNavigation.Invoke(directionX, directionY, PlayerIndex, newDirection);

            switch (CurrentState)
            {
                case GameState.StartScreen :
                    if(IsKeyDown(Keys.H))
                    {
                        throw new NotImplementedException("Show help menu to user, same help menu as in character and mapselection menu");
                    }
                    else if (Screen.currentKeyboardState.GetPressedKeys().Count() != 0)
                    {
                        CurrentState = GameState.CharacterMenu;
                    }
                    break;
            }

            //Updates all the timers added for the keys
            float elapsed = gameTime.ElapsedGameTime.Milliseconds;

            UpdateTimer(PlayerModel.KeyboardHit, directionX, directionY, elapsed, OnHitkeyDown, OnHitKeyUp, OnHitKeyPressed, ref HitDownTimer, ref HitUpTimer);

            UpdateTimer(PlayerModel.KeyboardSheild, directionX, directionY, elapsed, OnShieldkeyDown, OnShieldKeyUp, OnShieldKeyPressed, ref ShieldDownTimer, ref ShieldUpTimer);

            UpdateTimer(PlayerModel.KeyboardSuper, directionX, directionY, elapsed, OnSuperkeyDown, OnSuperKeyUp, OnSuperKeyPressed, ref SuperDownTimer, ref SuperUpTimer);


            if (IsKeyPressed(PlayerModel.KeyboardStart) && OnStartPress != null)
            {
                OnStartPress.Invoke(PlayerIndex);
            }

            if (IsKeyPressed(PlayerModel.KeyboardBack) && OnBackPress != null)
            {
                OnBackPress.Invoke(PlayerIndex);
            }
        }

        public override void Deactivate()
        {
        }

        public override void OnNext(GameStateManager value)
        {
           
        }

        private void UpdateTimers(GameTime gameTime)
        {

        }

        public delegate void ButtonDown(float xDirection, float yDirection, float timeUp, int playerIndex);
        public delegate void ButtonUp(float timeDown, int playerIndex);
        public delegate void ButtonPressed(int playerIndex);
        public delegate void NavigationKey(float xDirection, float yDirection, int playerIndex, bool newDirection);


        public ButtonDown OnHitkeyDown;
        public ButtonUp OnHitKeyUp;
        public ButtonPressed OnHitKeyPressed;

        public ButtonDown OnSuperkeyDown;
        public ButtonUp OnSuperKeyUp;
        public ButtonPressed OnSuperKeyPressed;

        public ButtonDown OnShieldkeyDown;
        public ButtonUp OnShieldKeyUp;
        public ButtonPressed OnShieldKeyPressed;

        public ButtonPressed OnStartPress;
        public ButtonPressed OnBackPress;

        public NavigationKey OnNavigation;
        
    }
}
