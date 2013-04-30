using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.State_Management
{
    public abstract class State
    {
        public double TotalTime = 0;
        protected Game game;

        public virtual void Initialize(Game g) { game = g; }
        public abstract void Shutdown();

        public abstract void Update(double delta);
        public abstract void Draw(SpriteBatch g, GraphicsDevice gd);
    }
}
