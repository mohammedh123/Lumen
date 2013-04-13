using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Crystal : Prop
    {
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
