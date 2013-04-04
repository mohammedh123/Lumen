using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Coin : NonPhysicsProp
    {
        public override bool CanCollide
        {
            get { return true; }
        }

        public Coin(Vector2 position) : base("coin", position)
        {
            PropType = PropTypeEnum.Coin;
        }

        public override void OnCollide(PhysicsEntity collider)
        {
            var player = collider as Player;

            if (player != null && player.CanPickUpCoins) //if collider is a player, basically, and can pick up coins
            {
                player.CoinCount++;

                IsToBeRemoved = true;
            }
        }
    }
}
