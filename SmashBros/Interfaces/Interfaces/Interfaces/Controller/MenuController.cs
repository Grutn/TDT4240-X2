using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces.Model;
using Interfaces.Controller;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Interfaces
{
    public class MenuController : IController
    {
        MenuModel model;

        public MenuController()
        {
        }

        public void Load(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
