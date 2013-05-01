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
    class MainMenuState : State
    {
        private Texture2D _lumenLogo;
        private Vector2 _logoOrigin, _logoPosition;

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);

            _logoPosition = new Vector2(GameDriver.DisplayResolution.X/2, 250);
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            const string logoString = "mainmenu_logo";
            _lumenLogo = TextureManager.GetTexture(logoString);
            _logoOrigin = TextureManager.GetOrigin(logoString);
        }

        public override void Shutdown()
        {
        }

        public override void Update(GameTime delta)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.Exit();

            if(InputManager.KeyPressed(Keys.Enter)) {
                StateManager.Instance.PushState(new MainGameState());
            }

            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(_lumenLogo, _logoPosition,null,Color.White,0.0f,_logoOrigin,1.0f,SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
