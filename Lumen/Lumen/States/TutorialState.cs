using System;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<PlayerIndex, bool> _playersPlaying;
        private Dictionary<PlayerIndex, int> _playerIdxs; 
        private const float DistanceBetweenSprites = 96.0f;

        public TutorialState(IEnumerable<PlayerIndex> playerOrder)
        {
            _lightManager = new LightManager();
            _playersPlaying = playerOrder.ToDictionary(pi => pi, pi => false);

            _playerIdxs = new Dictionary<PlayerIndex, int>();
            var idx = 0;
            foreach(var kvp in _playersPlaying) {
                _playerIdxs.Add(kvp.Key, idx);
                idx++;
            }
        }

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _lumenBackground = TextureManager.GetTexture("background");
            _tutorialOverlay = TextureManager.GetTexture("tutorial_overlay");
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

            if (InputManager.KeyPressed(Keys.Enter))
            {
                TransitionToMainGame();
            }

            for(var idx = PlayerIndex.One; idx <= PlayerIndex.Four; idx++) {
                if(GamePad.GetState(idx).IsConnected && _playersPlaying.ContainsKey(idx)) {
                    if (InputManager.GamepadButtonDown(idx, Buttons.A))
                        _playersPlaying[idx] = true;
                }
            }

            if(_playersPlaying.All(kvp => kvp.Value)) {
                TransitionToMainGame();
                return;
            }

            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        private void TransitionToMainGame()
        {
            StateManager.Instance.PopState();
            StateManager.Instance.PushState(new NextRoundState(7, _playersPlaying.Keys.ToList()));
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_sceneRt);
            DrawScene(spriteBatch);

            _lightManager.DrawScene(null, graphicsDevice, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
            GameDriver.DrawFullscreenQuad(_sceneRt, spriteBatch, true);
            spriteBatch.End();
            _lightManager.DrawLightDarkness(graphicsDevice, spriteBatch, _sceneRt);

            spriteBatch.Begin();
            
            spriteBatch.Draw(_tutorialOverlay, Vector2.Zero, Color.White);

            var leftMostFromCenter = new Vector2(GameDriver.DisplayResolution.X/2 - (_playersPlaying.Count-1) * DistanceBetweenSprites*0.5f, 518);
            
            int idx = 1;
            foreach (var kvp in _playerIdxs.OrderBy(kvp => kvp.Value))
            {
                var playerStr = "player" + (idx++);
                if (_playersPlaying[kvp.Key]) {
                    spriteBatch.Draw(TextureManager.GetTexture(playerStr), leftMostFromCenter, null, Color.White, 0.0f,
                                     TextureManager.GetOrigin(playerStr), 1.0f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(TextureManager.GetTexture(playerStr), leftMostFromCenter, null, Color.White*0.2f, 0.0f,
                                     TextureManager.GetOrigin(playerStr), 1.0f, SpriteEffects.None, 0);
                }

                leftMostFromCenter += new Vector2(DistanceBetweenSprites, 0);
            }

            spriteBatch.End();
        }

        private void DrawScene(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_lumenBackground, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}