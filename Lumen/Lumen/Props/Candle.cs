using System;
using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Candle : Prop, ILightProvider
    {
        public Player Owner = null;

        //ILightProvider members
        public Color LightColor { get; set; }
        public float Radius { get; set; }

        private readonly float _baseRadius;

        public override bool CanInteract
        {
            get { return true; }
        }

        public Candle(string textureKeyName, Vector2 position, Player owner, float lifetime) : base(textureKeyName, position)
        {
            PropType = PropTypeEnum.Candle;

            Owner = owner;
            Radius = GameVariables.CandleInitialRadius;
            _baseRadius = Radius;
            LightColor = Color.White;
            Lifetime = lifetime;
        }

        public override void Update(float dt)
        {
            Radius -= (GameVariables.CandleMinFlicker + (float)GameDriver.RandomGen.NextDouble()*GameVariables.CandleMaxFlicker)*dt;

            Lifetime -= dt;

            if (Lifetime <= 0 || Radius <= 0) {
                IsToBeRemoved = true;
                Owner.NumCandlesLeft++;
            }
        }

        public override void OnInteract(Entity collider)
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
