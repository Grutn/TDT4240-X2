using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using Microsoft.Xna.Framework;
using SmashBros.System;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;
using System.Threading;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace SmashBros.Controllers
{
    public enum MovementState { none, jumping, running, falling }

    public enum AttackState { none, attacking, shielding, chargingHit, chargingSuper }

    class CharacterController : Controller
    {
        /// <summary>
        /// Currently chosen character of this player.
        /// </summary>
        public Sprite character;

        /// <summary>
        /// The chosen character's stats and moves.
        /// </summary>
        public Character model;

        /// <summary>
        /// The state of which the player is currently in. (jumping, running, etc.)
        /// </summary>
        public MovementState movementState;

        /// <summary>
        /// Attacking state. See above!
        /// </summary>
        public AttackState attackState;
 
        /// <summary>
        /// The character is facing right direction?
        /// </summary>
        public bool faceRight;

        /// <summary>
        /// Number of jumps left. One of them is supermove, and once super is used, jumpsleft = 0.
        /// </summary>
        public int jumpsLeft = 3;

        /// <summary>
        /// An int that describes how much damage the player has taken in the current game/characterlife.
        /// </summary>
        public int damagePoints;

        /// <summary>
        /// Number of lifes left.
        /// </summary>
        public int lifes;

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

        public GamepadController pad;

        /// <summary>
        /// The move that has either begun, or that is in charging face.
        /// </summary>
        public Move move;

        /// <summary>
        /// The box that punches people on collision.
        /// </summary>
        public Body moveBox;

        public float chargeTime;

        public bool startWhenReady;

        public CharacterController(ScreenController screen, GamepadController pad) 
            : base(screen)
        {
            this.pad = pad;
        }

        public override void Load(ContentManager content)
        {
            character = new Sprite(content, "spiderman", 100, 100, 200, 200);
            character.BoundRect(World, 100, 100);
            AddView(character);

            character.BoundBox.CollisionCategories = Category.Cat10;
            character.BoundBox.CollidesWith = Category.All & ~Category.Cat10;

            pad.OnNavigation += OnNavigation;
            pad.OnHitkeyDown += OnHitKeyDown;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            switch (movementState)
            {
                case MovementState.jumping:
                    //Only collid with solide ground while jumping uppwards
                    if (position.Y < character.Position.Y)
                    {
                        movementState = MovementState.falling;
                    }
                    else if (position.Y == character.Position.Y)
                    {
                        //movementState = MovementState.none;
                    }
                    break;
                case MovementState.running:
                    if (Math.Abs(character.VelocityX) < 10)
                        movementState = MovementState.none;
                    break;
                case MovementState.falling:
                    if (position.Y == character.Position.Y)
                    {
                        movementState = MovementState.none;
                        jumpsLeft = 3;
                    }
                    break;
            }

            switch (attackState)
            {
                case AttackState.chargingHit:
                    chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                    break;
                case AttackState.chargingSuper:
                    chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                    break;
            }

            if (startWhenReady && chargeTime > move.minWait)

            position = character.Position;

            if (Constants.DebugMode)
            {
                screen.controllerViewManager.debugView.PlayerStates[pad.PlayerIndex] =
                    movementState.ToString() + " & " + attackState.ToString();
            }
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        private void OnNavigation(float directionX, float directionY, int playerIndex)
        {
            if (directionY < 0)
            {
                if (jumpsLeft > 1 && character.VelocityY > -10)
                {
                    movementState = MovementState.jumping;
                    character.Impulse = new Vector2(0, -15);
                    jumpsLeft--;
                }
            }
            
            if (movementState == MovementState.jumping)
            {
                if(Math.Abs(character.VelocityX) < 50)//model.maxSpeed)
                    character.ForceX = 20 * directionX;
            }
            else
            {
                if (Math.Abs(character.VelocityX) < 50)//model.maxSpeed)
                    character.ForceX = 50 * directionX;
                else if (Math.Abs(character.VelocityX) > 10)
                    movementState = MovementState.running;
            }
        }

        private void OnHitKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {

            if (attackState == AttackState.none)
            {
                if (movementState == MovementState.none && (directionX > 0.9 || directionY > 0.9))
                {
                    attackState = AttackState.chargingHit;
                    startWhenReady = false;
                    if (Math.Abs(directionX) > Math.Abs(directionY))
                        move = model.aLR;
                    else if (directionY > 0) move = model.aDown;
                    else move = model.aUp;
                }
                else
                {
                    attackState = AttackState.attacking;
                    move = model.a;
                    //moveDirection = directionX == 0 && directionY == 0? (faceRight? 'R' : 'L') : (directionX > directionY? (directionX > 0? 'R' : 'L') : (directionY > 0? 'D' : 'U'));
                    BeginMove();
                } 
            }
        }

        private void OnHitKeyUp(float downTimer, int playerIndex)
        {
            if (attackState == AttackState.chargingHit)
            {
                if (chargeTime > move.minWait)
                {
                    BeginMove();
                }
                else startWhenReady = true;
            }
        }

        private void BeginMove()
        {
            moveBox = BodyFactory.CreateRectangle(World, move.sqWidth, move.sqHeight, 0, character.Position, move);
            moveBox.IgnoreGravity = true;
            moveBox.CollidesWith = Category.Cat1;
            moveBox.OnCollision += (geom1, geom2, list) => 
            {
                Vector2 power;
                if (move.maxWait == 0)
                    power = move.maxPower;
                else
                    power = chargeTime > move.maxWait ?
                        move.maxPower : (chargeTime - move.minWait) / (move.maxWait - move.minWait) * (move.maxPower - move.minPower) + move.minPower;
                
                geom2.Body.ApplyLinearImpulse(power);
                return false;
            };
            moveBox.Friction = 0;
            moveBox.LinearVelocity = move.sqSpeed;
        }
    }
}
