using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Wax : Prop
    {
        public Wax(string textureKeyName, Vector2 position) : base(textureKeyName, position)
        {
            PropType = PropTypeEnum.Wax;
        }
    }
}
