using Microsoft.Xna.Framework;

namespace Lumen.Light_System
{
    internal interface ILightProvider
    {
        Vector2 Position { get; set; }

        Color LightColor { get; set; }

        float LightRadius { get; set; }

        float LightIntensity { get; set; }

        bool IsVisible { get; set; }
    }
}