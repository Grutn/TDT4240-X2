using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SmashBros.MySystem;
using SmashBros.Views;
using FarseerPhysics.Dynamics;
using SmashBros.Model;
using FarseerPhysics.Dynamics.Contacts;
using SmashBros.Models;
using System.Diagnostics;

namespace SmashBros.Controllers
{
    public class CursorController : Controller
    {
        List<CursorModel> playerCursors;

        public CursorController(ScreenManager screen)
            :base(screen)
        {

        }

        public override void Load(ContentManager content)
        {
            this.playerCursors = new List<CursorModel>();
            foreach (GamepadController pad in GamePadControllers)
            {
                var c = new CursorModel(content, World, pad, OnCursorNavigate, OnCollision, OnSeparation, false);
                c.SetMinMaxPos(10, Constants.WindowWidth - 25, 10, Constants.WindowHeight - 30);
                
                pad.OnHitKeyPressed += CheckForCursorPress;
                pad.OnSuperKeyPressed += CheckForCursorDeslect;

                playerCursors.Add(c);
            }

            ResetCursors();
            SubscribeToGameState = true;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnNext(GameStateManager value)
        {
            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    EnableCursors = false;
                    break;
                case GameState.CharacterMenu:
                    EnableCursors = true;
                    break;
                case GameState.MapsMenu:
                    EnableCursors = true;
                    break;
                case GameState.GamePlay:
                    EnableCursors = false;
                    break;
                case GameState.GamePause:
                    EnableCursors = true;
                    break;
                case GameState.GameOver:
                    EnableCursors = false;
                    break;
            }

            ResetCursors();
        }

        public override void Deactivate()
        {
        }

        public void ResetCursors()
        {
            int i = 0;
            foreach (var cursor in playerCursors)
            {
                cursor.SetPosition(280 * i + 100, 680);
                i++;
            }

        }

        public bool EnableCursors
        {
            set
            {

                if (value)
                {
                    bool onlyHasSelCharacter = GameState.MapsMenu == CurrentState || GameState.GamePause == CurrentState;
                    int i = 0;
                    foreach (var cursor in playerCursors)
                    {
                        if (onlyHasSelCharacter)
                        {
                            if (GamePadControllers[i].PlayerModel.SelectedCharacter == null)
                            {
                                RemoveView(cursor.Cursor);
                                cursor.Enabled = false;
                            }
                            else
                            {
                                AddView(cursor.Cursor);
                                cursor.Enabled = true;
                            }
                        }
                        else
                        {
                            AddView(cursor.Cursor);
                            cursor.Enabled = true;
                        }

                        i++;
                    }
                }
                else
                {
                    foreach (var cursor in playerCursors)
                    {
                        cursor.Enabled = false;
                        RemoveView(cursor.Cursor);
                    }
                }

            }
        }

        public void DisableCursor(int playerIndex)
        {
            RemoveView(playerCursors[playerIndex].Cursor);
            playerCursors[playerIndex].Enabled = false;
        }

        public void EnableCursor(int playerIndex)
        {
            AddView(playerCursors[playerIndex].Cursor);
            playerCursors[playerIndex].Enabled = true;
        }

        public void SetCursorCollisionCategory(Category cat = Category.Cat5)
        {
            if (playerCursors != null)
                playerCursors.ForEach(a => a.Cursor.BoundBox.CollidesWith = cat);
        }

        private bool OnCollision(Fixture cursor, Fixture box, Contact contact)
        {
            Screen.soundController.PlayMenuSound(MenuSoundType.btnHover, null);
            int playerIndex = (int)cursor.Body.UserData;
            var cursorModel = playerCursors[playerIndex];
            cursorModel.CurrentTarget = box.Body;

            if (OnCursorCollision != null)
                OnCursorCollision.Invoke(playerIndex, box.Body.UserData, cursorModel);

            return true;
        }

        private void OnSeparation(Fixture cursor, Fixture box)
        {
            int playerIndex = (int)cursor.Body.UserData;
            var cursorModel = playerCursors[playerIndex];

            if(OnCursorSeparation != null) 
                OnCursorSeparation.Invoke(playerIndex, cursorModel.CurrentTarget == null ? null : cursorModel.CurrentTarget.UserData, cursorModel);

            cursorModel.CurrentTarget = null;
        }

        private void OnCursorNavigate(float directionX, float directionY, int playerIndex, bool newDirection)
        {
            var cursor = playerCursors[playerIndex];
            cursor.SetPosition(cursor.Cursor.PositionX + directionX * Constants.MaxCursorSpeed,
                cursor.Cursor.PositionY + directionY * Constants.MaxCursorSpeed);

        }

        private void CheckForCursorPress(int playerIndex)
        {
            CheckForPress(playerIndex, true);
        }

        private void CheckForCursorDeslect(int playerIndex)
        {
            CheckForPress(playerIndex, false);            
        }

        private void CheckForPress(int playerIndex, bool selectKey)
        {
            var c = playerCursors[playerIndex];
            if (c.CurrentTarget != null)
            {
                OnCursorClick.Invoke(playerIndex, c.CurrentTarget.UserData, c, selectKey);
            }
        }

        
        //Events of for cursor
        public CursorAction OnCursorCollision;
        public CursorAction OnCursorSeparation;
        public CursorClick OnCursorClick;
    }

    public delegate void CursorAction(int playerIndex, object targetData, CursorModel cursor);
    public delegate void CursorClick(int playerIndex, object targetData, CursorModel cursor, bool selectKey);


}
