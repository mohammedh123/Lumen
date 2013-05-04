using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Entities;
using Lumen.Light_System;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Crystal : Prop, ILightProvider
    {
        public Color LightColor { get; set; }
        public float LightRadius
        {
            get
            {
                if (!IsSomeoneCollectingThis) return 0;

                return (1-(_collectionTimeLeft/GameVariables.CrystalCollectionTime))*
                       GameVariables.CrystalGlowRadius;
            }
            set { }
        }

        public Player Collector;

        public float _collectionTimeLeft = GameVariables.CrystalCollectionTime;
        public float LightIntensity { get; set; }

        private int _collectorCount = 0;
        private readonly List<Player> _collectors = new List<Player>();

        public void DecrementCollectorCount(Player p)
        {
            _collectorCount = Math.Max(_collectorCount - 1, 0);
            _collectors.Remove(p);
        }

        public void IncrementCollectorCount(Player p)
        {
            _collectorCount++;
            _collectors.Add(p);
        }

        public bool IsSomeoneCollectingThis
        {
            get { return _collectorCount > 0; }
        }

        public override bool CanCollide
        {
            get { return false; }
        }

        public Crystal(Vector2 position)
            : base("crystal", position)
        {
            LightIntensity = 1.0f;
            PropType = PropTypeEnum.Crystal;

            Health = GameVariables.CrystalHarvestRequirement;
        }

        public override void OnCollide(Entity collider)
        {
            var player = collider as Player;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            
            if (IsSomeoneCollectingThis) {
                SoundManager.GetSoundInstance("crystal_charge").Play();

                _collectionTimeLeft -= dt*_collectorCount;

                if (_collectionTimeLeft <= 0) {
                    DecrementCount();
                    Collector = _collectors.First();
                    _collectors.ForEach(p => p.ResetCollecting());
                }
            }
            else {
                _collectionTimeLeft = GameVariables.CrystalCollectionTime;
            }
        }

        public void DecrementCount()
        {
            Health--;

            if (Health <= 0) {
                IsToBeRemoved = true;
            }

            LightSpawner.Instance.AddStaticLight(Position, 1.0f,GameVariables.CrystalGlowRadius, 1);
        }
    }
}
