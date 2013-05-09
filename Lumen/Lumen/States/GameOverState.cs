using System;
using Lumen.Light_System;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.States
{
    internal class GameOverState : State
    {
        private Texture2D _playersWin, _guardianWin, _lumenBackground;
        private readonly LightManager _lightManager;
        private RenderTarget2D _sceneRt;
        private bool playersWin;

        public GameOverState(GameState winner)
        {
            _lightManager = new LightManager();

            switch(winner) {
                case GameState.PlayersWin:
                    playersWin = true;
                    break;
                case GameState.EnemyWins:
                    playersWin = false;
                    break;
                default:
                    throw new Exception("Invalid winner passed to GameOverState.");
            }
        }

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _lumenBackground = TextureManager.GetTexture("background");
            //_tutorialOverlay = TextureManager.GetTexture("tutorial_overlay");
            _playersWin = TextureManager.GetTexture("players_win");
            _guardianWin = TextureManager.GetTexture("guardian_win");
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Game.Exit();
            }

            for (var idx = PlayerIndex.One; idx <= PlayerIndex.Four; idx++)
            {
                if (GamePad.GetState(idx).IsConnected)
                {
                    if (InputManager.GamepadButtonPressed(idx, Buttons.A)) {
                        TransitionBackToMainMenu();
                    }
                }
            }

            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        private void TransitionBackToMainMenu()
        {
            StateManager.Instance.PopAll();
            StateManager.Instance.PushState(new MainMenuState());
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
            spriteBatch.Draw(playersWin ? _playersWin : _guardianWin, Vector2.Zero, Color.White);

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