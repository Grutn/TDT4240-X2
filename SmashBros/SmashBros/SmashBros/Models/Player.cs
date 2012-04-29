using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SmashBros.Model
{
    /// <summary>
    /// This class contains each players settings. 
    /// This is mainly the keyboard controlls
    /// </summary>
    public class Player
    {
        public Player()
        {

        }

        /// <summary>
        /// Wich player this is 
        /// range 0-3
        /// </summary>
        public int PlayerIndex { get; set; }
        /// <summary>
        /// Color of this player
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Tells wheter or not the keyboard is possible to use for this player
        /// </summary>
        public bool KeyboardEnabled { get; set; }

        /// <summary>
        /// Keys on keyboard to navigate in game and on menu
        /// </summary>
        public Keys KeyboardUp { get; set; }
        public Keys KeyboardDown { get; set; }
        public Keys KeyboardRight { get; set; }
        public Keys KeyboardLeft { get; set; }

        /// <summary>
        /// Which key on keyboard is used for regular attack
        /// Same key used for selection in menu
        /// </summary>
        public Keys KeyboardHit { get; set; }
        /// <summary>
        /// Key on keyboard used for special attack
        /// Same key used for deselect in menu
        /// </summary>
        public Keys KeyboardSuper { get; set; }
        /// <summary>
        /// Key used for shield on keyboard in gameplay
        /// </summary>
        public Keys KeyboardSheild { get; set; }

        /// <summary>
        /// Start button for keyboard
        /// </summary>
        public Keys KeyboardStart { get; set; }

        /// <summary>
        /// Back button for keyboard
        /// </summary>
        public Keys KeyboardBack { get; set; }
        /// <summary>
        /// The index of the selected character
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        /// Which character this person has selected
        /// </summary>
        public CharacterStats SelectedCharacter { get; set; }

    }
}
