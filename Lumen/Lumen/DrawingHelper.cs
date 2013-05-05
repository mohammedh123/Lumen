using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen
{
    internal class DrawingHelper
    {
        public static void DrawHorizontalFilledBar(Vector2 topLeftCorner, SpriteBatch sb, Color outerBarColor,
                                                   Color innerBarColor, int totalWidth, int totalHeight, int innerRadius,
                                                   float percentFilled)
        {
            var outerRect = new Rectangle((int) topLeftCorner.X, (int) topLeftCorner.Y, totalWidth, totalHeight);
            var innerRect = new Rectangle(outerRect.X + innerRadius, outerRect.Y + innerRadius,
                                          (int) ((totalWidth - 2*innerRadius)*percentFilled),
                                          totalHeight - 2*innerRadius);

            sb.Draw(TextureManager.GetTexture("blank"), outerRect, outerBarColor);
            sb.Draw(TextureManager.GetTexture("blank"), innerRect, innerBarColor);
        }
    }
}