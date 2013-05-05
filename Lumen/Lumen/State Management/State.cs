using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.State_Management
{
    public abstract class State
    {
        protected GameDriver Game;
        public double TotalTime = 0;

        public virtual void Initialize(GameDriver g)
        {
            Game = g;
        }

        public abstract void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice);
        public abstract void Shutdown();

        public abstract void Update(GameTime delta);
        public abstract void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
    }
}