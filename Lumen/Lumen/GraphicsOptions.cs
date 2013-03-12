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
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = true;

            graphics.ApplyChanges();
        }
    }
}
