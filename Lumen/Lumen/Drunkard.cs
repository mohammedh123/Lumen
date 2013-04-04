using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen
{
    class Drunkard : PhysicsEntity
    {
        public Drunkard(Vector2 position, World world)
            : base("drunkard", position, GameVariables.DrunkardCollisionRadius, world)
        {
              
        }

        public override void Update(float dt)
        {
        }
    }
}
