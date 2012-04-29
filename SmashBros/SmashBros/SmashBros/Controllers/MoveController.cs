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
        public MoveModel currentMove;

        public MoveController(ScreenManager screen, CharacterStats characterStats, int index)
            :base(screen)
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
            /*
            foreach (MoveModel move in moves)
            {
            }
            */
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
            moves.Add(currentMove);
            return new MoveModel(stats, right, playerIndex);
        }

        public void StartMove(Vector2 characterPosition, Vector2 characterVelocity, MoveModel move)
        {
            if (!move.moveStarted)
            {
                move.Box.SetBoundBox(World, (int)currentMove.Stats.SqSize.X, (int)currentMove.Stats.SqSize.Y, currentMove.Stats.SqFrom, FarseerPhysics.Dynamics.Category.Cat11);
                move.Box.BoundBox.IgnoreGravity = true;
                move.Box.BoundBox.IsStatic = false;
                move.Box.BoundBox.CollidesWith = Category.Cat11;
                move.Box.BoundBox.CollisionCategories = Category.Cat20;

                Vector2 velocity = currentMove.Stats.Type != MoveType.Range ?
                        (currentMove.Stats.SqTo - currentMove.Stats.SqFrom) / (currentMove.Stats.End - currentMove.Stats.Start) : ((RangeMove)currentMove.Stats).BulletVelocity;
                velocity *= currentMove.Xdirection;
                move.Box.BoundBox.LinearVelocity = velocity + characterVelocity;
                move.Box.BoundBox.UserData = currentMove;

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
    }
}
