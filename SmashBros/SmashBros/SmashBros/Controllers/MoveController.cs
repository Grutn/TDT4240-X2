using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.MySystem;
using Microsoft.Xna.Framework;
using SmashBros.Models;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;

namespace SmashBros.Controllers
{
    public class MoveController : Controller
    {
        private int playerIndex;
        
        private ImageController Img;
        private List<MoveModel> moves;
        private List<Explotion> explotions;

        public MoveController(ScreenManager screen, CharacterStats characterStats, int index)
            : base(screen)
        {
            Img = new ImageController(Screen, characterStats.moveAnimations, 120, false);
            Img.FramesPerRow = 6;
            Img.SetFrameRectangle(100, 100);
            Img.OriginDefault = new Vector2(50, 50);
            moves = new List<MoveModel>();
            explotions = new List<Explotion>();
            playerIndex = index;
        }

        public override void Load(ContentManager content)
        {
            AddController(Img);
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (int i = 0; i < explotions.Count; i++)
            {
                Explotion explotion = explotions[i];
                explotion.TimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                if (explotion.TimeLeft <= 0)
                {
                    explotion.Img.BoundBox.Dispose();
                    explotions.Remove(explotion);
                    i--;
                }
                else
                {
                    explotion.Img.BoundBox.Dispose();
                    explotion.Img.CurrentPos -= explotion.Size * explotion.TimeLeft / explotion.Duration / 2;
                    explotion.Img.SetBoundBox(World, (int)MathHelper.Max((float)(explotion.TimeLeft / explotion.Duration * explotion.Size.X), 1), (int)MathHelper.Max((float)(explotion.TimeLeft / explotion.Duration * explotion.Size.Y), 1), Vector2.Zero, Category.Cat31, Category.Cat11, true);
                    explotion.Img.BoundBox.IgnoreGravity = true;
                    explotion.Img.BoundBox.IsStatic = true;
                    explotion.Img.BoundBox.UserData = explotion;
                    explotion.Img.BoundBox.OnCollision += ExplodeHit;
                }
            }
        }

        public override void OnNext(GameStateManager value)
        {
            if (CurrentState == GameState.GamePause)
            {

            }
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public MoveModel newMove(MoveStats stats, bool right)
        {
            MoveModel move = new MoveModel(stats, right, playerIndex);
            moves.Add(move);
            return move;
        }

        public void StartMove(Vector2 characterPosition, Vector2 characterVelocity, MoveModel move)
        {
            if (!move.Started)
            {
                move.Img = Img.AddPosition(characterPosition + move.Stats.SqFrom * move.Xdirection);
                move.Img.SetBoundBox(World, (int)move.Stats.SqSize.X, (int)move.Stats.SqSize.Y, Vector2.Zero, Category.Cat20, Category.Cat11);

                move.Img.BoundBox.IgnoreGravity = true;
                move.Img.BoundBox.IsStatic = false;
                move.Img.BoundBox.UserData = move;
                move.Started = true;

                if (move.Stats.Type == MoveType.Range)
                {
                    move.Img.BoundBox.LinearVelocity = move.Stats.BulletVelocity * move.Xdirection;
                    Img.AnimateFrame(move.Img, move.Stats.AniBulletFrom, move.Stats.AniBulletTo, 14);
                    //if (move.Xdirection == new Vector2(-1, 1)) move.Img.
                    if (move.Stats.Gravity) move.Img.BoundBox.IgnoreGravity = false;
                    move.Img.BoundBox.CollidesWith = Category.Cat11 | Category.Cat10 | Category.Cat9 | Category.Cat8 | Category.Cat7;
                    if (move.Stats.Explotion != null) 
                        move.Img.BoundBox.CollisionCategories = Category.Cat31;
                    move.Img.BoundBox.OnCollision += Collision;
                }
                else
                {
                    Vector2 velocity = ConvertUnits.ToSimUnits((move.Stats.SqTo - move.Stats.SqFrom) / (move.Stats.End - move.Stats.Start) * 1000) * move.Xdirection;
                    velocity = move.Stats.SqFrom == move.Stats.SqTo ? characterVelocity : velocity + characterVelocity;
                    move.Img.BoundBox.LinearVelocity = velocity;
                    move.Img.CurrentFrame = -1;
                } 
            }
        }

        public void EndMove(MoveModel move)
        {
            Img.RemovePosition(move.Img);
        }

        public void RemoveMove(MoveModel move)
        {
            moves.Remove(move);
        }

        public void Freeze()
        {
            foreach (MoveModel move in moves)
                if (move.Stats.Type == MoveType.Range && move.Started) move.Img.BoundBox.LinearVelocity = new Vector2(0, 0);
        }

        public void UnFreeze()
        {
            foreach (MoveModel move in moves)
                if (move.Stats.Type == MoveType.Range && move.Started) move.Img.BoundBox.LinearVelocity = move.Stats.BulletVelocity;
        }

        private bool Collision(Fixture moveFixture, Fixture geom2, Contact list)
        {
            if (geom2.CollisionCategories != Category.Cat11 || ((CharacterController)geom2.Body.UserData).model.playerIndex != playerIndex)
            {
                MoveModel move = (MoveModel)moveFixture.Body.UserData;
                if (move.Stats.Explotion != null && geom2.CollisionCategories != (Category.Cat8 | Category.Cat7))
                    Explode(move.Stats.Explotion, ConvertUnits.ToDisplayUnits(moveFixture.Body.Position));
                Img.RemovePosition(move.Img);
                move.Ended = true;
                moves.Remove(move); 
            }
            return false;
        }

        private bool ExplodeHit(Fixture explotionFixture, Fixture characterFixture, Contact list)
        {
            CharacterController character = (CharacterController)characterFixture.Body.UserData;
            character.model.damagePoints += 5;
            Explotion explotion = (Explotion) explotionFixture.Body.UserData;
            Vector2 pos = explotion.Img.CurrentPos + explotion.Img.Origin;
            Vector2 velocityPlus = character.view.Position - pos;
            velocityPlus.Normalize();
            velocityPlus *= new Vector2(5,5);
            character.view.Velocity += velocityPlus;
            character.HitByExplotion(explotion.PlayerIndex, pos);
            return false;
        }

        private void Explode(Explotion explotion, Vector2 pos)
        {
            explotion.PlayerIndex = playerIndex;
            explotion.Img = Img.AddPosition(pos);
            explotion.Img.SetBoundBox(World, 10, 10, Vector2.Zero, Category.Cat20, FarseerPhysics.Dynamics.Category.Cat11);
            //explotion.Img.CurrentFrame = -1;
            explotion.Img.BoundBox.IgnoreGravity = true;
            explotion.Img.BoundBox.IsStatic = true;
            explotion.Img.BoundBox.UserData = explotion;
            explotion.Img.BoundBox.OnCollision += ExplodeHit;
            explotions.Add(explotion);
        }
    }
}
