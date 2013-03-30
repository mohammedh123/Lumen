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
        internal static bool Collides(Player player, Prop prop)
        {
            return CirclesCollide(player.Position, GameVariables.PlayerCollisionRadius, prop.Position,
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

        internal static bool Collides(Player player, Player otherPlayer, bool useVelocityPlayerOne = false, bool useVelocityPlayerTwo = false)
        {
            var p1 = new Rectangle((int) (player.Position.X - GameVariables.PlayerCollisionRadius + (useVelocityPlayerOne ? player.Velocity.X : 0)),
                                   (int)(player.Position.Y - GameVariables.PlayerCollisionRadius + (useVelocityPlayerOne ? player.Velocity.Y : 0)),
                                   (int) (GameVariables.PlayerCollisionRadius*2),
                                   (int) (GameVariables.PlayerCollisionRadius*2));

            var p2 = new Rectangle((int)(otherPlayer.Position.X - GameVariables.PlayerCollisionRadius + (useVelocityPlayerTwo ? otherPlayer.Velocity.X : 0)),
                                   (int)(otherPlayer.Position.Y - GameVariables.PlayerCollisionRadius + (useVelocityPlayerTwo ? otherPlayer.Velocity.Y : 0)),
                                   (int) (GameVariables.PlayerCollisionRadius*2),
                                   (int) (GameVariables.PlayerCollisionRadius*2));

            return p1.Intersects(p2);
        }

        private static bool CirclesCollide(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
        {
            return (centerA - centerB).LengthSquared() < (radiusA + radiusB)*(radiusA + radiusB);
        }
    }
}
