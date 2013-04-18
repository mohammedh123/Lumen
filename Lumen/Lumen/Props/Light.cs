using System;
using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class Light : Prop, ILightProvider
    {
        //ILightProvider members
        public Color LightColor { get; set; }
        public float LightRadius { get; set; }

        public override bool CanInteract
        {
            get { return true; }
        }

        public Light(string textureKeyName, Vector2 position, float lifetime) : base(textureKeyName, position)
        {
            PropType = PropTypeEnum.Candle;

            LightColor = Color.White;
            Lifetime = lifetime;
        }

        public override void Update(float dt)
        {
            Lifetime -= dt;

            if (Lifetime <= 0 || LightRadius <= 0) {
                IsToBeRemoved = true;
            }
        }
    }
}
