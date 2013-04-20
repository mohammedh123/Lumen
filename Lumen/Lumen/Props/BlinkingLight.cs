using System;
using Lumen.Entities;

namespace Lumen.Props
{
    class BlinkingLight : Light
    {
        public Entity Owner = null;

        public float timer;
        public float durationTimer;
        private float _lightRadius;

        public BlinkingLight(string textureKeyName, Entity owner, float lightRadius) : base(textureKeyName, owner.Position, 0)
        {
            Owner = owner;
            IsVisible = false;
            LightRadius = 0;
            _lightRadius = lightRadius;
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
                LightRadius = _lightRadius;
            }
            else
            {
                LightRadius = 0;
            }

            timer = Math.Max(timer - dt, 0.0f);
            durationTimer = Math.Max(durationTimer - dt, 0);
        }
    }
}
