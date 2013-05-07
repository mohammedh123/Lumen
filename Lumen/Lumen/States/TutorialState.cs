using System;
using Lumen.Light_System;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.States
{
    internal class TutorialState : State
    {
        private Texture2D _tutorialOverlay, _lumenBackground;
        private readonly LightManager _lightManager;
        private RenderTarget2D _sceneRt;

        public TutorialState()
        {
            _lightManager = new LightManager();    
        }

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _lumenBackground = TextureManager.GetTexture("background");
            _tutorialOverlay = TextureManager.GetTexture("texture_overlay");
            _sceneRt = new RenderTarget2D(graphicsDevice, (int)GameDriver.DisplayResolution.X,
                                          (int)GameDriver.DisplayResolution.Y);

            _lightManager.LoadContent(graphicsDevice, contentManager, (int)GameDriver.DisplayResolution.X,
                                      (int)GameDriver.DisplayResolution.Y);
            _lightManager.SetDarknessLevel(0.8f);
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

            for(var idx = PlayerIndex.One; idx <= PlayerIndex.Four; idx++) {
                
            }

            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_sceneRt);
            DrawScene(spriteBatch);
            
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
            GameDriver.DrawFullscreenQuad(_sceneRt, spriteBatch, true);
            spriteBatch.End();
            _lightManager.DrawLightDarkness(graphicsDevice, spriteBatch, _sceneRt);

            spriteBatch.Begin();
            spriteBatch.Draw(_tutorialOverlay, Vector2.Zero, Color.White);
        }

        private void DrawScene(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_lumenBackground, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}