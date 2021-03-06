using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Entities
{
    internal abstract class Entity
    {
        private Color Color = Color.White;
        public float SpriteAngle;

        protected Entity(string textureKeyName, Vector2 position)
        {
            Texture = TextureManager.GetTexture(textureKeyName);
            TextureOrigin = TextureManager.GetOrigin(textureKeyName);
            IsVisible = true;
            Position = position;
        }

        private Texture2D Texture { get; set; }
        private Vector2 TextureOrigin { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public int Health { get; set; }
        public float Speed { get; set; }

        public bool IsVisible { get; set; }

        public virtual void SetTexture(string textureKeyName)
        {
            Texture = TextureManager.GetTexture(textureKeyName);
            TextureOrigin = TextureManager.GetOrigin(textureKeyName);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (IsVisible) {
                sb.Draw(Texture, Position, null, Color, SpriteAngle, TextureOrigin, 1.0f, SpriteEffects.None, 0);
            }
        }

        protected void AdjustVelocity(float dx, float dy)
        {
            Velocity += new Vector2(dx, dy);
        }

        public void ApplyVelocity(bool applyX = true, bool applyY = true)
        {
            if (applyX && applyY) {
                Position += Velocity;
            }
            else {
                Position = new Vector2(Position.X + (applyX ? Velocity.X : 0), Position.Y + (applyY ? Velocity.Y : 0));
            }
        }

        public void ResetVelocity()
        {
            Velocity = Vector2.Zero;
        }

        public void WrapPositionAround()
        {
            var posX = Position.X;
            var posY = Position.Y;

            if (posX >= GameDriver.DisplayResolution.X) {
                posX -= GameDriver.DisplayResolution.X;
            }
            else if (posX < 0) {
                posX = GameDriver.DisplayResolution.X + posX;
            }

            if (posY >= GameDriver.DisplayResolution.Y) {
                posY -= GameDriver.DisplayResolution.Y;
            }
            else if (posY < 0) {
                posY = GameDriver.DisplayResolution.Y + posY;
            }

            Position = new Vector2(posX, posY);
        }
    }
}