using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.States
{
    class GameOverState : State
    {
        int selected = -1;

        public override void Initialize(Game g)
        {
            base.Initialize(g);

        }

        public override void Shutdown()
        {
        }

        public override void Update(double delta)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();
            
            TotalTime += delta;
        }

        public override void Draw(SpriteBatch g, GraphicsDevice gd)
        {
        }
    }
}
