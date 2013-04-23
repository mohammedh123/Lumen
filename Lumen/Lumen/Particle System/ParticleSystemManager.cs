using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    class ParticleSystemManager
    {
        #region Singleton Data

        private ParticleSystemManager()
        {
        }

        private ParticleSystemManager(ParticleSystemManager psm)
        {
        }

        private ParticleSystemManager _instance = null;

        public ParticleSystemManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ParticleSystemManager();

                return _instance;
            }
        }

        #endregion


        private readonly List<ParticleSystem> _particleSystems = new List<ParticleSystem>();

        public void Update(float dt, Vector2 bounds)
        {
            foreach(var ps in _particleSystems)
                ps.Update(dt, bounds);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(var ps in _particleSystems)
                ps.Draw(sb);
        }

        public void RemoveParticleSystem(ParticleSystem ps)
        {
            _particleSystems.Remove(ps);
        }

        public void RemoveAll()
        {
            _particleSystems.Clear();
        }
    }
}
