using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Lumen.Entities;
using Lumen.Light_System;
using Lumen.Props;
using Lumen.State_Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.States
{
    class MainGameState : State
    {
        private readonly GameManager _gameManager;
        private readonly LightManager _lightManager;

        private RenderTarget2D _sceneRt;
#if DEBUG
        public bool IsShowingDebugInformation = false;
#endif

        public MainGameState()
        {
            _gameManager = new GameManager(GameDriver.DisplayResolution);
            _lightManager = new LightManager();
        }

        public override void Initialize(GameDriver g)
        {
            base.Initialize(g);

#if DEBUG
            LoadVariables();
#endif

            ResetRound();
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _sceneRt = new RenderTarget2D(graphicsDevice, (int)GameDriver.DisplayResolution.X, (int)GameDriver.DisplayResolution.Y);
            _lightManager.LoadContent(graphicsDevice, contentManager, (int)GameDriver.DisplayResolution.X, (int)GameDriver.DisplayResolution.Y);
        }

        public override void Shutdown()
        {
            for (var gp = PlayerIndex.One; gp <= PlayerIndex.Four; gp++)
            {
                GamePad.SetVibration(gp, 0, 0);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Game.Exit();
            }

#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.Back))
                LoadVariables();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                ResetRound();

            var scale = 1.0f + Mouse.GetState().ScrollWheelValue / 12000.0f;
            GameVariables.CameraZoom = scale;

            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                IsShowingDebugInformation = true;
            }
            else
            {
                IsShowingDebugInformation = false;
            }
#endif

            _gameManager.Update(gameTime);
        }

        private void ResetRound(bool shufflePlayers = true, bool reloadVariables = true)
        {
            if (reloadVariables)
                LoadVariables();

            if (shufflePlayers)
            {
                _gameManager.ResetCompletely();

                var randomEnemyIdx = GameDriver.RandomGen.Next(1, 1);

                var playerNum = 1;
                for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
                {
                    if (GamePad.GetState(i).IsConnected || i <= PlayerIndex.Two)
                    {

                        if (i == PlayerIndex.One + randomEnemyIdx)
                        {
                            _gameManager.AddEnemy(
                                new Guardian(new Vector2(GameDriver.DisplayResolution.X - 64, GameDriver.DisplayResolution.Y / 2)),
                                i);
                        }
                        else
                        {
                            _gameManager.AddPlayer(
                                new Player("player" + playerNum++,
                                           new Vector2(64, GameDriver.DisplayResolution.Y / 2 - 96 + 32 * playerNum)),
                                i);
                            _gameManager.Players.Last().PlayerSpriteIndex = playerNum - 1;
                        }
                    }
                }
            }

            for (var i = 0; i < GameVariables.CrystalsToSpawn(_gameManager.RoundNumber); i++)
            {
                _gameManager.SpawnCrystalUniformly();
            }
        }
        
#if DEBUG
        private void LoadVariables()
        {
            try
            {
                using (var streamReader = new StreamReader("variables.txt"))
                {
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (line.StartsWith("//") || line.StartsWith(@"\\")) continue;
                        if (String.IsNullOrEmpty(line)) continue;

                        var variableName = line.Substring(0, line.IndexOf(' ')).Trim();
                        var variableValue = line.Substring(line.IndexOf('=') + 1).Trim();

                        try
                        {
                            var actualVariable = typeof(GameVariables).GetField(variableName);
                            var actualVariableValue = Convert.ChangeType(variableValue, actualVariable.FieldType);
                            actualVariable.SetValue(null, actualVariableValue);

                            if (variableName == "PlayerLanternRadius")
                            {
                                foreach (var lantern in _gameManager.Props.Where(p => p is BlinkingLight))
                                    (lantern as BlinkingLight).LightRadius = (float)actualVariableValue;
                            }
                            else if (variableName == "PlayerOrbsDistance")
                            {
                                foreach (var p in _gameManager.Players)
                                    p.OrbitRing.Radius = (float)actualVariableValue;
                            }
                            else if (variableName == "PlayerOrbsPeriod")
                            {
                                foreach (var p in _gameManager.Players)
                                    p.OrbitRing.OrbitPeriod = (float)actualVariableValue;
                            }
                            else if (variableName == "PlayerLightDuration")
                            {
                                foreach (var light in _gameManager.Props.Where(p => p is Light))
                                {
                                    (light as Light).LightRadius = (float)actualVariableValue;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ErrorLog.Log("Error was encountered with exception:" + Environment.NewLine + e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Log("Error was encountered with exception:" + Environment.NewLine + e);
            }
        }
#endif

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_sceneRt);
            _gameManager.DrawScene(spriteBatch);

            _lightManager.DrawScene(_gameManager.GetLights(), graphicsDevice, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
            DrawFullscreenQuad(_sceneRt, spriteBatch, true);
            spriteBatch.End();
            _lightManager.DrawLightDarkness(graphicsDevice, spriteBatch, _sceneRt);

#if DEBUG
            if (IsShowingDebugInformation)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(TextureManager.GetFont("debug"), String.Format("{0}", GameVariables.CameraZoom), Vector2.Zero, Color.White);
                spriteBatch.End();
            }
#endif
            string s = null;
            if (_gameManager.State == GameState.PlayersWin)
                s = "Wisps win round " + _gameManager.RoundNumber.ToString("0") + "!\nNext round in: " + _gameManager.TimeTillNextRound.ToString("0.0");
            else if (_gameManager.State == GameState.EnemyWins)
                //s = "The guardian has won.\n(Start) Retry?\n(Back) Restart?";
                s = "Guardian wins!\nNext round in: " + _gameManager.TimeTillNextRound.ToString("0.0");

            if (s != null)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(TextureManager.GetFont("big"), s, new Vector2(0, 300), Color.White);
                spriteBatch.End();
            }

            var alphaToUse = 1.0f;

            var uiRect = new Rectangle(128, 48, (int)GameDriver.DisplayResolution.X - 128 - 100, (int)GameDriver.DisplayResolution.Y - 48);

            if (!_gameManager.Players.All(p => uiRect.Contains((int)p.Position.X, (int)p.Position.Y)) || !uiRect.Contains((int)_gameManager.Guardian.Position.X, (int)_gameManager.Guardian.Position.Y) || !_gameManager.Props.All(p => uiRect.Contains((int)p.Position.X, (int)p.Position.Y)))
            {
                //alphaToUse = 0.25f;
            }

            DrawUI(spriteBatch, alphaToUse);
        }

        private void DrawUI(SpriteBatch spriteBatch, float alpha = 1)
        {
            var allPlayers = _gameManager.Players.Union(_gameManager.DeadPlayers.Keys).OrderBy(p => p.ControllerIndex).ToList();

            for (int i = 0; i < allPlayers.Count(); i++)
            {
                var player = allPlayers[i];

                DrawPlayerInformation(spriteBatch, new Vector2(66 + 360 * i, GameDriver.DisplayResolution.Y - 64), player, alpha);
            }

            DrawEnemyInformation(spriteBatch, 
                new Vector2(_gameManager.Guardian.Position.X - 16, _gameManager.Guardian.Position.Y - 32), alpha);
        }

        private void DrawEnemyInformation(SpriteBatch spriteBatch, Vector2 topLeft, float alpha)
        {
            spriteBatch.Begin();
            //DrawingHelper.DrawHorizontalFilledBar(topLeft, spriteBatch, Color.White * alpha, Color.Blue * alpha, 32, 8, 1, _gameManager.Guardian.EnergyRemaining / GameVariables.EnemyAttackMaxRadius);
            spriteBatch.End();
        }

        private void DrawPlayerInformation(SpriteBatch spriteBatch, Vector2 center, Player player, float alpha)
        {
            spriteBatch.Begin();

            var str = "player" + (player.IsAlive ? "" + player.PlayerSpriteIndex : "_dead") + "_portrait";
            spriteBatch.Draw(TextureManager.GetTexture("player_portrait"), center, null, Color.White * alpha, 0.0f, TextureManager.GetOrigin("player_portrait"), GameVariables.UIScale, SpriteEffects.None, 0);
            spriteBatch.Draw(TextureManager.GetTexture(str), center, null, Color.White * alpha, 0.0f, TextureManager.GetOrigin(str), GameVariables.UIScale, SpriteEffects.None, 0);

            for (int i = 0; i < player.CrystalCount; i++)
            {
                spriteBatch.Draw(TextureManager.GetTexture("crystal"), center + new Vector2(32 + 16 * i, 0), null, Color.White * alpha, 0.0f, TextureManager.GetOrigin("crystal"), GameVariables.UIScale * 2, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        private void DrawFullscreenQuad(Texture2D tex, SpriteBatch sb, bool useScreenShake = false)
        {
            sb.Draw(tex, new Rectangle(useScreenShake ? (int)(GameDriver.RandomGen.NextDouble() * GameVariables.ScreenShakeAmount) : 0, useScreenShake ? (int)(GameDriver.RandomGen.NextDouble() * GameVariables.ScreenShakeAmount) : 0, (int)GameDriver.DisplayResolution.X, (int)GameDriver.DisplayResolution.Y), Color.White);
        }
    }
}
