using System.Collections.Generic;
using Lumen.Entities;
using Lumen.Light_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    internal class OrbitingRing
    {
        private readonly float _particleScale;
        public readonly List<OrbitingParticle> Satellites;
        private bool _isVisible = true;
        private float _orbitPeriod;
        private Entity _orbitTarget;
        private float _radius;
        private string _textureKey;
        private Rectangle _textureRect;

        public OrbitingRing(float radius, int initialCount, float scale, float orbOrbitPeriod, string textureKey,
                            Rectangle textureRect, Entity orbitTarget)
        {
            _particleScale = scale;
            _radius = radius;
            _orbitPeriod = orbOrbitPeriod;
            _textureRect = textureRect;
            _textureKey = textureKey;
            _orbitTarget = orbitTarget;

            Satellites = new List<OrbitingParticle>();

            for (var i = 0; i < initialCount; i++) {
                var atu = i*(MathHelper.TwoPi/initialCount);

                Satellites.Add(new OrbitingParticle(TextureManager.GetTexture(textureKey), textureRect,
                                                    TextureManager.GetOrigin(textureKey), orbitTarget, _radius,
                                                    orbOrbitPeriod, atu)
                               {
                                   Alpha = 1.0f,
                                   Scale = _particleScale,
                                   Angle = atu,
                                   Color = Color.White
                               });
            }
        }

        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                foreach (var orb in Satellites) {
                    orb.DistanceFromCenter = value;
                }
            }
        }

        public float OrbitPeriod
        {
            set
            {
                _orbitPeriod = value;
                foreach (var orb in Satellites) {
                    orb.OrbitPeriod = value;
                }
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                foreach (var orb in Satellites) {
                    orb.IsVisible = value;
                }
            }
        }

        private float AngleDifference
        {
            get { return MathHelper.TwoPi/Satellites.Count; }
        }

        public void Update(float dt)
        {
            foreach (var orb in Satellites) {
                orb.Update(dt);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var orb in Satellites) {
                orb.Draw(sb);
            }
        }

        public IEnumerable<ILightProvider> GetLights()
        {
            return Satellites;
        }
    }
}