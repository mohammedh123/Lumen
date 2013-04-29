using System;
using Lumen.Entities;
using Lumen.Light_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Props
{
    class Light : Prop, ILightProvider
    {
        //ILightProvider members
        public Color LightColor { get; set; }
        public float LightRadius { get; set; }
        public float LightIntensity { get; set; }

        public Entity EntityAttachedTo { get; set; }

        public override bool CanInteract
        {
            get { return true; }
        }

        public Light(string textureKeyName, float radius, Vector2 position, Entity entityAttachedTo=null) : base(textureKeyName, position)
        {
            PropType = PropTypeEnum.Candle;

            LightColor = Color.White;
            LightIntensity = 1.0f;
            LightRadius = radius;
            EntityAttachedTo = entityAttachedTo;
        }

        public override void Update(float dt)
        {
            if (EntityAttachedTo != null)
                Position = EntityAttachedTo.Position;
        }

        public override void Draw(SpriteBatch sb)
        {
        }
    }
}
