using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.States
{
    public enum GAME_STATE
    {
        PLAYING = 0x001,
        PAUSED = 0x002,

        SIDE_MAIN = 0x010,
        SIDE_BUILD = 0x020,
        SIDE_EINFO = 0x040,
        SIDE_TINFO = 0x080,

        LOSING_FADE_OUT = 0x100,
        GO_TO_NEXT_LEVEL = 0x200
    }

    class MainGameState : State
    {
        public override void Initialize(Game g)
        {
            base.Initialize(g);

        }

        public override void Shutdown()
        {
        }

        public override void Update(double delta)
        {
        }
        
        public override void Draw(SpriteBatch g, GraphicsDevice gd)
        {
        }
    }
}
