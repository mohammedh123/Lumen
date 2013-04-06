using Lumen.Entities;

namespace Lumen.Props
{
    class AttachedCandle : Candle
    {
        public AttachedCandle(string textureKeyName, Player owner) : base(textureKeyName, owner.Position, owner, 0)
        {
            IsVisible = false;
        }

        public override void Update(float dt)
        {
            Position = Owner.Position;
        }
    }
}
