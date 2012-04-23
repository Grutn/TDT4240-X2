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
using FarseerPhysics.Dynamics.Contacts;
using System.Diagnostics;

namespace SmashBros.Controllers
{
    public enum MovementState { none, jumping, running, falling }

    public enum AttackState { none, attacking, shielding, chargingHit, chargingSuper }

    class MoveInfo { public Vector2 direction; public float chargeTime; public List<int> playerIndexes; public Move move; }

    class CharacterController : Controller
    {
        public int playerIndex;
        
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

        public Vector2 navigation;

        public bool newDirection;

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

        public Vector2 moveDirection;

        public float chargeTime;

        public float attackTimeLeft;

        public bool startMoveWhenReady;

        public CharacterController(ScreenController screen, GamepadController pad) 
            : base(screen)
        {
            this.pad = pad;
            model = pad.SelectedCharacter;
            damagePoints = 0;
            faceRight = true;
            playerIndex = pad.PlayerIndex;
        }

        public override void Load(ContentManager content)
        {
            character = new Sprite(content, "spiderman", 100, 100, 200, 200);
            character.BoundRect(World, 100, 100);
            AddView(character);

            character.BoundBox.CollisionCategories = Category.Cat11;
            character.BoundBox.CollidesWith = Category.All & ~Category.Cat11;
            character.BoundBox.OnCollision += Collision;

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
                case AttackState.attacking:
                    attackTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                    if (attackTimeLeft <= 0)
                    {
                        moveBox.Dispose();
                        attackState = AttackState.none;
                    }
                    break;
                case AttackState.shielding:

                    break;
            }

            if (startMoveWhenReady && chargeTime > move.minWait) BeginMove();
            
            position = character.Position;

            if (Constants.DebugMode)
            {
                screen.controllerViewManager.debugView.PlayerStates[pad.PlayerIndex] =
                    movementState.ToString() + " & " + attackState.ToString()
                    + "\n jumpsLeft: " +jumpsLeft
                    + "\n Ypos: " + character.PositionY
                    + "\n YposChar: " + position.Y
                    + "\n newDir: " + newDirection;
            }

        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        private void OnNavigation(float directionX, float directionY, int playerIndex, bool newDirection)
        {
            this.newDirection = newDirection;//(Math.Abs(navigation.X - directionX) > 0.7 && Math.Abs(directionX) > 0.9) || (Math.Abs(navigation.Y - directionY) > 0.7 && Math.Abs(directionY) > 0.9);
            this.navigation = new Vector2(directionX, directionY);
            if (directionY < 0)
            {
                if (this.newDirection && jumpsLeft > 1)
                {
                    movementState = MovementState.jumping;
                    character.VelocityY = -10;
                    jumpsLeft--;
                }
            }
            
            if (directionY > 0.9)// && character.BoundBox.ContactList.Contact.FixtureB.CollisionCategories == Category.Cat10)
            {
                character.BoundBox.CollidesWith = Category.All & ~Category.Cat10 & ~Category.Cat11;
                character.BoundBox.Awake = true;
            }
            else character.BoundBox.CollidesWith = Category.All & ~Category.Cat11;
            
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
            
            if(attackState == AttackState.none && directionX!=0) faceRight = directionX > 0;

        }

        private void OnHitKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {

            if (attackState == AttackState.none)
            {
                if(directionX == 0 && directionY == 0)
                    moveDirection.X = faceRight? 1 : -1;
                else if(Math.Abs(directionX) > Math.Abs(directionY))
                    moveDirection.X = directionX > 0? 1 : -1;
                else
                    moveDirection.Y = directionY > 0? 1 : -1;

                if (movementState == MovementState.none && (directionX > 0.9 || directionY > 0.9))
                {
                    attackState = AttackState.chargingHit;
                    startMoveWhenReady = false;
                    if (Math.Abs(directionX) > Math.Abs(directionY))
                        move = model.aLR;
                    else if (directionY > 0) move = model.aDown;
                    else move = model.aUp;
                }
                else
                {
                    attackState = AttackState.attacking;
                    move = model.a;
                    BeginMove();
                } 
            }
        }

        private void OnHitKeyUp(float downTimer, int playerIndex)
        {
            if (attackState == AttackState.chargingHit)
            {
                if (chargeTime > move.minWait) BeginMove();
                else startMoveWhenReady = true;
            }
        }

        private void BeginMove()
        {
            attackTimeLeft = move.duration;
            moveBox = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(move.sqWidth), ConvertUnits.ToSimUnits(move.sqHeight), 0, character.BoundBox.Position, move);
            moveBox.IgnoreGravity = true;
            moveBox.IsStatic = false;
            moveBox.CollidesWith = Category.Cat11;
            moveBox.CollisionCategories = Category.Cat20;
            moveBox.Friction = 0;
            moveBox.LinearVelocity = moveDirection*move.sqRange/move.duration;
            moveBox.UserData = new MoveInfo() { direction = moveDirection, chargeTime = chargeTime, playerIndexes = new List<int>() { playerIndex }, move = move };
        }

        private bool Collision(Fixture geom1, Fixture geom2, Contact list)
        {
            if (geom1.Body.Position.Y + character.size.Y / 2 <= geom2.Body.Position.Y)//(geom2.CollisionCategories == Category.All|| geom2.CollisionCategories == Category.Cat10) && 
            {
                jumpsLeft = 3;
                return true;
            }
            else if (geom2.CollisionCategories == Category.Cat20)
            {
                MoveInfo moveInfo = (MoveInfo)geom2.Body.UserData;
                if (!moveInfo.playerIndexes.Contains(playerIndex))
                {
                    //Debug.WriteLine("HIIIIIIIT");

                    ((MoveInfo)geom2.Body.UserData).playerIndexes.Add(playerIndex);
                    Move hit = moveInfo.move;
                    Vector2 direction = moveInfo.direction;
                    float chargeTime = moveInfo.chargeTime;
                    float ratio = 0;
                    if (chargeTime > hit.maxWait) ratio = 1;
                    else ratio = (chargeTime - hit.minWait) / (hit.maxWait - hit.minWait);
                    int power = (int)ratio * (hit.maxPower - hit.minPower) + hit.minPower;

                    character.BoundBox.ApplyLinearImpulse(direction * power * (1 + damagePoints / 100));
                    damagePoints += (int)ratio * (hit.maxDamage - hit.minDamage) + hit.minDamage;
                }
            }
            return false;
        }
    }
}
