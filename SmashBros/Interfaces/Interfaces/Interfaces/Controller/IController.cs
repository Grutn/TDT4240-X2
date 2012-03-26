using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Interfaces.Controller
{
    public interface IController
    {
        void Load(ContentManager content);
        void Unload();
    }
}
