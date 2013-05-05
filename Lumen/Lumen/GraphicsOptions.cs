using Microsoft.Xna.Framework;

namespace Lumen
{
    internal sealed class GraphicsOptions
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