using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    internal class ParticleSystemManager
    {
        #region Singleton Data

        private static ParticleSystemManager _instance;

        private ParticleSystemManager()
        {
        }

        private ParticleSystemManager(ParticleSystemManager psm)
        {
        }

        public static ParticleSystemManager Instance
        {
            get { return _instance ?? (_instance = new ParticleSystemManager()); }
        }

        #endregion

        private readonly Dictionary<string, ParticleSystem> _particleSystems = new Dictionary<string, ParticleSystem>();

        public void Update(float dt, Vector2 bounds)
        {
            foreach (var kvp in _particleSystems) {
                kvp.Value.Update(dt, bounds);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var kvp in _particleSystems) {
                kvp.Value.Draw(sb);
            }
        }

        public void FireParticleSystem(string key, float x, float y)
        {
            if (_particleSystems.ContainsKey(key)) {
                _particleSystems[key].StartSpawn(x, y);
            }
        }

        public void RemoveParticleSystem(string key)
        {
            _particleSystems.Remove(key);
        }

        public void RemoveAll()
        {
            _particleSystems.Clear();
        }

        public void StopFiringAllSystems()
        {
            foreach(var kvp in _particleSystems)
                kvp.Value.StopFiringEntirely();
        }

        public void KillAllParticles()
        {
            foreach (var kvp in _particleSystems)
                kvp.Value.KillAllParticles();
        }

        public void RegisterParticleSystem(string key, ParticleSystem ps)
        {
            _particleSystems.Add(key, ps);
        }
    }
}