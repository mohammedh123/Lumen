using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen.Particle_System
{
    internal class Particle
    {
        public Vector2 Position { get; set; }
        public float Velocity { get; set; }
        public Color Color { get; set; }
        public float Angle { get; set; }
        public float Lifetime { get; set; }
        public float InitialLifetime { get; set; }
        public float Scale { get; set; }

        public float Alpha { get; set; }
    }
}
