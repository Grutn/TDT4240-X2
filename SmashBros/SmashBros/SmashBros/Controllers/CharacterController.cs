using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using Microsoft.Xna.Framework;
using SmashBros.MySystem;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;
using System.Threading;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using System.Diagnostics;
using FarseerPhysics.Collision;
using SmashBros.Models;

namespace SmashBros.Controllers
{
    internal class MoveInfo
    {
        public Vector2 Xdirection;
        public float chargeTime;
        public List<int> PlayerIndexes;
        public MoveStats move;

        public MoveInfo(CharacterController controller)
        {
            Xdirection = controller.model.faceRight ? new Vector2(1, 1) : new Vector2(-1, 1);
            chargeTime = controller.move.chargeTime;
            PlayerIndexes = new List<int>();
            PlayerIndexes.Add(controller.model.playerIndex);
            move = controller.move.stats;
        }
    }

    public class CharacterController : Controller
    {
        /// <summary>
        /// The stats of the chosen character. STATIC attributes!!
        /// </summary>
        public CharacterStats stats;
        
        /// <summary>
        /// The charactermodel... Helloo
        /// </summary>
        public CharacterModel model;

        /// <summary>
        /// The characterview... Helloo
        /// </summary>
        public CharacterView view;

        /// <summary>
        /// The move.move the character is executing.
        /// </summary>
        //public MoveModel move;


        private MoveController moveC;

        /// <summary>
        /// DENNE BURDE FJERNES!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        private GamepadController pad;

        /// <summary>
        /// The current X and Y navigation
        /// </summary>
        public Vector2 navigation;

        /// <summary>
        /// Whether the navigation has changed in either X or Y with more than something.
        /// </summary>
        public bool newDirection;

        private bool _freezed;
        public bool Freezed
        {
            get { return _freezed; }
            set
            {
                view.Freezed = value;
                //move.view.Freezed = value;
            }
        }

        public CharacterController(ScreenManager screen, GamepadController pad, Vector2 startPos) 
            : base(screen)
        {
            model = new CharacterModel(pad, startPos);
            move = new MoveModel();
            stats = pad.SelectedCharacter;
            this.pad = pad;
        }

        public override void Load(ContentManager content)
        {
            view = new CharacterView(content, stats.animations, 200, 200, 200, 200, stats);
            model.view = view;
            view.Scale = 0.6f;
            view.BoundRect(World, stats.size.X, stats.size.Y);
            view.BoundBox.UserData = model;
            view.Layer = 100;
            view.FramesPerRow = 9;
            view.BoundBox.Friction = 0;
            view.BoundBox.IgnoreGravity = true;
            AddView(view);
            model.faceRight = true;
            model.inAir = true;

            view.BoundBox.CollisionCategories = Category.Cat11;
            view.BoundBox.CollidesWith = Category.All & ~Category.Cat11;
            view.BoundBox.OnCollision += Collision;
            view.BoundBox.OnSeparation += Seperation;
            
            view.Position = model.position;

            pad.OnNavigation += Navigation;
            pad.OnHitkeyDown += HitKeyDown;
            pad.OnHitKeyUp += HitKeyUp;
            pad.OnSuperkeyDown += SuperKeyDown;
            pad.OnSuperKeyUp += SuperKeyUp;

            moveC = new MoveController(Screen, stats);
            AddController(moveC);
            Screen.soundController.LoadCharacter(content, this);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!view.Freezed)
            {
                if (model.resetTimeLeft > 0)
                {
                    model.resetTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                    if (model.resetTimeLeft <= 0)
                    {
                        model.resetTimeLeft = -1;
                        view.Rotation = 0;
                        view.Scale = 0.6f;
                    }
                    else if (model.resetTimeLeft <= 2000 && model.resetTimeLeft >= 1000)
                    {
                        model.setState(CharacterState.falling);
                        AddView(view);
                        view.BoundBox.IsStatic = false;
                        view.Scale = (2 - model.resetTimeLeft / 1000) * 0.6f;
                        view.Rotation += (float)Math.PI * 2 * gameTime.ElapsedGameTime.Milliseconds / 1000;
                    }
                }
                else
                {
                    if (model.invounerable)
                    {
                        model.invounerableTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                        if (model.invounerableTimeLeft <= 0)
                        {
                            model.invounerable = false;
                            view.Opacity = 1;
                        }
                    }

                    // Calculating players new velocity

                    Vector2 velocityPlus = new Vector2(0, 0);
                    if (!model.attackMode && (Math.Abs(view.VelocityX) < Math.Abs(stats.maxSpeed * navigation.X) || view.VelocityX * navigation.X < 0))
                        velocityPlus.X += navigation.X * stats.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                    if (!model.attackMode || model.state == CharacterState.takingHit || move.stats.Type != MoveType.Body
                        || Math.Abs(((BodyMove)move.stats).BodyEnd - move.stats.Duration) > move.attackTimeLeft || Math.Abs(((BodyMove)move.stats).BodyStart - move.stats.Duration) < move.attackTimeLeft)
                    {
                        if (model.inAir)
                            velocityPlus.Y += stats.gravity * gameTime.ElapsedGameTime.Milliseconds / 1000f;
                        else if (view.VelocityX * navigation.X < 0 || navigation.X == 0)
                            velocityPlus.X += -(view.VelocityX / stats.maxSpeed) * 3 * stats.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000;
                    }

                    view.Velocity += velocityPlus;
                    if (model.state == CharacterState.attacking && move.stats.Type != MoveType.Range)
                    {
                        Vector2 moveVelocity = (move.stats.SqTo - move.stats.SqFrom) / (move.stats.End - move.stats.Start);
                        if (!model.faceRight) moveVelocity *= new Vector2(-1, 1);
                        move.view.LinearVelocity = moveVelocity + view.Velocity;
                    }

                    model.position = view.Position;
                    NaturalState();

                    switch (model.state)
                    {
                        case CharacterState.none:
                            break;
                        case CharacterState.running:
                            view.fps = (int)MathHelper.Clamp(Math.Abs(view.VelocityX) * 4, 5, 100);
                            break;
                        case CharacterState.braking:
                            break;
                        case CharacterState.jumping:
                            break;
                        case CharacterState.falling:
                            break;
                        case CharacterState.chargingHit:
                            move.chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (move.startMoveWhenReady && move.chargeTime > ((ChargeMove)move.stats).MinWait) model.setState(CharacterState.attacking, move.stats);
                            break;
                        case CharacterState.chargingSuper:
                            move.chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (move.startMoveWhenReady && move.chargeTime > ((ChargeMove)move.stats).MinWait || move.chargeTime > ((ChargeMove)move.stats).MaxWait) model.setState(CharacterState.attacking);
                            break;
                        case CharacterState.attacking:
                            move.attackTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                            if (move.attackTimeLeft <= 0)
                            {
                                model.attackMode = false;
                                if (move.stats.Type != MoveType.Range)
                                    move.view.Dispose();
                                NaturalState();
                            }
                            else if (move.attackTimeLeft <= Math.Abs(move.stats.Start - move.stats.Duration))
                            {
                                StartMove();
                                if (move.stats.Adjustable)
                                {
                                    float angle = move.view.Velocity.X == 0 ? 0 : (float)Math.Atan(move.view.Velocity.Y / move.view.Velocity.X);
                                    angle += model.faceRight ?
                                        navigation.Y * ((AdjustableMove)move.stats).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000 :
                                        -navigation.Y * ((AdjustableMove)move.stats).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000;
                                    move.view.Velocity = new Vector2(angle) * move.view.Velocity.Length();
                                }
                            }
                            else if (move.attackTimeLeft <= Math.Abs(move.stats.End - move.stats.Duration))
                            {
                                move.view.Dispose();
                            }
                            break;
                        case CharacterState.shielding:
                            break;
                        case CharacterState.takingHit:
                            if (view.VelocityY >= 0)
                            {
                                if (move.stats.Type != MoveType.Range) move.view.Dispose();
                                model.attackMode = false;
                                NaturalState();
                            }
                            break;
                    }

                    string p = "Player " + model.playerIndex + " ";
                    DebugWrite(p + "Jumps: ", model.jumpsLeft);
                    DebugWrite(p + "State: ", model.state);
                    DebugWrite(p + "inAir: ", newDirection);
                } 
            }
        }

        public void Reset(Vector2 startPos, bool behindMap)
        {
            RemoveView(view);
            view.BoundBox.IsStatic = true;
            view.Position = startPos;
            model.resetTimeLeft = 4000;
            model.invounerableTimeLeft = 3000;
            model.invounerable = true;
            if (move.view != null) move.view.Dispose();
        }

        private void NaturalState()
        {
            if (!model.attackMode)
            {
                if (model.inAir) model.setState(view.VelocityY < 0? CharacterState.jumping : CharacterState.falling);
                else if (Math.Round(view.VelocityX) == 0) model.setState(CharacterState.none);
                else if (navigation.X == 0 || navigation.X * view.VelocityX < 0) model.setState(CharacterState.braking);
                else model.setState(CharacterState.running);
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
            if (!model.attackMode && model.resetTimeLeft < 0)
            {
                if (directionX !=0 && directionX * view.VelocityX >= 0) model.faceRight = directionX > 0;
                if (directionY < 0 && newDirection && model.jumpsLeft > 1)
                {
                    model.setState(CharacterState.jumping);
                    view.VelocityY = -10;
                    model.jumpsLeft--;
                }
                if (directionY > 0.9)
                {
                    view.BoundBox.CollidesWith = Category.All & ~Category.Cat10 & ~Category.Cat11;
                    if(model.onSoftBox) model.setState(CharacterState.falling);
                }
                else view.BoundBox.CollidesWith = Category.All & ~Category.Cat11; 
            }

            this.newDirection = newDirection;
            this.navigation = new Vector2(directionX, directionY);
        }

        private void HitKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {
            if (!model.attackMode && !view.Freezed)
            {
                if (newDirection && (Math.Abs(navigation.X) > 0.9 || Math.Abs(navigation.Y) > 0.9))
                {
                    if (Math.Abs(navigation.X) >= Math.Abs(navigation.Y) || !model.inAir && navigation.Y > 0) move.stats = stats.aLR;
                    else move.stats = model.inAir && navigation.Y > 0 ? stats.aDown : stats.aUp;
                }
                else
                {
                    move.stats = stats.a;
                }

                Debug.WriteLine("new move");
                if (move.stats != null)
                {
                    if (move.stats.Type == MoveType.Charge)
                    {
                        model.setState(CharacterState.chargingHit);
                    }
                    else
                    {
                        model.setState(CharacterState.attacking, move.stats);
                        if (move.stats.Start == 0) StartMove();
                    } 
                }
            }
        }

        private void SuperKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {
            if (!model.attackMode && !view.Freezed)
            {
                if (navigation.X == 0 && navigation.Y == 0)
                {
                    move.stats = Math.Abs(view.VelocityX) < 3 ? move.stats = stats.x : stats.xLR;
                }
                else if (Math.Abs(navigation.X) > Math.Abs(navigation.Y)) move.stats = stats.xLR;
                else if (navigation.Y > 0) move.stats = stats.xDown;
                else if (model.jumpsLeft > 0)
                {
                    move.stats = stats.xUp;
                    model.jumpsLeft = 0;
                }
                else return;

                Debug.WriteLine("new move");
                if (move.stats != null)
                {
                    if (move.stats.Type == MoveType.Charge)
                    {
                        move.startMoveWhenReady = false;
                        model.setState(CharacterState.chargingHit);
                    }
                    else
                    {
                        model.setState(CharacterState.attacking);
                        if (move.stats.Start == 0) StartMove();
                    } 
                }
            }
        }

        private void HitKeyUp(float downTimer, int playerIndex)
        {
            if (model.state == CharacterState.chargingHit && !view.Freezed)
            {
                if (move.chargeTime > ((ChargeMove)move.stats).MinWait) StartMove();
                else move.startMoveWhenReady = true;
            }
        }

        private void SuperKeyUp(float downTimer, int playerIndex)
        {
            if (model.state == CharacterState.chargingSuper && !view.Freezed)
            {
                if (move.chargeTime > ((ChargeMove)move.stats).MinWait) StartMove();
                else move.startMoveWhenReady = true;
            }
        }

        private void StartMove()
        {
            if (!move.moveStarted)
            {
                

                move.view = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(move.stats.SqSize.X), ConvertUnits.ToSimUnits(move.stats.SqSize.Y), 0,
                        view.BoundBox.Position + move.stats.SqFrom, new MoveInfo(this));
                move.view.IgnoreGravity = true;
                move.view.IsStatic = false;
                move.view.CollidesWith = Category.Cat11;
                move.view.CollisionCategories = Category.Cat20;

                Vector2 velocity = move.stats.Type != MoveType.Range?
                    (move.stats.SqTo - move.stats.SqFrom) / (move.stats.End - move.stats.Start) : ((RangeMove)move.stats).BulletVelocity;
                if (!model.faceRight) velocity *= new Vector2(-1, 1);
                move.view.LinearVelocity = velocity + view.Velocity;
                move.view.UserData = new MoveInfo(this);

                move.moveStarted = true;
            }
        }

        private bool Collision(Fixture chara, Fixture obj, Contact list)
        {
            if ((obj.CollisionCategories == Category.Cat9 || obj.CollisionCategories == Category.Cat10)
                && (chara.Body.Position.Y + view.size.Y / 2 <= obj.Body.Position.Y - (float)obj.Body.UserData / 2 && view.VelocityY >= 0))
            {
                model.inAir = false;
                if (obj.CollisionCategories == Category.Cat10) model.onSoftBox = true;
                NaturalState();
                model.jumpsLeft = 3;
                view.VelocityY = 0;
                return true;
            }
            else if (obj.CollisionCategories == Category.Cat9) return true;
            else if (obj.CollisionCategories == Category.Cat20)
            {
                MoveInfo moveInfo = (MoveInfo)obj.Body.UserData;
                if (!moveInfo.PlayerIndexes.Contains(model.playerIndex) && !model.invounerable)
                {
                    moveInfo.PlayerIndexes.Add(model.playerIndex);

                    MoveStats move = moveInfo.move;
                    float ratio = 0;
                    if (move.Type == MoveType.Charge && moveInfo.chargeTime < ((ChargeMove)move).MaxWait)
                        ratio = moveInfo.chargeTime / ((ChargeMove)move).MaxWait;
                    else ratio = 1;
                    Vector2 power = ratio * move.Power;
                    int damage = (int)ratio * move.Damage;

                    view.Velocity = moveInfo.Xdirection * power * (1 + model.damagePoints / 100);
                    if (Math.Abs(view.VelocityY) == 0) view.VelocityY = -1;
                    model.damagePoints += damage;
                    model.setState(CharacterState.takingHit);

                    if (OnHit != null) OnHit.Invoke(ConvertUnits.ToDisplayUnits(obj.Body.Position), damage, model.damagePoints, moveInfo.PlayerIndexes.First(), model.playerIndex, move.hitSound);

                    //if (move.Adjustable && ((AdjustableMove)move).StopAtHit) move.attackTimeLeft = 0;
                }
            }
            else if (obj.CollisionCategories == Category.Cat7 || obj.CollisionCategories == Category.Cat8) OnCharacterDeath.Invoke(this, obj.CollisionCategories == Category.Cat7);
            
            return false;
        }

        private void Seperation(Fixture geom1, Fixture geom2)
        {
            if (geom2.CollisionCategories == Category.Cat10 || geom2.CollisionCategories ==  Category.Cat9)
            {
                model.inAir = true;
                if (!model.attackMode && model.jumpsLeft != 2) model.setState(CharacterState.falling);
                model.onSoftBox = false;
                model.jumpsLeft = 2;
                view.BoundBox.Awake = true;
                view.VelocityY -= 0.5f;
            }
        }

        public delegate void HitOccured(Vector2 pos, int damageDone, int newDamagepoints, int puncher_playerIndex, int reciever_playerIndex, GameSoundType soundtype);
        public delegate void CharacterDied(CharacterController characterController, bool behindScreen = false);

        public event HitOccured OnHit;
        public event CharacterDied OnCharacterDeath;
    }
}
