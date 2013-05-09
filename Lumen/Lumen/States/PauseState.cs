using System;
using Lumen.Light_System;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.States
{
    internal class PauseState : State
    {
        private PlayerIndex _controllingIndex;

        public PauseState(PlayerIndex idx)
        {
            _controllingIndex = idx;
        }

        public override bool IsPassthrough()
        {
            return true;
        } 

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
        }

        public override void Shutdown()
        {
        }

        public override void Update(GameTime delta)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Game.Exit();
            }

            if(GamePad.GetState(_controllingIndex).IsConnected) {
                
            }
            else {
                Exit();
            }
            
            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        private void Exit()
        {
            StateManager.Instance.PopState();
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(TextureManager.GetTexture("mainmenu_logo"), Vector2.One*300,Color.White);

            spriteBatch.End();
        }
    }
}