using System;
using Lumen.Light_System;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Lumen.States
{
    internal class PauseState : State
    {
        private PlayerIndex _controllingIndex;

        public PauseState(PlayerIndex idx)
        {
            _controllingIndex = idx;
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
            MediaPlayer.Pause();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Game.Exit();
            }

            if(GamePad.GetState(_controllingIndex).IsConnected) {
                if(InputManager.GamepadButtonPressed(_controllingIndex, Buttons.Start)) {
                    Exit();
                }
                else if(InputManager.GamepadButtonPressed(_controllingIndex, Buttons.Back)) {
                    ReturnToMainMenu();
                }
            }
            else {
                Exit();
            }
            
            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        private static void ReturnToMainMenu()
        {
            MediaPlayer.Stop();
            StateManager.Instance.PopAll();
            StateManager.Instance.PushState(new MainMenuState());
        }

        private void Exit()
        {
            StateManager.Instance.PopState();
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(TextureManager.GetTexture("pause_screen"), Vector2.Zero,Color.White);

            spriteBatch.End();
        }
    }
}