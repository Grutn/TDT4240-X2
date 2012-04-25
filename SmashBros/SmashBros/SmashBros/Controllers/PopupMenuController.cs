using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.System;
using SmashBros.Views;
using Microsoft.Xna.Framework.Input;

namespace SmashBros.Controllers
{
    public enum PopupState
    {
        hidden, colapsed, show, removed
    }

    class PopupMenuController : Controller
    {
        ImageTexture bg;

        private PopupState state;
        public PopupState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    this.state = value;
                    UpdateBgPos();
                }
            }
        }

        public PopupMenuController(ScreenController screen) : base(screen)
        {

        }

        public override void Load(ContentManager content)
        {
            bg = new ImageTexture(content, "Menu/PopupMenu", 175, -700);
            //Updates bg pos because the state may have been changed before the load was run
            UpdateBgPos();
            bg.Layer = 1000;
            AddView(bg);

            SubscribeToGameState = true;
        }


        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            switch (State)
            {
                case PopupState.hidden:
                    break;
                case PopupState.colapsed:
                    break;
                case PopupState.show:
                    break;
                case PopupState.removed:
                    break;
                default:
                    break;
            }
        }

        public override void OnNext(GameStateManager value)
        {
            switch (value.PreviousState)
            {
                case GameState.StartScreen:
                    break;
                case GameState.SelectionMenu:
                    break;
                case GameState.OptionsMenu:
                    break;
                case GameState.GamePlay:
                    break;
                default:
                    break;
            }

            switch (value.CurrentState)
            {
                case GameState.StartScreen:
                    State = PopupState.hidden;
                    break;
                case GameState.SelectionMenu:
                    State = PopupState.colapsed;
                    break;
                case GameState.OptionsMenu:
                    break;
                case GameState.GamePlay:
                    State = PopupState.removed;
                    break;
                default:
                    break;
            }
        }

        public override void Deactivate()
        {
        }

        private void UpdateBgPos()
        {
            if (bg != null)
            {

                int y = 0;
                switch (state)
                {
                    case PopupState.hidden:
                        y = -700;
                        break;
                    case PopupState.colapsed:
                        y = -610;
                        break;
                    case PopupState.show:
                        y = -40;
                        break;
                    case PopupState.removed:
                        RemoveView(bg);
                        break;
                    default:
                        break;
                }

                if (state != PopupState.removed)
                {
                    if(!bg.IsActive)
                        AddView(bg);


                    bg.Animate(0, 
                        screen.controllerViewManager.camera.Position+ 
                        new Vector2(175,y) - 
                        new Vector2(Constants.WindowWidth/2, Constants.WindowHeight/2), 
                        300);

                }
            }
        }
    }
}
