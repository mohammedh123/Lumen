using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Props;
using Microsoft.Xna.Framework;

namespace Lumen
{
    public static class Collider
    {
        internal static bool IsPlayerWithinRadius(Player e, Vector2 center, float radius)
        {
            return CirclesCollide(e.Position, GameVariables.PlayerCollisionRadius, center, radius);
        }

        internal static bool Collides(Player player, Prop prop)
        {
            return CirclesCollide(player.Position, GameVariables.PlayerCollisionRadius, prop.Position,
                                  GameVariables.EnemyCollisionRadius);
        }

        public static bool CirclesCollide(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
        {
            return (centerA - centerB).LengthSquared() < (radiusA + radiusB)*(radiusA + radiusB);
        }
    }
}
