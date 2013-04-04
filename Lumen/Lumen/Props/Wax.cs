using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Wax : PhysicsProp
    {
        public Wax(string textureKeyName, Vector2 position, World world) : base(textureKeyName, position, GameVariables.EnemyCollisionRadius, world)
        {
            PropType = PropTypeEnum.Wax;
        }
    }
}
