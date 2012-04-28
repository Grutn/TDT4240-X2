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
        /// The stats of the chosen character. STATIC attributes!!
        /// </summary>
        public CharacterStats stats;

        /// <summary>
        /// Attacking CharacterState. See above!
        /// </summary>
        public CharacterState state { get; private set; }
        
        public void setState(CharacterState newState, MoveStats move = null)
        {
            if (newState == CharacterState.attacking && move == null) throw new NotImplementedException();
            if (newState != state)
            {
                if (newState != CharacterState.running) view.fps = Constants.FPS;
                switch (newState)
                {
                    case CharacterState.none:
                        attackMode = false;
                        inAir = false;
                        if (state == CharacterState.running || state == CharacterState.attacking) view.AddAnimation(0, 2, true);
                        else if (state == CharacterState.falling || state == CharacterState.jumping)
                        {
                            view.StartAnimation(stats.ani_landStart, stats.ani_landEnd, false);
                            view.AddAnimation(stats.ani_noneStart, stats.ani_noneEnd, true);
                        }
                        else view.StartAnimation(stats.ani_noneStart, stats.ani_noneEnd, true);
                        break;
                    case CharacterState.running:
                        attackMode = false;
                        if (state == CharacterState.falling || state == CharacterState.jumping)
                        {
                            view.StartAnimation(stats.ani_landStart, stats.ani_landEnd, false);
                            view.AddAnimation(stats.ani_runStart, stats.ani_runEnd, true);
                        }
                        else view.StartAnimation(stats.ani_runStart, stats.ani_runEnd, true);
                        break;
                    case CharacterState.braking:
                        attackMode = false;
                        if (state == CharacterState.falling || state == CharacterState.jumping)
                        {
                            view.StartAnimation(stats.ani_landStart, stats.ani_landEnd, false);
                            view.AddAnimation(stats.ani_brake, stats.ani_brake, true);
                        }
                        else view.StartAnimation(stats.ani_brake, stats.ani_brake, true);
                        break;
                    case CharacterState.jumping:
                        attackMode = false;
                        inAir = true;
                        view.StartAnimation(stats.ani_jumpStart, stats.ani_jumpEnd, false);
                        view.AddAnimation(stats.ani_fallStart, stats.ani_fallEnd, true);
                        break;
                    case CharacterState.falling:
                        attackMode = false;
                        inAir = true;
                        view.StartAnimation(stats.ani_fallStart, stats.ani_fallEnd, true);
                        break;
                    case CharacterState.takingHit:
                        attackMode = true;
                        view.StartAnimation(stats.ani_takeHitStart, stats.ani_takeHitEnd, true);
                        break;
                    case CharacterState.attacking:
                        attackMode = true;
                        view.StartAnimation(move.AniFrom, move.AniTo, false);
                        if (move.Type == MoveType.Body) view.Velocity = ((BodyMove)move).BodySpeed;
                        break;
                    case CharacterState.shielding:
                        attackMode = true;
                        //invounerable = true;
                        break;
                    case CharacterState.chargingHit:
                        attackMode = true;
                        break;
                    case CharacterState.chargingSuper:
                        attackMode = true;
                        break;
                }
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
