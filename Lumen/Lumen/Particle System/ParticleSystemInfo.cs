using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    class ParticleSystemInfo
    {
        public Texture2D Texture;
        public Rectangle TextureRect;
        public Vector2 TextureOrigin;
        public float FiringDuration;
        public float NumberOfParticlesPerSecond;

        public float ParticleLifetimeMin, ParticleLifetimeMax;

        public float ParticleAngle, ParticleAngleSpread;

        public float ParticleVelocityMin, ParticleVelocityMax;

        public float ParticleScaleMin, ParticleScaleMax;

        public Color ParticleColorStart, ParticleColorEnd;
        public float ParticleColorVariation;
    }
}
