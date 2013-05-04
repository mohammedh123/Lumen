using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen
{
    class GraphicsOptions
    {
        public virtual void ApplySettings(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = 1152;
            graphics.PreferredBackBufferHeight = 864;
            graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }
    }
}
