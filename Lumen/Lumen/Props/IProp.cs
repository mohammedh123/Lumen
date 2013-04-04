using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Props
{
    interface IProp
    {
        float Lifetime { get; set; }
        PropTypeEnum PropType { get; set; }
        Vector2 Position { get; }

        //these must be overridden
        bool CanCollide { get; set; }
        bool CanInteract { get; set; }

        bool IsToBeRemoved { get; set; }

        void Update(float dt);

        void Draw(SpriteBatch sb);

        void OnCollide(PhysicsEntity collider);
        void OnInteract(PhysicsEntity collider);
    }
}
