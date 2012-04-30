using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using SmashBros.MySystem;
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
        // Get the current gamepad state.
        private GamePadState oldGamePadState;
        private GamePadState currentGamePadState;
        private KeyboardState currentKeyboardState;
        private KeyboardState oldKeyboardState;

        private Queue<Tuple<TimeSpan, Vector2>> oldNavigations;
        private bool newDirection = false;

        public Player PlayerModel { get; set; }
        public int PlayerIndex { get { return PlayerModel.PlayerIndex; } }

        #region Keyboard Functions

        /// <summary>
        /// Checks the old keyboard state agianst the current keyboard state to 
        /// determin if a key was pressed
        /// </summary>
        /// <param name="key">Which key was pressed</param>
        /// <returns></returns>
        protected bool IsKeyPressed(Keys key)
        {
            return this.oldKeyboardState.IsKeyDown(key) && this.currentKeyboardState.IsKeyUp(key);
        }

        protected bool IsKeyPressedReversed(Keys key)
        {
            return this.oldKeyboardState.IsKeyUp(key) && this.currentKeyboardState.IsKeyDown(key);
        }

        protected bool IsKeyDown(Keys key)
        {
            return this.currentKeyboardState.IsKeyDown(key);
        }

        protected bool IsKeyUp(Keys key)
        {
            return this.currentKeyboardState.IsKeyUp(key);
        }

        #endregion

        public GamepadController(ScreenManager screen, Player playerModel) : base(screen)
        {
            this.PlayerModel = playerModel;
            this.oldNavigations = new Queue<Tuple<TimeSpan, Vector2>>(8);
        }

        public override void Load(ContentManager content)
        {
            this.SubscribeToGameState = true;
        }


        public override void Unload()
        {
        }

        private void UpdateTimer(Keys key, Buttons button, float directionX, float directionY, float elapsedTime, ButtonDown downAction, ButtonUp upAction, ButtonPressed pressAction, ref float downTimer, ref float upTimer)
        {
            if (IsKeyDown(key) || currentGamePadState.IsButtonDown(button))
            {
                if(downAction != null && downTimer == 0)
                    downAction.Invoke(directionX, directionY, upTimer, PlayerIndex);

                downTimer += elapsedTime;
                upTimer = 0;
            }
            else if (IsKeyUp(key) || currentGamePadState.IsButtonUp(button))
            {
                //Check if it was a press of key
                if(this.oldKeyboardState.IsKeyDown(key) || oldGamePadState.IsButtonDown(button)){
                    if (pressAction != null)
                        pressAction.Invoke(PlayerIndex);

                    if (upAction != null)
                        upAction.Invoke(downTimer, PlayerIndex);
                }
                upTimer += elapsedTime;
                downTimer = 0;
            }

        }

        private void updateNavigation()
        {
           
        }
        public override void Update(GameTime gameTime)
        {
            oldGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState((Microsoft.Xna.Framework.PlayerIndex)PlayerIndex);

            //Save keyboard state so all controllers can access them
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            //Update the position of the cursor
            float directionX = 0, directionY = 0;

            if (IsKeyDown(PlayerModel.KeyboardLeft)) directionX = -1;
            else if (IsKeyDown(PlayerModel.KeyboardRight)) directionX = 1;

            if (IsKeyDown(PlayerModel.KeyboardUp)) directionY = -1;
            else if (IsKeyDown(PlayerModel.KeyboardDown)) directionY = 1;

            Vector2 stick = currentGamePadState.ThumbSticks.Left;
            if (stick.X != 0) directionX = stick.X;
            if (stick.Y != 0) directionY = stick.Y * -1;

            if (oldNavigations.Count == 0)
            {
                oldNavigations.Enqueue(new Tuple<TimeSpan, Vector2> (gameTime.TotalGameTime, new Vector2(directionX, directionY)));
                newDirection = true;
            }
            else
            {
                TimeSpan lastNavigation = new TimeSpan(oldNavigations.Last().Item1.Ticks);
                if (lastNavigation.Add(new TimeSpan(0,0,0,0,50)).CompareTo(gameTime.TotalGameTime) <= 0)
                {
                    Tuple<TimeSpan, Vector2> oldNav = oldNavigations.Peek();
                    if (new TimeSpan(oldNav.Item1.Ticks).Add(new TimeSpan(0,0,0,0,200)).CompareTo(gameTime.TotalGameTime) <= 0) oldNavigations.Dequeue();
                    newDirection = Math.Abs(directionX - oldNav.Item2.X) > 0.8 || Math.Abs(directionY - oldNav.Item2.Y) > 0.8;
                    oldNavigations.Enqueue(new Tuple<TimeSpan, Vector2>(gameTime.TotalGameTime, new Vector2(directionX, directionY)));
                }
            }

            if (OnNavigation != null)
                OnNavigation.Invoke(directionX, directionY, PlayerIndex, newDirection);


            switch (CurrentState)
            {
                case GameState.StartScreen :
                    if(IsKeyDown(Keys.H) || currentGamePadState.IsButtonDown(Buttons.Back))
                    {
                        Screen.popupMenuController.State = PopupState.Options;
                    }
                    else if (this.currentKeyboardState.GetPressedKeys().Count() != 0 || currentGamePadState.IsButtonDown(Buttons.A) || currentGamePadState.IsButtonDown(Buttons.Start))
                    {
                        CurrentState = GameState.CharacterMenu;
                    }
                    break;
            }

            //Updates all the timers added for the keys
            float elapsed = gameTime.ElapsedGameTime.Milliseconds;

            UpdateTimer(PlayerModel.KeyboardHit, Buttons.A, directionX, directionY, elapsed, OnHitkeyDown, OnHitKeyUp, OnHitKeyPressed, ref HitDownTimer, ref HitUpTimer);

            UpdateTimer(PlayerModel.KeyboardSheild, Buttons.RightShoulder, directionX, directionY, elapsed, OnShieldkeyDown, OnShieldKeyUp, OnShieldKeyPressed, ref ShieldDownTimer, ref ShieldUpTimer);

            UpdateTimer(PlayerModel.KeyboardSuper, Buttons.X, directionX, directionY, elapsed, OnSuperkeyDown, OnSuperKeyUp, OnSuperKeyPressed, ref SuperDownTimer, ref SuperUpTimer);


            if ((IsKeyPressed(PlayerModel.KeyboardStart)|| isControllerPressed(Buttons.Start) )&& OnStartPress != null)
            {
                OnStartPress.Invoke(PlayerIndex);
            }

            if ((IsKeyPressed(PlayerModel.KeyboardBack) || isControllerPressed(Buttons.Back)) && OnBackPress != null)
            {
                OnBackPress.Invoke(PlayerIndex);
            }
        }

        private bool isControllerPressed(Buttons button)
        {
            return currentGamePadState.IsButtonUp(button) && oldGamePadState.IsButtonDown(button);
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
