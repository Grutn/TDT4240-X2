using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.MySystem;
using SmashBros.Model;
using Microsoft.Xna.Framework;
using SmashBros.Models;
using FarseerPhysics.Dynamics;

namespace SmashBros.Controllers
{
    public class MoveController : Controller
    {
        private int playerIndex;
        
        private ImageController Img;
        private List<MoveModel> moves;

        public MoveController(ScreenManager screen, CharacterStats characterStats, int index)
            : base(screen)
        {
            Img = new ImageController(Screen, characterStats.moveAnimations, 120, false);
            moves = new List<MoveModel>();
            playerIndex = index;
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            AddController(Img);
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
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
            if (!move.moveStarted)
            {
                move.Box = Img.AddPosition(characterPosition);
                move.Box.SetBoundBox(World, (int)move.Stats.SqSize.X, (int)move.Stats.SqSize.Y, move.Stats.SqFrom, FarseerPhysics.Dynamics.Category.Cat11);
                move.Box.BoundBox.IgnoreGravity = true;
                move.Box.BoundBox.IsStatic = false;
                move.Box.BoundBox.CollidesWith = Category.Cat11;
                move.Box.BoundBox.CollisionCategories = Category.Cat20;

                Vector2 velocity = move.Stats.Type != MoveType.Range ?
                        (move.Stats.SqTo - move.Stats.SqFrom) / (move.Stats.End - move.Stats.Start) : ((RangeMove)move.Stats).BulletVelocity;
                velocity *= move.Xdirection;
                move.Box.BoundBox.LinearVelocity = velocity + characterVelocity;
                move.Box.BoundBox.UserData = move;

                if (move.Stats.Type == MoveType.Range)
                {
                    move.Box.StartFrame = ((RangeMove)move.Stats).AniBulletFrom;
                    move.Box.EndFrame = ((RangeMove)move.Stats).AniBulletTo;
                }
                else
                {
                    move.Box.StartFrame = 0;
                    move.Box.EndFrame = 0;
                }

                move.moveStarted = true; 
            }
        }

        public void EndMove(MoveModel move)
        {
            Img.RemovePosition(move.Box);
        }

        public void RemoveMove(MoveModel move)
        {
            moves.Remove(move);
        }

        public void Freeze()
        {
            foreach (MoveModel move in moves)
            {
                // GJØR NOE
            }
        }

        public void UnFreeze()
        {
            foreach (MoveModel move in moves)
            {
                // GJØR NOE
            }
        }
    }
}
