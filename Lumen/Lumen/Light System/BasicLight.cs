using Microsoft.Xna.Framework;

namespace Lumen.Light_System
{
    internal class BasicLight : ILightProvider
    {
        #region ILightProvider Members

        public Vector2 Position { get; set; }
        public Color LightColor { get; set; }
        public float LightRadius { get; set; }
        public float LightIntensity { get; set; }
        public bool IsVisible { get; set; }

        #endregion
    }
}