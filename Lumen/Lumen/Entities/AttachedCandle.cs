using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen.Entities
{
    class AttachedCandle : Candle
    {
        public AttachedCandle(string textureKeyName, Player owner) : base(textureKeyName, owner.Position, owner)
        {
            IsVisible = false;
        }

        public override void Update(float dt)
        {
            Position = Owner.Position;
        }
    }
}
