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
    public enum State { none, running, braking, jumping, falling, takingHit, attacking, shielding, chargingHit, chargingSuper }

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
        /// Attacking state. See above!
        /// </summary>
        private State _state;
        public State state
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    switch (_state)
                    {
                        case State.none:
                            attackMode = false;
                            inAir = false;
                            break;
                        case State.running:
                            attackMode = false;
                            character.StartAnimation("run", 0, 13, true);
                            break;
                        case State.braking:
                            attackMode = false;
                            break;
                        case State.jumping:
                            attackMode = false;
                            inAir = true;
                            break;
                        case State.falling:
                            attackMode = false;
                            inAir = true;
                            break;
                        case State.takingHit:
                            attackMode = true;
                            break;
                        case State.attacking:
                            attackMode = true;
                            break;
                        case State.shielding:
                            attackMode = true;
                            break;
                        case State.chargingHit:
                            attackMode = true;
                            break;
                        case State.chargingSuper:
                            attackMode = true;
                            break;
                    } 
                }
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
                if (value) character.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                else character.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            }
        }

        /// <summary>
        /// The character is in air or on ground?
        /// </summary>
        public bool _inAir;
        public bool inAir
        {
            get { return _inAir; }
            set
            {
                _inAir = value;
                if (value) 
                {
                    
                }
                else 
                {
                
                }
            }
        }
        
        /// <summary>
        /// Whether the character is in an attackstate.
        /// </summary>
        public bool attackMode;

        /// <summary>
        /// Number of jumps left. One of them is supermove, and once super is used, jumpsleft = 0.
        /// </summary>
        public int jumpsLeft = 3;

        /// <summary>
        /// An int that describes how much damage the player has taken in the current game/characterlife.
        /// </summary>
        public int damagePoints = 0;

        /// <summary>
        /// Number of lifes left.
        /// </summary>
        public int lifes;

        /// <summary>
        /// The current position of character sprite.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// The current X and Y navigation
        /// </summary>
        public Vector2 navigation;

        /// <summary>
        /// Whether the navigation has changed in either X or Y with more than something.
        /// </summary>
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

        /// <summary>
        /// How long the character has charged the attack.
        /// </summary>
        public float chargeTime;

        /// <summary>
        /// Time left to when attack is over.
        /// </summary>
        public float attackTimeLeft;

        /// <summary>
        /// Is set to true if the character releases attackbutton before minimun chargetime has passed.
        /// </summary>
        public bool startMoveWhenReady;

        /// <summary>
        /// Startposition for when the character comes to life.
        /// </summary>
        private Vector2 startPos;

        public CharacterController(ScreenController screen, GamepadController pad, Vector2 startPos) 
            : base(screen)
        {
            this.startPos = startPos;
            this.pad = pad;
            this.model = pad.SelectedCharacter;
            this.damagePoints = 0;
            this.playerIndex = pad.PlayerIndex;
        }

        public override void Load(ContentManager content)
        {
            character = new Sprite(content, model.animations, 200, 200, 200, 200);
            character.Scale = 0.6f;
            character.BoundRect(World, 60, 120);
            character.Layer = 100;
            character.FramesPerRow = 8;
            character.BoundBox.Friction = 0;
            character.BoundBox.IgnoreGravity = true;
            AddView(character);
            faceRight = true;
            inAir = true;

            character.BoundBox.CollisionCategories = Category.Cat11;
            character.BoundBox.CollidesWith = Category.All & ~Category.Cat11;
            character.BoundBox.OnCollision += Collision;
            character.BoundBox.OnSeparation += Seperation;
            
            character.Position = startPos;

            pad.OnNavigation += OnNavigation;
            pad.OnHitkeyDown += OnHitKeyDown;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if(!attackMode && Math.Abs(character.VelocityX) < Math.Abs(model.maxSpeed * navigation.X)) character.VelocityX += navigation.X * model.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if(inAir) character.VelocityY += model.gravity * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            else if(character.VelocityX * navigation.X < 0 || navigation.X == 0) character.VelocityX -= (character.VelocityX / model.maxSpeed) * 3 * model.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000;

            position = character.Position;

            switch (state)
            {
                case State.none:
                    break;
                case State.running:
                    if (navigation.X * character.VelocityX <= 0) state = State.braking;
                    else
                    {
                        character.fps = (int)MathHelper.Clamp(Math.Abs(character.VelocityX) * 5, 5, 100);
                    }
                    break;
                case State.braking:
                    if (navigation.X == 0 && Math.Floor(character.VelocityX) == 0) state = State.none;
                    else if (navigation.X * character.VelocityX >= 0) state = State.running;
                    break;
                case State.jumping:
                    if (character.VelocityY >= 0) state = State.falling;
                    break;
                case State.falling:
                    break;
                case State.chargingHit:
                    chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (startMoveWhenReady && chargeTime > move.minWait) BeginMove();
                    break;
                case State.chargingSuper:
                    chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (startMoveWhenReady && chargeTime > move.minWait || chargeTime > move.maxWait) BeginMove();
                    break;
                case State.attacking:
                    attackTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                    if (attackTimeLeft <= 0)
                    {
                        moveBox.Dispose();
                        state = State.none;
                    }
                    break;
                case State.shielding:
                    break;
            }

            string p = "Player " + pad.PlayerIndex+ " ";
            DebugWrite(p + "Jumps: ", jumpsLeft);
            DebugWrite(p + "State: ", state);
            DebugWrite(p + "inAir: ", newDirection);

        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        private void OnNavigation(float directionX, float directionY, int playerIndex, bool newDirection)
        {
            this.newDirection = newDirection;
            this.navigation = new Vector2(directionX, directionY);

            if (!attackMode)
            {
                if (!inAir && directionX !=0 && directionX * character.VelocityX >= 0) faceRight = directionX > 0;
                if (directionY < 0 && newDirection && jumpsLeft > 1)
                {
                    state = State.jumping;
                    character.VelocityY = -10;
                    jumpsLeft--;
                }
                if (directionY > 0.9)
                {
                    character.BoundBox.CollidesWith = Category.All & ~Category.Cat10 & ~Category.Cat11;
                    state = State.falling;
                    //character.BoundBox.Awake = true;
                }
                else character.BoundBox.CollidesWith = Category.All & ~Category.Cat11; 
            }
        }

        private void OnHitKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {
            if (state == State.none)
            {
                if (Math.Abs(navigation.X) >= Math.Abs(navigation.Y) || !inAir && navigation.Y > 0) moveDirection = faceRight ? new Vector2(1, 0) : new Vector2(-1, 0);
                else moveDirection = !inAir || navigation.Y < 0? new Vector2(0,-1) : new Vector2(0,1);
                if (newDirection)
                {
                    state = State.chargingHit;
                    startMoveWhenReady = false;
                    if (moveDirection.X != 0)
                        move = model.aLR;
                    else move = moveDirection.Y > 0 ? model.aDown : model.aUp;
                }
                else
                {
                    state = State.attacking;
                    move = model.a;
                    BeginMove();
                } 
            }
        }

        private void OnHitKeyUp(float downTimer, int playerIndex)
        {
            if (state == State.chargingHit)
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

            state = State.attacking;
        }

        private bool Collision(Fixture geom1, Fixture geom2, Contact list)
        {
            if ((geom2.CollisionCategories == Category.Cat9 || geom2.CollisionCategories == Category.Cat10) && (geom1.Body.Position.Y + character.size.Y / 2 <= geom2.Body.Position.Y + (float)geom2.Body.UserData / 2 && character.VelocityY >= 0))//(geom2.CollisionCategories == Category.All|| geom2.CollisionCategories == Category.Cat10) && 
            {
                inAir = false;
                if (!attackMode)
                {
                    state = character.VelocityX != 0 ?
                        (navigation.X * character.VelocityX > 0 ? State.running : State.braking) : State.none;
                }
                jumpsLeft = 3;
                character.VelocityY = 0;
                return true;
            }
            else if (geom2.CollisionCategories == Category.Cat20)
            {
                MoveInfo moveInfo = (MoveInfo)geom2.Body.UserData;
                if (!moveInfo.playerIndexes.Contains(playerIndex))
                {
                    ((MoveInfo)geom2.Body.UserData).playerIndexes.Add(playerIndex);
                    Move hit = moveInfo.move;
                    Vector2 direction = moveInfo.direction;
                    if(direction.Y == 0) direction.Y -= 0.4f;
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

        private void Seperation(Fixture geom1, Fixture geom2)
        {
            if ((geom2.CollisionCategories == Category.Cat10 || geom2.CollisionCategories ==  Category.Cat9) && jumpsLeft !=2)
            {
                if(!attackMode) state = State.falling;
                jumpsLeft = 2;
            }
        }
    }
}
