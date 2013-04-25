using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen.Light_System
{
    class BasicLight : ILightProvider
    {
        public Vector2 Position { get; set; }
        public Color LightColor { get; set; }
        public float LightRadius { get; set; }
        public float LightIntensity { get; set; }
    }
}
