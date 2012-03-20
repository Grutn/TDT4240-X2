using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SmashBros
{
    public class ControllerState
    {
        #region Fields

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadState;

        public readonly bool[] GamePadWasConnected;

        #endregion

        #region Initialization

        public ControllerState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadState = new GamePadState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            for (int i = 0; i < MaxInputs; ++i)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadState[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }
        }

        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadState[i].IsButtonUp(button));
            }
            else
            {
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }

        public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return (IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                    IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex));
        }

        public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return (IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex));
        }

        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return (IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex));
        }

        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return (IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex));
        }

        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return (IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex));
        }

        #endregion
    }
}
