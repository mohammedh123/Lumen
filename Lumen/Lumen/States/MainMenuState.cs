using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Light_System;
using Lumen.Particle_System;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Lumen.States
{
    class MainMenuState : State
    {
        enum SetupState
        {
            SettingPlayersUp,
            TransitioningToGame
        }
        private readonly LightManager _lightManager;

        private SetupState _state = SetupState.SettingPlayersUp;
        private Texture2D _lumenBackground, _lumenLogo, _playerSprites;
        private Vector2 _logoOrigin, _logoPosition, _playerOrigin, _textPosition;
        private SpriteFont _mainMenuFont;
        private readonly bool[] _playersPlaying = new bool[4];
        private readonly BasicLight[] _playersLight = new BasicLight[4];
        private readonly Player[] _players = new Player[4];
        private PlayerIndex _lastPlayerReady;
        private const float DistanceBetweenPlayerSprites = 96;
        private RenderTarget2D _sceneRt;

        private static readonly Color[] PlayerColors = new Color[4]
                                                       {
                                                           Color.Red, Color.Green, Color.Blue, Color.Violet
                                                       };

        private bool AreAllPlayersReady
        {
            get { return _playersPlaying.All(b => b); }
        }

        public MainMenuState()
        {
            _lightManager = new LightManager();
        }

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);

            _logoPosition = new Vector2(GameDriver.DisplayResolution.X / 2, 250);

            var leftMostPlayerPos = new Vector2(GameDriver.DisplayResolution.X / 2, 550);
            const float width = 3 * DistanceBetweenPlayerSprites;
            leftMostPlayerPos -= new Vector2(width * 0.5f, 0);

            _textPosition = new Vector2(GameDriver.DisplayResolution.X / 2, 500);

            for(int i = 0; i < 4; i++) {
                _players[i] = new Player("player" + (i+1),leftMostPlayerPos + new Vector2(i*DistanceBetweenPlayerSprites, 0))
                              {ControllerIndex = PlayerIndex.One + i};
                _playersLight[i] = new BasicLight()
                                   {
                                       IsVisible = true,
                                       LightColor = Color.White,
                                       LightIntensity = 0.5f,
                                       LightRadius = 24.0f,
                                       Position = _players[i].Position
                                   };

            }
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            const string logoString = "mainmenu_logo";
            _lumenBackground = TextureManager.GetTexture("background");
            _lumenLogo = TextureManager.GetTexture(logoString);
            _logoOrigin = TextureManager.GetOrigin(logoString);
            _mainMenuFont = TextureManager.GetFont("debug");
            _playerOrigin = TextureManager.GetOrigin("white_player");
            _playerSprites = TextureManager.GetTexture("white_player");
            
            _sceneRt = new RenderTarget2D(graphicsDevice, (int)GameDriver.DisplayResolution.X, (int)GameDriver.DisplayResolution.Y);
            _lightManager.LoadContent(graphicsDevice, contentManager, (int)GameDriver.DisplayResolution.X, (int)GameDriver.DisplayResolution.Y);
        }

        public override void Shutdown()
        {
        }

        public override void Update(GameTime delta)
        {
            var song = SoundManager.GetSong("main_bgm");

            if (MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(song);
            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.Exit();

            if(InputManager.KeyPressed(Keys.Enter)) {
                TransitionToMainGame();
            }

            if (_state == SetupState.SettingPlayersUp) {
                for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++) {
                    if (GamePad.GetState(i).IsConnected) {
                        if (InputManager.GamepadButtonPressed(i, Buttons.A)) {
                            if (!_playersPlaying[(int)i])
                            {
                                _playersLight[(int)i].LightRadius = 64;
                                _playersLight[(int)i].LightIntensity = 1.0f;
                                _players[(int)i].SetTexture("player" + (_playersPlaying.Count(b => b)+1));
                                SoundManager.GetSoundInstance("player_light").Play();

                                _playersPlaying[(int) i] = true;
                                _lastPlayerReady = i;

                                if (AreAllPlayersReady) {
                                    _state = SetupState.TransitioningToGame;
                                    TransitionToMainGame();
                                }
                            }
                        }
                    }
                }
            }

            foreach(var player in _players)
                player.OrbitRing.Update((float)delta.ElapsedGameTime.TotalSeconds);

            TotalTime += delta.ElapsedGameTime.TotalSeconds;
        }

        private void TransitionToMainGame()
        {
            StateManager.Instance.PushState(new MainGameState(_lastPlayerReady));
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            const string pressAStr = "Press (X)";

            graphicsDevice.SetRenderTarget(_sceneRt);
            DrawScene(spriteBatch);

            _lightManager.DrawScene(GetLights(), graphicsDevice, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
            GameDriver.DrawFullscreenQuad(_sceneRt, spriteBatch, true);
            spriteBatch.End();
            _lightManager.DrawLightDarkness(graphicsDevice, spriteBatch, _sceneRt);

            spriteBatch.Begin();
            spriteBatch.Draw(_lumenLogo, _logoPosition, null, Color.White, 0.0f, _logoOrigin, 1.0f, SpriteEffects.None,
                             0);
            spriteBatch.DrawString(_mainMenuFont, pressAStr, GameDriver.GetFontPositionAtCenter(pressAStr, _mainMenuFont, _textPosition), Color.White);
            spriteBatch.End();
        }

        private IEnumerable<ILightProvider> GetLights()
        {
            return _playersLight;
        }

        private void DrawScene(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_lumenBackground, Vector2.Zero, Color.White);

            foreach(var player in _players)
            {
                DrawPlayerInformation(player, spriteBatch);
            }
                                                                                                                                                                                                                                                                                                                                                                                
            spriteBatch.End();
        }

        private void DrawPlayerInformation(Player player, SpriteBatch spriteBatch)
        {
            if (GamePad.GetState(player.ControllerIndex).IsConnected)
                if (_playersPlaying[(int)player.ControllerIndex])
                    player.Draw(spriteBatch);
                else
                    spriteBatch.Draw(_playerSprites, player.Position, null, _playersPlaying[(int)player.ControllerIndex] ? Color.Lerp(PlayerColors[(int)player.ControllerIndex], Color.White, 0.5f) : Color.White, 0.0f, _playerOrigin, 1.0f, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(_playerSprites, player.Position, null, Color.White*0.25f, 0.0f, _playerOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
