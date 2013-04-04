using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Props;
using Microsoft.Xna.Framework;

namespace Lumen
{
    public class Collider
    {
        internal static bool Collides(PhysicsEntity entity, IProp physicsProp)
        {
            return CirclesCollide(entity.Position, GameVariables.PlayerCollisionRadius, physicsProp.Position,
                                  GameVariables.EnemyCollisionRadius);
        }

        internal static bool Collides(Player player, Block block, bool useVelocity = false)
        {
            if(useVelocity)
            {
                
            }
            else
            {
                
            }

            return false;
        }

        private static bool CirclesCollide(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
        {
            return (centerA - centerB).LengthSquared() < (radiusA + radiusB)*(radiusA + radiusB);
        }

        internal static bool CircleRect(float cx, float cy, float r, Rectangle rect)
        {
            var closestX = MathHelper.Clamp(cx, rect.Left, rect.Right);
            var closestY = MathHelper.Clamp(cy, rect.Top, rect.Bottom);

            var distX = cx - closestX;
            var distY = cy - closestY;

            var dS = (distX * distX) + (distY * distY);
            return dS < (r * r);
        }

        internal static bool Collides(PhysicsEntity entity, Rectangle rect)
        {
            return new Rectangle((int)(entity.Position.X - GameVariables.PlayerCollisionRadius),
                                 (int)(entity.Position.Y - GameVariables.PlayerCollisionRadius),
                                 (int)GameVariables.PlayerCollisionRadius * 2, (int)GameVariables.PlayerCollisionRadius * 2).Intersects(rect);
        }

        internal static bool Collides(Candle entity, Rectangle rect)
        {
            return CircleRect(entity.Position.X, entity.Position.Y, entity.Radius*0.8f, rect);
        }
    }
}
