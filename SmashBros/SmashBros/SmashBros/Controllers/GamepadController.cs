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
        
        MenuController menu;
        Sprite cursor;

        private float HitUpTimer = 0, HitDownTimer=0, 
            ShieldUpTimer = 0,ShieldDownTimer=0, SuperUpTimer = 0, SuperDownTimer=0;

        //public int playerIndex;
        public Player PlayerModel { get; set; }
        public int PlayerIndex { get; set; }
        private bool disableCursor = false;
        public bool DisableCursor { get { return disableCursor; } 
            set
            {
                if (value && cursor.IsActive)
                {
                    RemoveView(cursor);
                }
                else if (!value && !cursor.IsActive)
                {
                    AddView(cursor);
                }

                disableCursor = value;
            } 
        }

        /// <summary>
        /// The slected character can only be set by the gamepad itself, it uses the HoverCharacter and checks if Selection key is pressed
        /// </summary>
        public Character SelectedCharacter { get; set; }
        /// <summary>
        /// Assigned when the players cursor hovers a player
        /// </summary>
        public Character HoverCharacter { get; set; }

        public GamepadController(ScreenController screen, Player playerModel, MenuController menu) : base(screen)
        {
           // this.playerIndex = playerIndex;
            this.menu = menu;
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

        private void UpdateTimer(Keys key, float elapsedTime, ButtonDown downAction, ButtonUp upAction, ButtonPressed pressAction, ref float downTimer,ref float upTimer)
        {
            if (IsKeyDown(key))
            {
                if(downAction != null)
                    downAction.Invoke(downTimer, PlayerIndex);

                downTimer += elapsedTime;
                upTimer = 0;
            }
            else if (IsKeyUp(key))
            {
                //Check if it was a press of key
                if (pressAction != null && screen.oldKeyboardState.IsKeyDown(key))
                    pressAction.Invoke(PlayerIndex);

                if(upAction != null)
                    upAction.Invoke(upTimer, PlayerIndex);
                upTimer += elapsedTime;
                downTimer = 0;
            }

        }
        
        public override void Update(GameTime gameTime)
        {
            //Updates all the timers added for the keys
            float elapsed = gameTime.ElapsedGameTime.Milliseconds;

            UpdateTimer(PlayerModel.KeyboardHit, elapsed, OnHitkeyDown, OnHitKeyUp, OnHitKeyPressed, ref HitDownTimer, ref HitUpTimer);
            
            UpdateTimer(PlayerModel.KeyboardSheild, elapsed, OnShieldkeyDown, OnShieldKeyUp, OnShieldKeyPressed, ref ShieldDownTimer, ref ShieldUpTimer);

            UpdateTimer(PlayerModel.KeyboardSuper, elapsed, OnSuperkeyDown, OnSuperKeyUp, OnSuperKeyPressed, ref SuperDownTimer, ref SuperUpTimer);


            if (IsKeyPressed(PlayerModel.KeyboardStart) && OnStartPress != null)
            {
                OnStartPress.Invoke(PlayerIndex);
            }

            if (IsKeyPressed(PlayerModel.KeyboardBack ) && OnBackPress != null)
            {
                OnBackPress.Invoke(PlayerIndex);
            }


            //Update the position of the cursor
            float directionX = 0, directionY = 0;
            if (IsKeyDown(PlayerModel.KeyboardLeft)) 
                directionX = -1;
            else if (IsKeyDown(PlayerModel.KeyboardRight)) 
                directionX = 1;

            if (IsKeyDown(PlayerModel.KeyboardUp)) 
                directionY = -1;
            else if (IsKeyDown(PlayerModel.KeyboardDown)) 
                directionY = 1;

            if ((directionX != 0 || directionY != 0) && OnNavigation != null)
                OnNavigation.Invoke(directionX, directionY, PlayerIndex);

            switch (CurrentState)
            {
                case GameState.StartScreen :
                    if(IsKeyDown(Keys.H))
                    {
                        throw new NotImplementedException("Show help menu to user, same help menu as in character and mapselection menu");
                    }
                    else if (screen.currentKeyboardState.GetPressedKeys().Count() != 0)
                    {
                        CurrentState = GameState.SelectionMenu;
                    }
                    break;
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
        
        public delegate void ButtonDown(float timeUp, int playerIndex);
        public delegate void ButtonUp(float timeDown, int playerIndex);
        public delegate void ButtonPressed(int playerIndex);
        public delegate void NavigationKey(float xDirection, float yDirection, int playerIndex);


        public event ButtonDown OnHitkeyDown;
        public event ButtonUp OnHitKeyUp;
        public event ButtonPressed OnHitKeyPressed;

        public event ButtonDown OnSuperkeyDown;
        public event ButtonUp OnSuperKeyUp;
        public event ButtonPressed OnSuperKeyPressed;

        public event ButtonDown OnShieldkeyDown;
        public event ButtonUp OnShieldKeyUp;
        public event ButtonPressed OnShieldKeyPressed;

        public event ButtonPressed OnStartPress;
        public event ButtonPressed OnBackPress;

        public event NavigationKey OnNavigation;
        
    }
}
