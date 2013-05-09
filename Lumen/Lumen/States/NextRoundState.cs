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
    internal class NextRoundState : State
    {
        private enum ScreenState
        {
            FadingIn,
            DisplayingInFull,
            FadingOut,
            FinishedFading
        }

        private Texture2D _numTexture, _lumenBackground;
        private Vector2 _numOrigin;
        private readonly LightManager _lightManager;
        private RenderTarget2D _sceneRt;
        private readonly int _count;
        private readonly List<PlayerIndex> _playerOrder;
        private ScreenState _state = ScreenState.FadingIn;

        private const float Duration = 1.0f;
        private const float FadeOutDuration = 0.25f;
        private const float FadeInDuration = 0.25f;
        private float _numAlpha = 0.0f;
        
        public NextRoundState(int count, List<PlayerIndex> playerOrder)
        {
            _lightManager = new LightManager();
            _playerOrder = playerOrder;

            _count = count;
        }

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _sceneRt = new RenderTarget2D(graphicsDevice, (int)GameDriver.DisplayResolution.X,
                                          (int)GameDriver.DisplayResolution.Y);

            _lightManager.LoadContent(graphicsDevice, contentManager, (int)GameDriver.DisplayResolution.X,
                                      (int)GameDriver.DisplayResolution.Y);
            _lightManager.SetDarknessLevel(0.8f);

            switch (_count)
            {
                case 5:
                    _numTexture = TextureManager.GetTexture("goal_five");
                    _numOrigin = TextureManager.GetOrigin("goal_five");
                    break;
                case 6:
                    _numTexture = TextureManager.GetTexture("goal_six");
                    _numOrigin = TextureManager.GetOrigin("goal_six");
                    break;
                case 7:
                    _numTexture = TextureManager.GetTexture("goal_seven");
                    _numOrigin = TextureManager.GetOrigin("goal_seven");
                    break;
                case 8:
                    _numTexture = TextureManager.GetTexture("goal_eight");
                    _numOrigin = TextureManager.GetOrigin("goal_eight");
                    break;
                case 9:
                    _numTexture = TextureManager.GetTexture("goal_nine");
                    _numOrigin = TextureManager.GetOrigin("goal_nine");
                    break;
                default:
                    throw new Exception("Error: a round count that is not between 5 and 9 (inclusive) was attempted.");
            }

            _lumenBackground = TextureManager.GetTexture("background");

            SoundManager.GetSoundInstance("round_change").Play();
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

            if (TotalTime <= FadeInDuration) {
                _state = ScreenState.FadingIn;
            }
            else if (TotalTime > FadeInDuration && TotalTime <= FadeInDuration + Duration)
            {
                _state = ScreenState.FadingOut;
            }
            else if (TotalTime > FadeInDuration +Duration && TotalTime <= FadeInDuration + Duration + FadeOutDuration)
            {
                _state = ScreenState.FadingOut;
            }
            else if (TotalTime >= FadeInDuration + Duration + FadeOutDuration)
            {
                _state = ScreenState.FinishedFading;
            }

            if (_state == ScreenState.FadingIn)
            {
                _numAlpha = MathHelper.Lerp(0.0f, 1.0f, (float)(TotalTime) / FadeInDuration);
            }
            else if (_state == ScreenState.DisplayingInFull) {
                _numAlpha = 1.0f;
            }
            else if (_state == ScreenState.FadingOut) {
                _numAlpha = MathHelper.Lerp(1.0f, 0.0f, (float) (TotalTime - Duration - FadeInDuration)/FadeOutDuration);
            }
            else if (_state == ScreenState.FinishedFading) {
                TransitionBackToGame();
                return;
            }
            else {
                throw new ArgumentOutOfRangeException();
            }
            
            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        private void TransitionBackToGame()
        {
            StateManager.Instance.PopState();
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
            
            var center = GameDriver.DisplayResolution*0.5f;

            spriteBatch.Draw(_numTexture, center, null, Color.White*_numAlpha, 0.0f, _numOrigin, 1.0f, SpriteEffects.None, 0);
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