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
        public ImageController Img;
        public List<MoveModel> models;
        public MoveModel currentModel;

        /// <summary>
        /// Is set to true if the character releases attackbutton before minimun chargetime has passed.
        /// </summary>
        public bool startMoveWhenReady = false;

        /// <summary>
        /// Whether the movebox have bin created etc.
        /// </summary>
        public bool moveStarted = false;

        public MoveController(ScreenManager screen, CharacterStats characterStats)
            :base(screen)
        {
            Img = new ImageController(Screen, characterStats.moveAnimations, 120, false);
            models = new List<MoveModel>();
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

        public void newMove(MoveStats stats)
        {
            currentModel = new MoveModel(stats);
            models.Add(currentModel);
        }

        public void StartMove(Vector2 characterPosition)
        {
            currentModel.Box.SetBoundBox(World, (int)currentModel.Stats.SqSize.X, (int)currentModel.Stats.SqSize.Y, currentModel.Stats.SqFrom, FarseerPhysics.Dynamics.Category.Cat11);
            currentModel.Box.BoundBox.IgnoreGravity = true;
            currentModel.Box.BoundBox.IsStatic = false;
            currentModel.Box.BoundBox.CollidesWith = Category.Cat11;
            currentModel.Box.BoundBox.CollisionCategories = Category.Cat20;

            startMoveWhenReady = false;
        }

        public void EndMove()
        {

        }
    }
}
