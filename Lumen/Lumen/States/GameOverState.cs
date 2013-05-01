using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.States
{
    class GameOverState : State
    {
        int selected = -1;

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);

        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }

        public override void Shutdown()
        {
        }

        public override void Update(GameTime delta)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.Exit();
            
            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
        }
    }
}
