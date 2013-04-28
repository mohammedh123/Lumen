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

        public void SetSatelliteCount(int n)
        {
            var diff = n - Satellites.Count;
            var originalCount = Satellites.Count;
            var newAngleDiff = MathHelper.TwoPi/n;

            if (n <= 0)
                Satellites.Clear();
            else if (diff < 0) {
                Satellites.RemoveRange(Satellites.Count + diff - 1, n);
            }
            else if (diff > 0) {
                for (int i = 0; i < diff; i++)
                {
                    Satellites.Add(new OrbitingParticle(TextureManager.GetTexture(_textureKey), _textureRect,
                        TextureManager.GetOrigin(_textureKey), _orbitTarget, _radius,
                        _orbitPeriod, newAngleDiff*i)
                    {
                        Alpha = 1.0f,
                        Scale = _particleScale,
                        Angle = 0.0f,
                        Color = Color.White
                    });
                }

                for (int i = 0; i < n; i++) {
                    if (i < originalCount) {
                        if (originalCount > 0)
                            Satellites[i].Angle -= i*((MathHelper.TwoPi/originalCount) - (MathHelper.TwoPi/n));
                    }
                    else {
                        if(originalCount > 0)
                            Satellites[i].Angle = Satellites[i - 1].Angle + newAngleDiff;
                        else {
                            Satellites[i].Angle = newAngleDiff*i;
                        }
                    }
                }
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
