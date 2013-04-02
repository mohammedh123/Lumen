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

        //http://lazyfoo.net/SDL_tutorials/lesson17/
        internal static bool Collides(Player player, Player otherPlayer, bool useVelocityXPlayerOne, bool useVelocityYPlayerOne, bool useVelocityXPlayerTwo, bool useVelocityYPlayerTwo)
        {
            var leftA = player.Position.X - GameVariables.PlayerCollisionRadius + (useVelocityXPlayerOne ? player.Velocity.X : 0);
            var topA = player.Position.Y - GameVariables.PlayerCollisionRadius + (useVelocityYPlayerOne ? player.Velocity.Y : 0);
            var rightA = leftA + GameVariables.PlayerCollisionRadius * 2;
            var bottomA = topA + GameVariables.PlayerCollisionRadius * 2;

            var leftB = otherPlayer.Position.X - GameVariables.PlayerCollisionRadius + (useVelocityXPlayerTwo ? otherPlayer.Velocity.X : 0);
            var topB = otherPlayer.Position.Y - GameVariables.PlayerCollisionRadius + (useVelocityYPlayerTwo ? otherPlayer.Velocity.Y : 0);
            var rightB = leftB + GameVariables.PlayerCollisionRadius * 2;
            var bottomB = topB + GameVariables.PlayerCollisionRadius * 2;

            if (bottomA <= topB) return false;
            if (topA >= bottomB) return false;
            if (rightA <= leftB) return false;
            if (leftA >= rightB) return false;

            return true;
        }

        private static bool CirclesCollide(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
        {
            return (centerA - centerB).LengthSquared() < (radiusA + radiusB)*(radiusA + radiusB);
        }
    }
}
