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

                return (_collectors.Max(p => p.CollectingTime)/GameVariables.CrystalCollectionTime)*
                       GameVariables.CrystalGlowRadius;
            }
            set { }
        }

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
            get { return true; }
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
