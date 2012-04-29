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
        public MoveModel currentMove;


        /// <summary>
        /// Is set to true if the character releases attackbutton before minimun chargetime has passed.
        /// </summary>
        private bool startAfterMinCharge = false;

        private MoveController moves;

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
                if (value) moves.Freeze();
                else moves.UnFreeze();
            }
        }

        public CharacterController(ScreenManager screen, GamepadController pad, Vector2 startPos, int countDown) 
            : base(screen)
        {
            stats = pad.PlayerModel.SelectedCharacter;
            model = new CharacterModel(pad, startPos, countDown, stats);
            moves = new MoveController(Screen, stats, pad.PlayerIndex);
            this.pad = pad;
        }

        public override void Load(ContentManager content)
        {
            view = new CharacterView(content, stats.animations, 200, 200, 200, 200, stats);
            model.view = view;
            view.Scale = 0.6f;
            view.BoundRect(World, stats.size.X, stats.size.Y);
            view.BoundBox.UserData = this;
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

            AddController(moves);
            Screen.soundController.LoadCharacter(content, this);
        }

        public override void Unload()
        {
            pad.OnNavigation -= Navigation;
            pad.OnHitkeyDown -= HitKeyDown;
            pad.OnHitKeyUp -= HitKeyUp;
            pad.OnSuperkeyDown -= SuperKeyDown;
            pad.OnSuperKeyUp -= SuperKeyUp;

            if (view.BoundBox != null && view.BoundBox.FixtureList != null)
            {
                view.BoundBox.OnCollision -= Collision;
                view.BoundBox.OnSeparation -= Seperation;
            }

            SubscribeToGameState = false;

            DisposeView(view);
            System.GC.SuppressFinalize(this);
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
                    else if (model.resetTimeLeft <= 4000 && model.resetTimeLeft >= 2000)
                    {
                        model.setState(CharacterState.falling);
                        AddView(view);
                        view.BoundBox.IsStatic = false;
                        view.Scale = 0.6f - (model.resetTimeLeft - 2000) / 2000 * 0.6f;
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
                    if (!model.attackMode && (Math.Abs(view.VelocityX) < Math.Abs(model.maxSpeed * navigation.X) || view.VelocityX * navigation.X < 0))
                        velocityPlus.X += navigation.X * model.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                    if (!model.attackMode || model.state == CharacterState.takingHit || currentMove.Stats.Type != MoveType.Body
                        || Math.Abs(((BodyMove)currentMove.Stats).BodyEnd - currentMove.Stats.Duration) > currentMove.attackTimeLeft || Math.Abs(((BodyMove)currentMove.Stats).BodyStart - currentMove.Stats.Duration) < currentMove.attackTimeLeft)
                    {
                        if (model.inAir)
                            velocityPlus.Y += stats.gravity * gameTime.ElapsedGameTime.Milliseconds / 1000f;
                        else if (view.VelocityX * navigation.X < 0 || navigation.X == 0)
                            velocityPlus.X += -(view.VelocityX / model.maxSpeed) * 3 * model.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000;
                    }

                    view.Velocity += velocityPlus;
                    if (model.state == CharacterState.attacking && currentMove.Stats.Type != MoveType.Range)
                    {
                        Vector2 moveVelocity = (currentMove.Stats.SqTo - currentMove.Stats.SqFrom) / (currentMove.Stats.End - currentMove.Stats.Start);
                        if (!model.faceRight) moveVelocity *= new Vector2(-1, 1);
                        currentMove.Box.BoundBox.LinearVelocity = moveVelocity + view.Velocity;
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
                            currentMove.chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (startAfterMinCharge && currentMove.chargeTime > ((ChargeMove)currentMove.Stats).MinWait)
                            {
                                currentMove.moveStarted = true;
                                model.setState(CharacterState.attacking, currentMove.Stats);
                            }
                            break;
                        case CharacterState.chargingSuper:
                            currentMove.chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (startAfterMinCharge && currentMove.chargeTime > ((ChargeMove)currentMove.Stats).MinWait || currentMove.chargeTime > ((ChargeMove)currentMove.Stats).MaxWait)
                            {
                                currentMove.moveStarted = true;
                                model.setState(CharacterState.attacking);
                            }
                            break;
                        case CharacterState.attacking:
                            currentMove.attackTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                            if (currentMove.attackTimeLeft <= 0)
                            {
                                if (currentMove.Stats.Type != MoveType.Range)
                                {
                                    moves.EndMove(currentMove);
                                    moves.RemoveMove(currentMove);
                                }
                                model.attackMode = false;
                                NaturalState();
                            }
                            else if (currentMove.attackTimeLeft <= Math.Abs(currentMove.Stats.Start - currentMove.Stats.Duration))
                            {
                                moves.StartMove(view.Position, view.Velocity, currentMove);
                                if (currentMove.Stats.Adjustable)
                                {
                                    float angle = (float)Math.Atan(currentMove.Box.BoundBox.LinearVelocity.Y / currentMove.Box.BoundBox.LinearVelocity.X);
                                    angle += model.faceRight ?
                                        navigation.Y * ((AdjustableMove)currentMove.Stats).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000 :
                                        -navigation.Y * ((AdjustableMove)currentMove.Stats).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000;
                                    currentMove.Box.BoundBox.LinearVelocity = new Vector2(angle) * currentMove.Box.BoundBox.LinearVelocity.Length();
                                }
                            }
                            else if (currentMove.attackTimeLeft <= Math.Abs(currentMove.Stats.End - currentMove.Stats.Duration))
                            {
                                moves.EndMove(currentMove);
                            }
                            break;
                        case CharacterState.shielding:
                            break;
                        case CharacterState.takingHit:
                            if (view.VelocityY >= 0)
                            {
                                if (currentMove.Stats.Type != MoveType.Range)
                                {
                                    moves.EndMove(currentMove);
                                    moves.RemoveMove(currentMove);
                                }
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
            model.resetTimeLeft = 7000;
            model.invounerableTimeLeft = 3000;
            model.invounerable = true;
            if (currentMove != null)
            {
                moves.EndMove(currentMove);
                moves.RemoveMove(currentMove);
            }
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
            if (!model.attackMode && model.resetTimeLeft <= 0)
            {
                if (directionX !=0 && directionX * view.VelocityX >= 0) model.faceRight = directionX > 0;
                
                if (directionY < 0 && newDirection && model.jumpsLeft > 1 && view.VelocityY > -model.JumpStartVelocity + 4)
                {
                    model.setState(CharacterState.jumping);
                    view.VelocityY = -model.JumpStartVelocity;
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
            if (!model.attackMode)
            {
                MoveStats moveStats;
                if (newDirection && (Math.Abs(navigation.X) > 0.9 || Math.Abs(navigation.Y) > 0.9))
                {
                    if (Math.Abs(navigation.X) >= Math.Abs(navigation.Y) || !model.inAir && navigation.Y > 0) moveStats = stats.aLR;
                    else moveStats = model.inAir && navigation.Y > 0 ? stats.aDown : stats.aUp;
                }
                else moveStats = stats.a;

                if (moveStats != null)
                {
                    currentMove = moves.newMove(moveStats, model.faceRight);
                    if (currentMove.Stats.Type == MoveType.Charge)
                    {
                        model.setState(CharacterState.chargingHit, currentMove.Stats);
                    }
                    else
                    {
                        model.setState(CharacterState.attacking, currentMove.Stats);
                        if (currentMove.Stats.Start == 0) moves.StartMove(view.Position, view.Velocity, currentMove);
                    } 
                }
            }
        }

        private void SuperKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {
            if (!model.attackMode)
            {
                MoveStats moveStats;
                if (navigation.X == 0 && navigation.Y == 0)
                {
                    moveStats = Math.Abs(view.VelocityX) < 3 ? stats.x : stats.xLR;
                }
                else if (Math.Abs(navigation.X) > Math.Abs(navigation.Y)) moveStats = stats.xLR;
                else if (navigation.Y > 0) moveStats = stats.xDown;
                else if (model.jumpsLeft > 0)
                {
                    moveStats = stats.xUp;
                    model.jumpsLeft = 0;
                }
                else return;

                Debug.WriteLine("new move: " + currentMove.Stats.Type);
                if (moveStats != null)
                {
                    currentMove = moves.newMove(moveStats, model.faceRight);
                    if (moveStats.Type == MoveType.Charge)
                    {
                        model.setState(CharacterState.chargingHit);
                    }
                    else
                    {
                        model.setState(CharacterState.attacking);
                        if (moveStats.Start == 0) moves.StartMove(view.Position, view.Velocity, currentMove);
                    } 
                }
            }
        }

        private void HitKeyUp(float downTimer, int playerIndex)
        {
            if (model.state == CharacterState.chargingHit)
            {
                if (currentMove.chargeTime > ((ChargeMove)currentMove.Stats).MinWait)
                {
                    moves.StartMove(view.Position, view.Velocity, currentMove);
                    model.setState(CharacterState.attacking, currentMove.Stats);
                    currentMove.moveStarted = true;
                }
                else startAfterMinCharge = true;
            }
        }

        private void SuperKeyUp(float downTimer, int playerIndex)
        {
            if (model.state == CharacterState.chargingSuper)
            {
                if (currentMove.chargeTime > ((ChargeMove)currentMove.Stats).MinWait) moves.StartMove(view.Position, view.Velocity, currentMove);
                else startAfterMinCharge = true;
            }
        }

        public void AddPowerUp(PowerUp powerUp)
        {
            model.maxSpeed += powerUp.maxSpeed;
            model.acceleration += powerUp.acceleration;
            model.weight += powerUp.weight;
            model.JumpStartVelocity += powerUp.jumpStartVelocity;
            //model.powerUp += powerUp.power;

            view.Blinking = true;
        }

        public void RemovePowerUp(PowerUp powerUp)
        {
            model.maxSpeed -= powerUp.maxSpeed;
            model.acceleration -= powerUp.acceleration;
            model.weight -= powerUp.weight;
            model.JumpStartVelocity -= powerUp.jumpStartVelocity;
            //model.powerUp -= powerUp.power;

            view.Blinking = false;
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
                MoveModel move = (MoveModel)obj.Body.UserData;
                if (!move.PlayerIndexes.Contains(model.playerIndex) && !model.invounerable)
                {
                    move.PlayerIndexes.Add(model.playerIndex);

                    MoveStats stats = move.Stats;
                    float ratio = 0;
                    if (stats.Type == MoveType.Charge && move.chargeTime < ((ChargeMove)stats).MaxWait)
                        ratio = move.chargeTime / ((ChargeMove)stats).MaxWait;
                    else ratio = 1;
                    Vector2 power = ratio * stats.Power;
                    int damage = (int)ratio * stats.Damage;

                    view.Velocity = move.Xdirection * power * (1 + model.damagePoints / 100) * (100 / model.weight);
                    if (Math.Abs(view.VelocityY) == 0) view.VelocityY = -1;
                    model.damagePoints += damage;
                    model.setState(CharacterState.takingHit);

                    if (OnHit != null) OnHit.Invoke(ConvertUnits.ToDisplayUnits(obj.Body.Position), damage, model.damagePoints, move.PlayerIndexes.First(), model.playerIndex, stats.hitSound);

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
