using System;
using Lumen.Entities;

namespace Lumen.Props
{
    class AttachedCandle : Candle
    {
        public float timer;
        public float durationTimer;

        public AttachedCandle(string textureKeyName, Player owner) : base(textureKeyName, owner.Position, owner, 0)
        {
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
