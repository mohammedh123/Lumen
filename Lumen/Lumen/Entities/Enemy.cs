using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Entities
{
    class Enemy : PhysicsEntity
    {
        public Enemy(Vector2 position, World world) : base("enemy", position, GameVariables.EnemyCollisionRadius, world)
        {
        }

        public override void Update(float dt)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
        }
    }
}
