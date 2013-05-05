using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    internal class ParticleSystemInfo
    {
        public float FiringDuration;
        public float NumberOfParticlesPerSecond;

        public float ParticleAngle, ParticleAngleSpread;

        public float ParticleAngularVelocityMax;
        public float ParticleAngularVelocityMin;

        public Color ParticleColorEnd;
        public Color ParticleColorStart;
        public float ParticleColorVariation;
        public float ParticleLifetimeMax;
        public float ParticleLifetimeMin;
        public float ParticleScaleMax;
        public float ParticleScaleMin;
        public float ParticleVelocityMax;
        public float ParticleVelocityMin;
        public Texture2D Texture;
        public Vector2 TextureOrigin;
        public Rectangle TextureRect;
    }
}