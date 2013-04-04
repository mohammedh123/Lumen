using Lumen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Props
{
    class NonPhysicsProp : NonPhysicsEntity, IProp 
    {
        public float Lifetime { get; set; }
        public PropTypeEnum PropType { get; set; }

        //these must be overridden
        public virtual bool CanCollide { get; set; }
        public virtual bool CanInteract { get; set; }

        public bool IsToBeRemoved { get; set; }

        public NonPhysicsProp(string textureKeyName, Vector2 position)
            : base(textureKeyName, position)
        {
            Lifetime = 0.0f;
        }

        public override void Update(float dt)
        {
            Lifetime += dt;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public virtual void OnCollide(PhysicsEntity collider)
        {
        }

        public virtual void OnInteract(PhysicsEntity collider)
        {
        }
    }
}
