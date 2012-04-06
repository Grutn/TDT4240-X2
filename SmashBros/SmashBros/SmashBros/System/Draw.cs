using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.System
{
    class Draw
    {
        public static Texture2D ColoredRectangle(GraphicsDevice graphic, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphic, width, height, true, SurfaceFormat.Color);
            Color[] colorArr = new Color[width * height];
            for (int i = 0; i < colorArr.Length; i++)
            {
                colorArr[i] = color;
            }

            texture.SetData(colorArr);
            return texture;
        }

    }
}
