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
            move = controller.move.move;
        }
    }

    public class CharacterController : Controller
    {
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
        public MoveModel move;

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



        public CharacterController(ScreenManager screen, GamepadController pad, Vector2 startPos) 
            : base(screen)
        {
            model = new CharacterModel(pad, startPos);
            move = new MoveModel();
            model.stats = pad.SelectedCharacter;
            this.pad = pad;
        }

        public void Reset(Vector2 startPos, bool behindMap)
        {
            RemoveView(view);
            view.BoundBox.IsStatic = true;
            view.Position = startPos;
            model.resetTimeLeft = 4000;
            model.invounerableTimeLeft = 3000;
            model.invounerable = true;
            if(move.moveBox != null) move.moveBox.Dispose();
        }

        public override void Load(ContentManager content)
        {
            view = new CharacterView(content, model.stats.animations, 200, 200, 200, 200);
            model.view = view;
            view.Scale = 0.6f;
            view.BoundRect(World, model.stats.size.X, model.stats.size.Y);
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

            Screen.soundController.LoadCharacter(content, this);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
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
                if (!model.attackMode && (Math.Abs(view.VelocityX) < Math.Abs(model.stats.maxSpeed * navigation.X) || view.VelocityX * navigation.X < 0))
                    velocityPlus.X += navigation.X * model.stats.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                if (!model.attackMode || model.state == CharacterState.takingHit || move.move.Type != MoveType.Body 
                    || Math.Abs(((BodyMove)move.move).BodyEnd - move.move.Duration) > move.attackTimeLeft || Math.Abs(((BodyMove)move.move).BodyStart - move.move.Duration) < move.attackTimeLeft)
                {
                    if (model.inAir)
                        velocityPlus.Y += model.stats.gravity * gameTime.ElapsedGameTime.Milliseconds / 1000f;
                    else if (view.VelocityX * navigation.X < 0 || navigation.X == 0)
                        velocityPlus.X += -(view.VelocityX / model.stats.maxSpeed) * 3 * model.stats.acceleration * gameTime.ElapsedGameTime.Milliseconds / 1000; 
                }

                view.Velocity += velocityPlus;
                if (model.state == CharacterState.attacking && move.move.Type != MoveType.Range) move.moveBox.LinearVelocity += velocityPlus;

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
                        if (move.startMoveWhenReady && move.chargeTime > ((ChargeMove)move.move).MinWait) model.setState(CharacterState.attacking, move.move);
                        break;
                    case CharacterState.chargingSuper:
                        move.chargeTime += gameTime.ElapsedGameTime.Milliseconds;
                        if (move.startMoveWhenReady && move.chargeTime > ((ChargeMove)move.move).MinWait || move.chargeTime > ((ChargeMove)move.move).MaxWait) model.setState(CharacterState.attacking);
                        break;
                    case CharacterState.attacking:
                        move.attackTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                        if (move.attackTimeLeft <= 0)
                        {
                            model.attackMode = false;
                            NaturalState();
                        }
                        else if (Math.Abs(move.move.Start - move.move.Duration) <= move.attackTimeLeft)
                        {
                            if (!move.moveStarted) StartMove();
                            if (move.move.Adjustable)
                            {
                                float angle = move.moveBox.LinearVelocity.X == 0 ? 0 : (float)Math.Atan(move.moveBox.LinearVelocity.Y / move.moveBox.LinearVelocity.X);
                                angle += model.faceRight ?
                                    navigation.Y * ((AdjustableMove)move.move).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000 :
                                    -navigation.Y * ((AdjustableMove)move.move).AdjustAcc * gameTime.ElapsedGameTime.Milliseconds / 1000;
                                move.moveBox.LinearVelocity = new Vector2(angle) * move.moveBox.LinearVelocity.Length();
                            }
                        }
                        else if (Math.Abs(move.move.End - move.move.Duration) <= move.attackTimeLeft)
                        {
                            move.moveBox.Dispose();
                        }
                        break;
                    case CharacterState.shielding:
                        break;
                    case CharacterState.takingHit:
                        if (view.VelocityY >= 0)
                        {
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
            if (!model.attackMode)
            {
                if (newDirection && (Math.Abs(navigation.X) > 0.9 || Math.Abs(navigation.Y) > 0.9))
                {
                    if (Math.Abs(navigation.X) >= Math.Abs(navigation.Y) || !model.inAir && navigation.Y > 0) move.move = model.stats.aLR;
                    else move.move = model.inAir && navigation.Y > 0 ? model.stats.aDown : model.stats.aUp;
                }
                else
                {
                    move.move = model.stats.a;
                }

                if (move.move != null)
                {
                    if (move.move.Type == MoveType.Charge)
                    {
                        move.startMoveWhenReady = false;
                        model.setState(CharacterState.chargingHit);
                    }
                    else
                    {
                        model.setState(CharacterState.attacking, move.move);
                        if (move.move.Start == 0) StartMove();
                    } 
                }
            }
        }

        private void SuperKeyDown(float directionX, float directionY, float downTimer, int playerIndex)
        {
            if (!model.attackMode)
            {
                if (navigation.X == 0 && navigation.Y == 0)
                {
                    move.move = Math.Abs(view.VelocityX) < 3 ? move.move = model.stats.x : model.stats.xLR;
                }
                else if (Math.Abs(navigation.X) > Math.Abs(navigation.Y)) move.move = model.stats.xLR;
                else if (navigation.Y > 0) move.move = model.stats.xDown;
                else if (model.jumpsLeft > 0)
                {
                    move.move = model.stats.xUp;
                    model.jumpsLeft = 0;
                }
                else return;

                if (move.move != null)
                {
                    if (move.move.Type == MoveType.Charge)
                    {
                        move.startMoveWhenReady = false;
                        model.setState(CharacterState.chargingHit);
                    }
                    else
                    {
                        model.setState(CharacterState.attacking);
                        if (move.move.Start == 0) StartMove();
                    } 
                }
            }
        }

        private void HitKeyUp(float downTimer, int playerIndex)
        {
            if (model.state == CharacterState.chargingHit)
            {
                if (move.chargeTime > ((ChargeMove)move.move).MinWait) StartMove();
                else move.startMoveWhenReady = true;
            }
        }

        private void SuperKeyUp(float downTimer, int playerIndex)
        {
            if (model.state == CharacterState.chargingSuper)
            {
                if (move.chargeTime > ((ChargeMove)move.move).MinWait) StartMove();
                else move.startMoveWhenReady = true;
            }
        }

        private void StartMove()
        {
            move.moveBox = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(move.move.SqSize.X), ConvertUnits.ToSimUnits(move.move.SqSize.Y), 0,
                view.BoundBox.Position + move.move.SqFrom, new MoveInfo(this));
            move.moveBox.IgnoreGravity = true;
            move.moveBox.IsStatic = false;
            move.moveBox.CollidesWith = Category.Cat11;
            move.moveBox.CollisionCategories = Category.Cat20;
            
            Vector2 speed = (move.move.SqTo - move.move.SqFrom) / (move.move.End - move.move.Start);
            move.moveBox.LinearVelocity = model.faceRight ? (speed + view.Velocity) * new Vector2(1, 1) : (speed + view.Velocity) * new Vector2(-1, 1);
            move.moveBox.UserData = new MoveInfo(this);
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

                    //if (move.Adjustable && ((AdjustableMove)move).StopAtHit) move.attackTimeLeft = 0; Denne skal på moveboxens oncollision...
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
