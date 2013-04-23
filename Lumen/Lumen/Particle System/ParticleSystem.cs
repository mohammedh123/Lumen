using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    class ParticleSystem
    {
        private class SpawningInfo
        {
            public float Timer=0.0f;
            public float Duration=0.0f;
            public Vector2 Position;
        }

        public const int MaxParticles = 512;

        private Vector2 _spawningPosition = Vector2.Zero;
        private readonly ParticleSystemInfo _systemInfo;
        private Particle[] _particles = new Particle[MaxParticles];
        private int _freeParticleIndex = 0, _numParticlesAlive = 0;
        private readonly float _spawningInterval;
        private List<SpawningInfo> _spawnInfos = new List<SpawningInfo>();
        
        public ParticleSystem(ParticleSystemInfo psInfo)
        {
            _systemInfo = psInfo;
            _spawningInterval = 1.0f/_systemInfo.NumberOfParticlesPerSecond;
        }

        private bool GetNextParticleIndex()
        {
            var count = 0;

            if(_numParticlesAlive == MaxParticles) return false;

            while(count < MaxParticles && _particles[_freeParticleIndex].Lifetime <= 0) {
                _freeParticleIndex++;
                count++;

                if (_freeParticleIndex >= MaxParticles)
                    _freeParticleIndex -= MaxParticles;
            }

            return count < MaxParticles;
        }

        public void Update(float dt, Vector2 bounds)
        {
            for (int i = 0; i < _spawnInfos.Count; i++) {
                var si = _spawnInfos[i];
                si.Timer += dt;

                if (si.Timer >= _spawningInterval) {
                    si.Timer = 0.0f;
                    
                    //spawn particle
                    if(GetNextParticleIndex()) {
                        //theres still room in the pool of particles
                        _particles[_freeParticleIndex].Angle = _systemInfo.ParticleAngle +
                                                               (float)
                                                               (GameDriver.RandomGen.NextDouble()*
                                                                _systemInfo.ParticleAngleSpread*0.5f);
                        _particles[_freeParticleIndex].Color = Color.Lerp(_systemInfo.ParticleColorStart, _systemInfo.ParticleColorEnd, (float)(GameDriver.RandomGen.NextDouble() * _systemInfo.ParticleColorVariation));
                        _particles[_freeParticleIndex].Lifetime = _particles[_freeParticleIndex].InitialLifetime = _systemInfo.ParticleLifetimeMin +
                                                                  (float)(GameDriver.RandomGen.NextDouble() *
                                                                           (_systemInfo.ParticleLifetimeMax-_systemInfo.ParticleLifetimeMin));
                        _particles[_freeParticleIndex].Position = si.Position;
                        _particles[_freeParticleIndex].Scale = _systemInfo.ParticleScaleMin +
                                                              (float)
                                                              (GameDriver.RandomGen.NextDouble()*
                                                               (_systemInfo.ParticleScaleMax -
                                                                _systemInfo.ParticleScaleMin));
                        _particles[_freeParticleIndex].Velocity = _systemInfo.ParticleVelocityMin +
                                                              (float)
                                                              (GameDriver.RandomGen.NextDouble() *
                                                               (_systemInfo.ParticleVelocityMax -
                                                                _systemInfo.ParticleVelocityMin));
                        _particles[_freeParticleIndex].Alpha = 1.0f;

                        _numParticlesAlive++;
                    }
                }

                si.Duration += dt;

                if(si.Duration >= _systemInfo.FiringDuration) {
                    _spawnInfos.RemoveAt(i);
                    i--;
                }
            }
            
            var screenBounds = new Rectangle(0, 0, (int) bounds.X, (int) bounds.Y);

            for (int i = 0; i < _particles.Length; i++) {
                var particle = _particles[i];
                if (particle.Lifetime <= 0.0f) {
                    continue;
                }

                particle.Lifetime -= dt;

                if (particle.Lifetime <= 0.0f) {
                    _numParticlesAlive--;
                }
                else {
                    particle.Position +=
                        new Vector2((float) Math.Cos(particle.Angle), (float) Math.Sin(particle.Angle))*
                        particle.Velocity*dt;
                    particle.Alpha = MathHelper.Lerp(1.0f, 0.0f, particle.Lifetime/particle.InitialLifetime);

                    //bounds check
                    if (screenBounds.Contains((int) particle.Position.X, (int) particle.Position.Y)) {
                        KillParticle(i);
                    }
                }
            }
        }

        private void KillParticle(int idx)
        {
            //unsafe for speed
            _numParticlesAlive--;
            _particles[idx].Lifetime = -1.0f;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(var p in _particles) {
                sb.Draw(_systemInfo.Texture, p.Position, _systemInfo.TextureRect,p.Color,p.Angle,_systemInfo.TextureOrigin,p.Scale,SpriteEffects.None,0);
            }
        }

        public void StartSpawn(float x, float y)
        {
            _spawnInfos.Add(new SpawningInfo
                            {
                                Duration = _systemInfo.FiringDuration,
                                Position = new Vector2(x, y),
                                Timer = 0.0f
                            });
        }
    }
}
