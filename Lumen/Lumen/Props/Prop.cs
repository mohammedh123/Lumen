using Lumen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Props
{
    internal enum PropTypeEnum
    {
        Wax,
        Candle,
        Coin,
        Crystal
    }

    internal class Prop : Entity
    {
        public Prop(string textureKeyName, Vector2 position) : base(textureKeyName, position)
        {
            Lifetime = 0.0f;
        }

        public float Lifetime { get; set; }
        public PropTypeEnum PropType { get; set; }

        //these must be overridden
        public virtual bool CanCollide { get; protected set; }
        public virtual bool CanInteract { get; protected set; }

        public bool IsToBeRemoved { get; set; }

        public virtual void Update(float dt)
        {
            Lifetime += dt;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public virtual void OnCollide(Entity collider)
        {
        }

        public virtual void OnInteract(Entity collider)
        {
        }
    }
}