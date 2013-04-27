using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen.Light_System
{
    interface ILightProvider
    {
        Vector2 Position { get; set; }
        
        Color LightColor { get; set; }

        float LightRadius { get; set; }

        float LightIntensity { get; set; }

        bool IsVisible { get; set; }
    }
}
