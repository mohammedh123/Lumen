using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Light_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    class OrbitingRing
    {
        public List<OrbitingParticle> Satellites;

        private float _radius, _particleScale, _orbitPeriod;
        private bool _isVisible = true;
        private Entity _orbitTarget;
        private string _textureKey;
        private Rectangle _textureRect;

        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                foreach (var orb in Satellites)
                    orb.DistanceFromCenter = value;
            }
        }

        public float OrbitPeriod
        {
            get { return _orbitPeriod; }
            set
            {
                _orbitPeriod = value;
                foreach (var orb in Satellites)
                    orb.OrbitPeriod = value;
            }
        }
        
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                foreach (var orb in Satellites) orb.IsVisible = value;
            }
        }

        private float AngleDifference
        {
            get { return MathHelper.TwoPi/Satellites.Count; }
        }

        public OrbitingRing(float radius, int initialCount, float scale, float orbOrbitPeriod, string textureKey, Rectangle textureRect, Entity orbitTarget)
        {
            _particleScale = scale;
            _radius = radius;
            _orbitPeriod = orbOrbitPeriod;
            _textureRect = textureRect;
            _textureKey = textureKey;
            _orbitTarget = orbitTarget;

            Satellites = new List<OrbitingParticle>();

            for (int i = 0; i < initialCount; i++)
            {
                var atu = i * (MathHelper.TwoPi / initialCount);

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
        
        public void Update(float dt)
        {
            foreach(var orb in Satellites)
                orb.Update(dt);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(var orb in Satellites)
                orb.Draw(sb);
        }

        public IEnumerable<ILightProvider> GetLights()
        {
            return Satellites;
        }
    }
}
