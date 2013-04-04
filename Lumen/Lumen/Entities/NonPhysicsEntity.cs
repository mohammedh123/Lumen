using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Lumen.Entities
{
    abstract class NonPhysicsEntity
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        protected Texture2D Texture { get; set; }
        protected Vector2 TextureOrigin { get; set; }

        protected float Angle = 0f;
        public Color Color = Color.White;
        
        public bool IsVisible = true;

        protected NonPhysicsEntity(string textureKeyName, Vector2 position)
        {
            Texture = TextureManager.GetTexture(textureKeyName);
            TextureOrigin = TextureManager.GetOrigin(textureKeyName);

            Position = position;
        }

        public abstract void Update(float dt);

        public virtual void Draw(SpriteBatch sb)
        {
            if (IsVisible)
                sb.Draw(Texture, Position, null, Color, Angle, TextureOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
