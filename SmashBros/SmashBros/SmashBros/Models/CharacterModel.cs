using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SmashBros.System;
using SmashBros.Controllers;

namespace SmashBros.Models
{
    public enum CharacterState { none, running, braking, jumping, falling, takingHit, attacking, shielding, chargingHit, chargingSuper }
    
    public class CharacterModel
    {
        /// <summary>
        /// Wich player this is 
        /// range 0-3
        /// </summary>
        public int playerIndex;

        /// <summary>
        /// Currently chosen character of this player.
        /// </summary>
        public CharacterView view;

        /// <summary>
        /// Attacking CharacterState. See above!
        /// </summary>
        public CharacterState state { get; private set; }
        
        public void setState(CharacterState newState, MoveStats move = null)
        {
            if (newState == CharacterState.attacking || newState == CharacterState.chargingHit || newState == CharacterState.chargingSuper || newState == CharacterState.shielding)
            {
                if(newState != CharacterState.shielding && move == null) throw new NotImplementedException();
                attackMode = true;
            }
            else attackMode = false;
            
            if(newState == CharacterState.jumping || newState == CharacterState.falling) inAir = true;

            if (newState != state)
            {
                view.StateChanged(state, newState, move);
                state = newState;
            }
        }

        /// <summary>
        /// The character is facing right direction?
        /// </summary>
        public bool _faceRight;
        public bool faceRight
        {
            get { return _faceRight; }
            set
            {
                _faceRight = value;
                if (value) view.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                else view.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            }
        }

        /// <summary>
        /// The character is in air or on ground?
        /// </summary>
        public bool inAir;

        /// <summary>
        /// Whether the character is standing on a box which can be gone through.
        /// </summary>
        public bool onSoftBox;

        /// <summary>
        /// Whether the character is in an attackCharacterState.
        /// </summary>
        public bool attackMode;

        /// <summary>
        /// Whether or not the player is in an invounerable CharacterState.
        /// </summary>
        private bool _invounerable;
        public bool invounerable
        {
            get { return _invounerable; }
            set
            {
                _invounerable = value;
                view.Blinking = value;
            }
        }

        /// <summary>
        /// Time left of invounerability.
        /// </summary>
        public float invounerableTimeLeft = 0;

        /// <summary>
        /// Time left to when the character reappears on the map.
        /// </summary>
        public float resetTimeLeft = -1;

        /// <summary>
        /// Number of jumps left. One of them is supermove, and once super is used, jumpsleft = 0.
        /// </summary>
        public int jumpsLeft = 3;

        /// <summary>
        /// An int that describes how much damage the player has taken in the current game/characterlife.
        /// </summary>
        public int damagePoints = 0;

        /// <summary>
        /// The current position of character sprite.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// A powerUp the player currently is in posetion of.
        /// </summary>
        public PowerUp powerUp;

        /// <summary>
        /// Time left before current powerup expires.
        /// </summary>
        public int powerUpTimeLeft;

        /// <summary>
        /// Currently posessed weapon.
        /// </summary>
        public Weapon weapon;

        public CharacterModel(GamepadController pad, Vector2 startPos)
        {
            position = startPos;
            playerIndex = pad.PlayerIndex;
        }
    }
}
