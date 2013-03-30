using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen
{
    class Block : Prop
    {
        public bool IsMoveable { get; set; }
        public Rectangle BoundingBox;

        public Block(Vector2 topLeftCorner, int size, bool isMoveable = false) : base("block", new Vector2(topLeftCorner.X + size/2.0f, topLeftCorner.Y + size/2.0f))
        {
            IsMoveable = isMoveable;
            BoundingBox = new Rectangle((int)topLeftCorner.X, (int)topLeftCorner.Y, size, size);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture,BoundingBox,Color);
        }
    }
}
