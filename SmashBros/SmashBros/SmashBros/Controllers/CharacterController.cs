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
using FarseerPhysics.Collision;

namespace SmashBros.Controllers
{
    public enum State { none, running, braking, jumping, falling, takingHit, attacking, shielding, chargingHit, chargingSuper }

    internal class MoveInfo
    {
        public Vector2 Xdirection;
        public float ChargeTime;
        public List<int> PlayerIndexes;
        public Move Move;

        public MoveInfo(CharacterController controller)
        {
            Xdirection = controller.faceRight ? new Vector2(1, 1) : new Vector2(-1, 1);
            ChargeTime = controller.chargeTime;
            PlayerIndexes = new List<int>();
            PlayerIndexes.Add(controller.playerIndex);
            Move = controller.move;
        }
    }

    public class CharacterController : Controller
    {
        public int playerIndex;
        
        /// <summary>
        /// Currently chosen character of this player.
        /// </summary>
        public Sprite sprite;

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
                    if (value != State.running) sprite.fps = Constants.FPS;
                    switch (value)
                    {
                        case State.none:
                            attackMode = false;
                            inAir = false;
                            if (_state == State.running || _state == State.attacking) sprite.AddAnimation(0, 2, true);
                            else if (_state == State.falling || _state == State.jumping)
                            {
                                sprite.StartAnimation(model.ani_landStart, model.ani_landEnd, false);
                                sprite.AddAnimation(model.ani_noneStart, model.ani_noneEnd, true);
                            }
                            else sprite.StartAnimation(model.ani_noneStart, model.ani_noneEnd, true);
                            break;
                        case State.running:
                            attackMode = false;
                            if (_state == State.falling || _state == State.jumping)
                            {
                                sprite.StartAnimation(model.ani_landStart, model.ani_landEnd, false);
                                sprite.AddAnimation(model.ani_runStart, model.ani_runEnd, true);
                            }
                            else sprite.StartAnimation(model.ani_runStart, model.ani_runEnd, true);
                            break;
                        case State.braking:
                            attackMode = false;
                            if (_state == State.falling || _state == State.jumping)
                            {
                                sprite.StartAnimation(model.ani_landStart, model.ani_landEnd, false);
                                sprite.AddAnimation(model.ani_brake, model.ani_brake, true);
                            }
                            else sprite.StartAnimation(model.ani_brake, model.ani_brake, true);
                            break;
                        case State.jumping:
                            attackMode = false;
                            inAir = true;
                            PlayerSound.Invoke(playerIndex, PlayerSoundType.Jump);
                            sprite.StartAnimation(model.ani_jumpStart, model.ani_jumpEnd, false);
                            sprite.AddAnimation(model.ani_fallStart, model.ani_fallEnd, true);
                            break;
                        case State.falling:
                            attackMode = false;
                            inAir = true;
                            sprite.StartAnimation(model.ani_fallStart, model.ani_fallEnd, true);
                            break;
                        case State.takingHit:
                            attackMode = true;
                            sprite.StartAnimation(model.ani_takeHitStart, model.ani_takeHitEnd, true);
                            break;
                        case State.attacking:
                            attackMode = true;
                            sprite.StartAnimation(move.AniFrom, move.AniTo, false);
                            if (move.Type == MoveType.Body) sprite.Velocity = ((BodyMove)move).BodySpeed;
                            break;
                        case State.shielding:
                            attackMode = true;
                            //invounerable = true;
                            break;
                        case State.chargingHit:
                            attackMode = true;
                            break;
                        case State.chargingSuper:
                            attackMode = true;
                            break;
                    }
                    _state = value;
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
                if (value) sprite.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                else sprite.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
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
        /// Whether the character is in an attackstate.
        /// </summary>
        public bool attackMode;
        
        /// <summary>
        /// Whether or not the player is in an invounerable state.
        /// </summary>
        private bool _invounerable;
        public bool invounerable
        {
            get { return _invounerable; }
            set
            {
                _invounerable = value;
                sprite.Blinking = value;
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
        /// Whether the movebox have bin created etc.
        /// </summary>
        private bool moveStarted;

        /// <summary>
        /// Startposition for when the character comes to life.
        /// </summary>
        private Vector2 startPos;

        public CharacterController(ScreenManager screen, GamepadController pad, Vector2 startPos) 
            : base(screen)
        {
            this.startPos = startPos;
            this.pad = pad;
            this.model = pad.SelectedCharacter;
            this.damagePoints = 0;
            this.playerIndex = pad.PlayerIndex;
        }

        public void Reset(Vector2 startPos, bool behindMap)
        {
            RemoveView(sprite);
            sprite.BoundBox.IsStatic = true;
            sprite.Position = startPos;
            resetTimeLeft = 4000;
            invounerableTimeLeft = 3000;
            invounerable = true;
            if(moveBox != null) moveBox.Dispose();
        }

        public override void Load(ContentManager content)
        {
            sprite = new Sprite(content, model.animations, 200, 200, 200, 200);
            sprite.Scale = 0.6f;
            sprite.BoundRect(World, model.size.X, model.size.Y);
            sprite.Layer = 100;
            sprite.FramesPerRow = 9;
            sprite.BoundBox.Friction = 0;
            sprite.BoundBox.IgnoreGravity = true;
            AddView(sprite);
            faceRight = true;
            inAir = true;

            sprite.BoundBox.CollisionCategories = Category.Cat11;
            sprite.BoundBox.CollidesWith = Category.All & ~Category.Cat11;
            sprite.BoundBox.OnCollision += Collision;
            sprite.BoundBox.OnSeparation += Seperation;
            
            sprite.Position = startPos;

            pad.OnNavigation += Navigation;
            pad.OnHitkeyDown += HitKeyDown;
            pad.OnHitKeyUp += HitKeyUp;
            pad.OnSuperkeyDown += SuperKeyDown;
            pad.OnSuperKeyUp += SuperKeyUp;

            Screen.soundController.LoadCharacter(content, this);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (resetTimeLeft > 0)
            {
                resetTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                if (resetTimeLeft <= 0)
                {
                    resetTimeLeft = -1;
                    sprite.Rotation = 0;
                    sprite.Scale = 0.6f;
                }
                else if (resetTimeLeft <= 2000 && resetTimeLeft >= 1000)
                {
                    state = State.falling;
                    AddView(sprite);
                    sprite.BoundBox.IsStatic = false;
                    sprite.Scale = (2 - resetTimeLeft / 1000) * 0.6f;
                    sprite.Rotation += (float)Math.PI * 2 * gameTime.ElapsedGameTime.Milliseconds / 1000;
                }
            }
            else
            {
                if (invounerable)
                {
                    invounerableTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                    if (invounerableTimeLeft <= 0)
                    {
                        invounerable = false;
                        sprite.Opacity = 1;
                    }
                }

                // Calculating players new velocity

                Vector2 velocityPlus = new Vector2(0, 0);
                if (!attackMode && (Math.Abs(sprite.VelocityX) < Math.Abs(model.maxSpeed * navigation.X) || sprite.VelocityX * navigation.X < 0))
                    velocityPlus.X += navigation.X * model.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                if (!attackMode || state == State.takingHit || move.Type != MoveType.Body 
                    || Math.Abs(((BodyMove)move).BodyEnd - move.Duration) > attackTimeLeft || Math.Abs(((BodyMove)move).BodyStart - move.Duration) < attackTimeLeft)
                {
                    if (inAir)
                        velocityPlus.Y += model.gravity * gameTime.ElapsedGameTime.Milliseconds / 1000f;
                    else if (sprite.VelocityX * navigation.X < 0 || navigation.X == 0)
                        velocityPlus.X += -(sprite.VelocityX / model.maxSpeed) * 3 * model.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000; 
                }

                sprite.Velocity += velocityPlus;
                if (state == State.attacking && move.Type != MoveType.Range) moveBox.LinearVelocity += velocityPlus;

                position = sprite.Position;
                NaturalState();

                switch (state)
                {
                    case State.none:
                        break;
                    case State.running:
                        sprite.fps = (int)MathHelper.Clamp(Math.Abs(sprite.VelocityX) * 4, 5, 100);
                        break;
                    case State.braking:
                        break;
                    case State.jumping:
                        break;
                    case State.falling:
                        break;
                    case State.chargingHit:
                        chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                        if (startMoveWhenReady && chargeTime > ((ChargeMove)move).MinWait) state = State.attacking;
                        break;
                    case State.chargingSuper:
                        chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                        if (startMoveWhenReady && chargeTime > ((ChargeMove)move).MinWait || chargeTime > ((ChargeMove)move).MaxWait) state = State.attacking;
                        break;
                    case State.attacking:
                        attackTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                        if (attackTimeLeft <= 0)
                        {
                            attackMode = false;
                            NaturalState();
                        }
                        else if (Math.Abs(move.Start - move.Duration) <= attackTimeLeft)
                        {
                            if (!moveStarted) StartMove();
                            if (move.Adjustable)
                            {
                                float angle = moveBox.LinearVelocity.X == 0 ? 0 : (float)Math.Atan(moveBox.LinearVelocity.Y / moveBox.LinearVelocity.X);
                                angle += faceRight ?
                                    navigation.Y * ((AdjustableMove)move).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000 :
                                    -navigation.Y * ((AdjustableMove)move).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000;
                                moveBox.LinearVelocity = new Vector2(angle) * moveBox.LinearVelocity.Length();
                            }
                        }
                        else if (Math.Abs(move.End - move.Duration) <= attackTimeLeft)
                        {
                            moveBox.Dispose();
                        }
                        break;
                    case State.shielding:
                        break;
                    case State.takingHit:
                        if (sprite.VelocityY >= 0)
                        {
                            attackMode = false;
                            NaturalState();
                        }
                        break;
                }

                string p = "Player " + pad.PlayerIndex + " ";
                DebugWrite(p + "Jumps: ", jumpsLeft);
                DebugWrite(p + "State: ", state);
                DebugWrite(p + "inAir: ", newDirection);
            }
        }

        private void NaturalState()
        {
            if (!attackMode)
            {
                if (inAir) state = sprite.VelocityY < 0? State.jumping : State.falling;
                else if (Math.Round(sprite.VelocityX) == 0) state = State.none;
                else if (navigation.X == 0 || navigation.X * sprite.VelocityX < 0) state = State.braking;
                else state = State.running;
            }
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        private void Navigation(float directionX, float directionY, int playerIndex, bool newDirection)
        {
            if (!attackMode && resetTimeLeft < 0)
            {
                if (directionX !=0 && directionX * sprite.VelocityX >= 0) faceRight = directionX > 0;
                if (directionY < 0 && newDirection && jumpsLeft > 1)
                {
                    state = State.jumping;
                    sprite.VelocityY = -10;
                    jumpsLeft--;
                }
                if (directionY > 0.9)
                {
                    sprite.BoundBox.CollidesWith = Category.All & ~Category.Cat10 & ~Category.Cat11;
                    if(onSoftBox) state = State.falling;
                }
                else sprite.BoundBox.CollidesWith = Category.All & ~Category.Cat11; 
            }

            this.newDirection = newDirection;
            this.navigation = new Vector2(directionX, directionY);
        }

        private void HitKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {
            if (!attackMode)
            {
                if (newDirection && (Math.Abs(navigation.X) > 0.9 || Math.Abs(navigation.Y) > 0.9))
                {
                    if (Math.Abs(navigation.X) >= Math.Abs(navigation.Y) || !inAir && navigation.Y > 0) move = model.aLR;
                    else move = inAir && navigation.Y > 0 ? model.aDown : model.aUp;
                }
                else
                {
                    move = model.a;
                }

                if (move != null)
                {
                    if (move.Type == MoveType.Charge)
                    {
                        startMoveWhenReady = false;
                        state = State.chargingHit;
                    }
                    else
                    {
                        state = State.attacking;
                        if (move.Start == 0) StartMove();
                    } 
                }
            }
        }

        private void SuperKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {
            if (!attackMode)
            {
                if (navigation.X == 0 && navigation.Y == 0)
                {
                    move = Math.Abs(sprite.VelocityX) < 3 ? move = model.x : model.xLR;
                }
                else if (Math.Abs(navigation.X) > Math.Abs(navigation.Y)) move = model.xLR;
                else if (navigation.Y > 0) move = model.xDown;
                else if (jumpsLeft > 0)
                {
                    move = model.xUp;
                    jumpsLeft = 0;
                }
                else return;

                if (move != null)
                {
                    if (move.Type == MoveType.Charge)
                    {
                        startMoveWhenReady = false;
                        state = State.chargingHit;
                    }
                    else
                    {
                        state = State.attacking;
                        if (move.Start == 0) StartMove();
                    } 
                }
            }
        }

        private void HitKeyUp(float downTimer, int playerIndex)
        {
            if (state == State.chargingHit)
            {
                if (chargeTime > ((ChargeMove)move).MinWait) StartMove();
                else startMoveWhenReady = true;
            }
        }

        private void SuperKeyUp(float downTimer, int playerIndex)
        {
            if (state == State.chargingSuper)
            {
                if (chargeTime > ((ChargeMove)move).MinWait) StartMove();
                else startMoveWhenReady = true;
            }
        }

        private void StartMove()
        {
            moveBox = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(move.SqSize.X), ConvertUnits.ToSimUnits(move.SqSize.Y), 0,
                sprite.BoundBox.Position + move.SqFrom, new MoveInfo(this));
            moveBox.IgnoreGravity = true;
            moveBox.IsStatic = false;
            moveBox.CollidesWith = Category.Cat11;
            moveBox.CollisionCategories = Category.Cat20;
            
            Vector2 speed = (move.SqTo - move.SqFrom) / (move.End - move.Start);
            moveBox.LinearVelocity = faceRight ? (speed + sprite.Velocity) * new Vector2(1, 1) : (speed + sprite.Velocity) * new Vector2(-1, 1);
            moveBox.UserData = new MoveInfo(this);
        }

        private bool Collision(Fixture chara, Fixture obj, Contact list)
        {
            if ((obj.CollisionCategories == Category.Cat9 || obj.CollisionCategories == Category.Cat10)
                && (chara.Body.Position.Y + sprite.size.Y / 2 <= obj.Body.Position.Y - (float)obj.Body.UserData / 2 && sprite.VelocityY >= 0))
            {
                inAir = false;
                if (obj.CollisionCategories == Category.Cat10) onSoftBox = true;
                NaturalState();
                jumpsLeft = 3;
                sprite.VelocityY = 0;
                return true;
            }
            else if (obj.CollisionCategories == Category.Cat9) return true;
            else if (obj.CollisionCategories == Category.Cat20)
            {
                MoveInfo moveInfo = (MoveInfo)obj.Body.UserData;
                if (!moveInfo.PlayerIndexes.Contains(playerIndex) && !invounerable)
                {
                    moveInfo.PlayerIndexes.Add(playerIndex);

                    Move move = moveInfo.Move;
                    float ratio = 0;
                    if (move.Type == MoveType.Charge && moveInfo.ChargeTime < ((ChargeMove)move).MaxWait)
                        ratio = moveInfo.ChargeTime / ((ChargeMove)move).MaxWait;
                    else ratio = 1;
                    Vector2 power = ratio * move.Power;
                    int damage = (int)ratio * move.Damage;

                    sprite.Velocity = moveInfo.Xdirection * power * (1 + damagePoints / 100);
                    if (Math.Abs(sprite.VelocityY) == 0) sprite.VelocityY = -1;
                    damagePoints += damage;
                    state = State.takingHit;

                    if (OnHit != null) OnHit.Invoke(ConvertUnits.ToDisplayUnits(obj.Body.Position), damage, damagePoints, moveInfo.PlayerIndexes.First(), playerIndex, move.hitSound);

                    if (move.Adjustable && ((AdjustableMove)move).StopAtHit) attackTimeLeft = 0;
                }
            }
            else if (obj.CollisionCategories == Category.Cat7 || obj.CollisionCategories == Category.Cat8) OnCharacterDeath.Invoke(this, obj.CollisionCategories == Category.Cat7);
            
            return false;
        }

        private void Seperation(Fixture geom1, Fixture geom2)
        {
            if (geom2.CollisionCategories == Category.Cat10 || geom2.CollisionCategories ==  Category.Cat9)
            {
                inAir = true;
                if (!attackMode && jumpsLeft != 2) state = State.falling;
                onSoftBox = false;
                jumpsLeft = 2;
                sprite.BoundBox.Awake = true;
                sprite.VelocityY -= 0.5f;
            }
        }

        public delegate void HitOccured(Vector2 pos, int damageDone, int newDamagepoints, int puncher_playerIndex, int reciever_playerIndex, GameSoundType soundtype);
        public delegate void CharacterDied(CharacterController characterController, bool behindScreen = false);

        public event SmashBros.Controllers.SoundController.PlayerSound PlayerSound;
        public event HitOccured OnHit;
        public event CharacterDied OnCharacterDeath;
    }
}
