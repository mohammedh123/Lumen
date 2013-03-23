using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen
{
    public class Collider
    {
        internal static bool Collides(Player player, Prop prop)
        {
            return CirclesCollide(player.Position, GameVariables.PlayerCollisionRadius, prop.Position,
                                  GameVariables.EnemyCollisionRadius);
        }

        private static bool CirclesCollide(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
        {
            return (centerA - centerB).LengthSquared() < (radiusA + radiusB)*(radiusA + radiusB);
        }
    }
}
