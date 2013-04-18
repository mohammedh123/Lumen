using System;
using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Crystal : Prop, ILightProvider
    {

        public Color LightColor { get; set; }
        public float LightRadius
        {
            get { return IsSomeoneCollectingThis ? GameVariables.CrystalGlowRadius : 0; }
            set { }
        }

        private int _collectorCount = 0;

        public void DecrementCollectorCount()
        {
            _collectorCount = Math.Max(_collectorCount - 1, 0);
        }

        public void IncrementCollectorCount()
        {
            _collectorCount++;
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
            PropType = PropTypeEnum.Crystal;

            Health = GameVariables.CrystalHarvestRequirement;
        }

        public override void OnCollide(Entity collider)
        {
            var player = collider as Player;
        }
    }
}
