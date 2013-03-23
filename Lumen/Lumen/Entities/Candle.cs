using System;
using Microsoft.Xna.Framework;

namespace Lumen.Entities
{
    class Candle : Prop, ILightProvider
    {
        public Player Owner = null;

        //ILightProvider members
        public Color LightColor { get; set; }
        public float Radius { get; set; }

        private readonly float _baseRadius;

        public Candle(string textureKeyName, Vector2 position, Player owner) : base(textureKeyName, position)
        {
            PropType = PropTypeEnum.Candle;

            Owner = owner;
            Radius = GameVariables.CandleInitialRadius;
            _baseRadius = Radius;
            LightColor = Color.White;
        }

        public override void Update(float dt)
        {
            Radius = _baseRadius +
                     GameVariables.CandleFlickerAmount*
                     (float) Math.Sin(Lifetime*MathHelper.Pi/GameVariables.CandleFlickerPeriod);

            base.Update(dt);
        }
    }
}
