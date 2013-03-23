using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Entities
{
    enum PropTypeEnum
    {
        Wax,
        Candle
    }

    class Prop : Entity
    {
        public float Lifetime { get; protected set; }
        public PropTypeEnum PropType { get; set; }

        public Prop(string textureKeyName, Vector2 position) : base(textureKeyName, position)
        {
            Lifetime = 0.0f;
        }

        public virtual void OnPickup(Entity pickerUpper) //TODO: event arguments, method most likely will be removed
        {
        }

        public virtual void OnPlace(Entity placer) //TODO: event arguments, method most likely will be removed
        {
        }

        public virtual void OnInteract(Entity placer) //TODO: event arguments, method most likely will be removed
        {
        }

        public override void Update(float dt)
        {
            Lifetime += dt;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
