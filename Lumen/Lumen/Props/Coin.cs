using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Coin : Prop
    {
        public override bool CanCollide
        {
            get { return true; }
        }

        public Coin(Vector2 position) : base("coin", position)
        {
        }

        public override void OnCollide(Entity collider)
        {
            var player = collider as Player;

            player.CoinCount++;

            IsToBeRemoved = true;
        }
    }
}
