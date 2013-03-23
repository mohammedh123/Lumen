using Microsoft.Xna.Framework;

namespace Lumen.Entities
{
    class Wax : Prop
    {
        public Wax(string textureKeyName, Vector2 position) : base(textureKeyName, position)
        {
            PropType = PropTypeEnum.Wax;
        }
    }
}
