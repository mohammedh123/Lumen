using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen
{
    sealed class GraphicsOptions
    {
        public static void ApplySettings(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = 1152;
            graphics.PreferredBackBufferHeight = 864;
            graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }
    }
}
