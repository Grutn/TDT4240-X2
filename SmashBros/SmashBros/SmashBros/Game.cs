using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SmashBros.MySystem;
using SmashBros.Controllers;
using System.Diagnostics;

namespace SmashBros {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    ///Test
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        #endregion

        #region Initialization

        public Game() {
            
            Serializing.GenereateModels();

            Content.RootDirectory = "Content";
            IsMouseVisible = !true;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Constants.WindowWidth;
            graphics.PreferredBackBufferHeight = Constants.WindowHeight;
            graphics.IsFullScreen = Constants.FullScreen;

            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            Components.Add(new ScreenManager(this));
        }


        #endregion
    }
}
