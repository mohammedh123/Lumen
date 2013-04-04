using Lumen.Entities;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace Lumen.Props
{
    class Candle : NonPhysicsProp, ILightProvider
    {
        public Player Owner = null;

        public Vector2 Position { get; set; }
        public Color LightColor { get; set; }
        public float Radius { get; set; }

        private readonly float _baseRadius;

        public override bool CanInteract
        {
            get { return true; }
        }

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

        public override void OnInteract(PhysicsEntity collider)
        {
            var player = collider as Player;

            if (player != null) {
                //pick up candle, increase player's candle count
                player.NumCandlesLeft++;

                IsToBeRemoved = true;
            }
        }
    }
}
