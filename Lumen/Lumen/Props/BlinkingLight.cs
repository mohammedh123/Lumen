using System;
using Lumen.Entities;

namespace Lumen.Props
{
    class BlinkingLight : Light
    {
        public Entity Owner = null;

        public float timer;
        public float durationTimer;

        public BlinkingLight(string textureKeyName, Entity owner) : base(textureKeyName, owner.Position, 0)
        {
            Owner = owner;
            IsVisible = false;
            Radius = 0;
        }

        public override void Update(float dt)
        {
            Position = Owner.Position;
            
            if(timer == 0.0f)
            {
                timer = GameVariables.BlinkingPeriod;
                durationTimer = GameVariables.BlinkingDuration;
            }

            if(durationTimer > 0)
            {
                Radius = GameVariables.BlinkingRadius;
            }
            else
            {
                Radius = 0;
            }

            timer = Math.Max(timer - dt, 0.0f);
            durationTimer = Math.Max(durationTimer - dt, 0);
        }
    }
}
