using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SmashBros.System;
using SmashBros.Views;

namespace SmashBros.Controllers
{
    public enum PopupState
    {
        hidden, colapsed, show
    }

    class PopupMenuController : Controller
    {
        PopupState state;
        ImageTexture bg;

        public PopupMenuController(ScreenController screen) : base(screen)
        {

        }

        public override void Load(ContentManager content)
        {
            bg = new ImageTexture(content, "Menu/PopupMenu", 175, -600);
            bg.Layer = 1000;
            AddView(bg);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }
    }
}
