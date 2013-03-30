using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Entities
{
    abstract class Entity
    {
        protected Texture2D Texture { get; set; }
        protected Vector2 TextureOrigin { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        protected float Angle = 0f;
        protected Color Color = Color.White;

        public bool IsVisible = true;

        protected Entity(string textureKeyName, Vector2 position)
        {
            Texture = TextureManager.GetTexture(textureKeyName);
            TextureOrigin = TextureManager.GetOrigin(textureKeyName);

            Position = position;
        }

        public abstract void Update(float dt);
        
        public virtual void Draw(SpriteBatch sb)
        {
            if(IsVisible)
                sb.Draw(Texture, Position, null, Color, Angle,TextureOrigin,1.0f,SpriteEffects.None,0);
        }

        protected void AdjustVelocity(float dx, float dy)
        {
            Velocity += new Vector2(dx, dy);
        }

        public void ApplyVelocity()
        {
            Position += Velocity;
        }

        public void ResetVelocity()
        {
            Velocity = Vector2.Zero;
        }
    }
}
