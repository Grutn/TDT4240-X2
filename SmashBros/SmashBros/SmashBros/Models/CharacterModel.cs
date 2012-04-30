using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SmashBros.MySystem;
using SmashBros.Controllers;
using System.Diagnostics;

namespace SmashBros.Models
{
    public enum CharacterState { none, running, braking, jumping, falling, takingHit, attacking, shielding, charging }
    
    public class CharacterModel
    {
        /// <summary>
        /// Wich player this is 
        /// range 0-3
        /// </summary>
        public int playerIndex;

        /// <summary>
        /// The stats of the chosen character. STATIC attributes!!
        /// </summary>
        public CharacterStats stats;

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
            if (newState == CharacterState.attacking || newState == CharacterState.charging || newState == CharacterState.shielding)
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
        /// Number of jumps left. One of them is supermove, and once super is used, jumpsleft = 0.
        /// </summary>
        public int jumpsLeft = 3;

        /// <summary>
        /// An int that describes how much damage the player has taken in the current game/characterlife.
        /// </summary>
        public int damagePoints = 0;

        /// <summary>
        /// Time left to when the character reappears on the map.
        /// </summary>
        public float resetTimeLeft;

        /// <summary>
        /// The current position of character sprite.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// A powerUp the player currently is in posetion of.
        /// </summary>
        public PowerUp powerUp;
        /*
        {
            get;
            set
            {
                if (value == null)
                {
                    maxSpeed = stats.maxSpeed;
                    acceleration = stats.acceleration;
                    weight = stats.weight;
                    jumpHeight = stats.jumpStartVelocity;
                }
            }
        }
        */
        /// <summary>
        /// Currently posessed weapon.
        /// </summary>
        public Weapon weapon;

        /// <summary>
        /// The CURRENT (powerup concidered) maximum magnitude of speed in x-direction this character can have.
        /// </summary>
        public int maxSpeed;

        /// <summary>
        /// CURRENT (powerup concidered) Acceleration!
        /// </summary>
        public int acceleration;

        /// <summary>
        /// The CURRENT (powerup concidered) weight of the character. Determines, along with players damagePoints, how far the character is pushed by some force.
        /// </summary>
        public int weight;

        /// <summary>
        /// The CURRENT (powerup concidered) velocity of the characters jump.
        /// </summary>
        public int JumpStartVelocity;

        public CharacterModel(GamepadController pad, Vector2 startPos, int countDown, CharacterStats stats)
        {
            position = startPos;
            playerIndex = pad.PlayerIndex;
            resetTimeLeft = countDown * 1000;
            maxSpeed = stats.maxSpeed;
            acceleration = stats.acceleration;
            weight = stats.weight;
            JumpStartVelocity = stats.jumpStartVelocity;
        }
    }
}
