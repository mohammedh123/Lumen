using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    internal class ParticleSystem
    {
        public const int MaxParticles = 512;

        private readonly Particle[] _particles = new Particle[MaxParticles];
        private readonly List<SpawningInfo> _spawnInfos = new List<SpawningInfo>();
        private readonly float _spawningInterval;
        private readonly ParticleSystemInfo _systemInfo;

        public Vector2 Position = Vector2.Zero;
        private int _freeParticleIndex, _numParticlesAlive;
        private Vector2 _spawningPosition = Vector2.Zero;

        public ParticleSystem(ParticleSystemInfo psInfo)
        {
            _systemInfo = psInfo;
            _spawningInterval = 1.0f/_systemInfo.NumberOfParticlesPerSecond;

            for (int i = 0; i < MaxParticles; i++) {
                _particles[i] = new Particle();
            }
        }

        private bool GetNextParticleIndex()
        {
            int count = 0;

            if (_numParticlesAlive == MaxParticles) {
                return false;
            }

            while (count < MaxParticles && _particles[_freeParticleIndex].Lifetime >= 0) {
                _freeParticleIndex++;
                count++;

                if (_freeParticleIndex >= MaxParticles) {
                    _freeParticleIndex -= MaxParticles;
                }
            }

            return count < MaxParticles;
        }

        public void Update(float dt, Vector2 bounds)
        {
            for (int i = 0; i < _spawnInfos.Count; i++) {
                SpawningInfo si = _spawnInfos[i];
                si.Timer += dt;

                while (si.Timer >= _spawningInterval) {
                    si.Timer -= _spawningInterval;

                    //spawn particle
                    if (GetNextParticleIndex()) {
                        //theres still room in the pool of particles
                        _particles[_freeParticleIndex].Angle = _systemInfo.ParticleAngle -
                                                               _systemInfo.ParticleAngleSpread*0.5f +
                                                               (float)
                                                               (GameDriver.RandomGen.NextDouble()*
                                                                _systemInfo.ParticleAngleSpread);
                        _particles[_freeParticleIndex].AngularVelocity = _systemInfo.ParticleAngularVelocityMin +
                                                                         (float)
                                                                         (GameDriver.RandomGen.NextDouble()*
                                                                          (_systemInfo.ParticleAngularVelocityMax -
                                                                           _systemInfo.ParticleAngularVelocityMin));
                        _particles[_freeParticleIndex].Color = Color.Lerp(_systemInfo.ParticleColorStart,
                                                                          _systemInfo.ParticleColorEnd,
                                                                          (float)
                                                                          (GameDriver.RandomGen.NextDouble()*
                                                                           _systemInfo.ParticleColorVariation));
                        _particles[_freeParticleIndex].Lifetime =
                            _particles[_freeParticleIndex].InitialLifetime = _systemInfo.ParticleLifetimeMin +
                                                                             (float) (GameDriver.RandomGen.NextDouble()*
                                                                                      (_systemInfo.ParticleLifetimeMax -
                                                                                       _systemInfo.ParticleLifetimeMin));
                        _particles[_freeParticleIndex].Position = si.Position;
                        _particles[_freeParticleIndex].Scale = _systemInfo.ParticleScaleMin +
                                                               (float)
                                                               (GameDriver.RandomGen.NextDouble()*
                                                                (_systemInfo.ParticleScaleMax -
                                                                 _systemInfo.ParticleScaleMin));
                        _particles[_freeParticleIndex].Velocity = _systemInfo.ParticleVelocityMin +
                                                                  (float)
                                                                  (GameDriver.RandomGen.NextDouble()*
                                                                   (_systemInfo.ParticleVelocityMax -
                                                                    _systemInfo.ParticleVelocityMin));
                        _particles[_freeParticleIndex].Alpha = 1.0f;

                        _numParticlesAlive++;
                    }
                }

                si.Duration -= dt;

                if (si.Duration <= 0) {
                    _spawnInfos.RemoveAt(i);
                    i--;
                }
            }

            var screenBounds = new Rectangle(0, 0, (int) bounds.X, (int) bounds.Y);

            for (int i = 0; i < _particles.Length; i++) {
                Particle particle = _particles[i];
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
                    particle.Angle += particle.AngularVelocity*dt;
                    particle.Alpha = MathHelper.Lerp(1.0f, 0.0f, 1 - particle.Lifetime/particle.InitialLifetime);

                    //bounds check
                    if (!screenBounds.Contains((int) particle.Position.X, (int) particle.Position.Y)) {
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
            foreach (Particle p in _particles) {
                if (p.Lifetime <= 0.0f) {
                    continue;
                }

                sb.Draw(_systemInfo.Texture, p.Position, _systemInfo.TextureRect, p.Color*p.Alpha, p.Angle,
                        _systemInfo.TextureOrigin, p.Scale, SpriteEffects.None, 0);
            }
        }

        public void StartSpawn(float x = 0, float y = 0)
        {
            _spawnInfos.Add(new SpawningInfo
                            {
                                Duration = _systemInfo.FiringDuration,
                                Position = Position + new Vector2(x, y),
                                Timer = 0.0f
                            });
        }

        public void MoveTo(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        #region Nested type: SpawningInfo

        private class SpawningInfo
        {
            public float Duration;
            public Vector2 Position;
            public float Timer;
        }

        #endregion
    }
}