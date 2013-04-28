using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lumen.Entities;
using Lumen.Light_System;
using Lumen.Particle_System;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen
{
    public class GameDriver : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameManager _gameManager;
        private LightManager _lightManager;

        private RenderTarget2D _sceneRT;
        private const int MaxPlayers = 4;

        public static Random RandomGen;

        public static readonly Vector2 DisplayResolution = new Vector2(1024,768);

#if DEBUG
        public bool IsShowingDebugInformation = false;
#endif

        public GameDriver()
        {
            Exiting += Cleanup;

            RandomGen = new Random();
            _graphics = new GraphicsDeviceManager(this);
            var graphicsOptions = new GraphicsOptions();
            graphicsOptions.ApplySettings(_graphics);

            _gameManager = new GameManager(DisplayResolution);
            _lightManager = new LightManager();

            Content.RootDirectory = "Content";
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

                        try {
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
                            else if(variableName == "PlayerLightDuration")
                            {
                                foreach(var light in _gameManager.Props.Where(p => p is Light))
                                {
                                    (light as Light).LightRadius = (float) actualVariableValue;
                                }
                            }
                        }
                        catch (Exception e) {
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

        private void ResetRound(bool shufflePlayers = true, bool reloadVariables = true)
        {
            if(reloadVariables)
                LoadVariables();

            if (shufflePlayers) {
                _gameManager.ResetCompletely();

                var randomEnemyIdx = RandomGen.Next(1,1);

                var playerNum = 1;
                for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++) {
                    if (GamePad.GetState(i).IsConnected || i <= PlayerIndex.Two) {

                        if (i == PlayerIndex.One + randomEnemyIdx) {
                            _gameManager.AddEnemy(
                                new Guardian(new Vector2(DisplayResolution.X - 64, DisplayResolution.Y/2)),
                                i);
                        }
                        else {
                            _gameManager.AddPlayer(
                                new Player("player" + playerNum++,
                                           new Vector2(64, DisplayResolution.Y/2 - 96 + 32*playerNum)),
                                i);
                            _gameManager.Players.Last().PlayerSpriteIndex = playerNum - 1;
                        }
                    }
                }
            }

            for (var i = 0; i < GameVariables.CrystalsToSpawn(_gameManager.RoundNumber); i++)
            {
                _gameManager.AddCrystal(new Vector2(RandomGen.Next(16, (int)DisplayResolution.X), RandomGen.Next(16, (int)DisplayResolution.Y)));
            }
        }
#endif

        protected override void Initialize()
        {
            base.Initialize();
        }

        private void Cleanup(object sender, EventArgs e)
        {
            for(var gp = PlayerIndex.One; gp <= PlayerIndex.Four; gp++) {
                GamePad.SetVibration(gp, 0, 0);
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadContent(Content);
            SoundManager.LoadContent(Content);
            _lightManager.LoadContent(_graphics, GraphicsDevice, Content);
            LoadParticleSystems();
            
#if DEBUG
            LoadVariables();
#endif
            ResetRound();

            _sceneRT = new RenderTarget2D(GraphicsDevice, (int)DisplayResolution.X, (int)DisplayResolution.Y);
        }

        private void LoadParticleSystems()
        {
            var psm = ParticleSystemManager.Instance;

            var sampleParticleSystemInfo = new ParticleSystemInfo
            {
                FiringDuration = 1.0f,
                NumberOfParticlesPerSecond = 1.0f,
                ParticleAngle = 0.0f,
                ParticleAngleSpread = MathHelper.TwoPi,
                ParticleAngularVelocityMin = 0.0f,
                ParticleAngularVelocityMax = 0.0f,
                ParticleColorStart = Color.Red,
                ParticleColorEnd = Color.Orange,
                ParticleColorVariation = 0.5f,
                ParticleLifetimeMin = 1.0f,
                ParticleLifetimeMax = 2.0f,
                ParticleScaleMin = 0.5f,
                ParticleScaleMax = 1.0f,
                ParticleVelocityMin = 50.0f,
                ParticleVelocityMax = 150.0f,
                Texture = TextureManager.GetTexture("guardian"),
                TextureOrigin = new Vector2(32, 32),
                TextureRect = new Rectangle(0, 0, 64, 64)
            };
            var sampleParticleSystem = new ParticleSystem(sampleParticleSystemInfo);

            var playerHitParticleSystemInfo = new ParticleSystemInfo
            {
                FiringDuration = 0.2f,
                NumberOfParticlesPerSecond = 100.0f,
                ParticleAngle = 0.0f,
                ParticleAngleSpread = MathHelper.TwoPi,
                ParticleAngularVelocityMin = 0.0f,
                ParticleAngularVelocityMax = MathHelper.TwoPi,
                ParticleColorStart = Color.Cyan,
                ParticleColorEnd = Color.White,
                ParticleColorVariation = 0.5f,
                ParticleLifetimeMin = 0.1f,
                ParticleLifetimeMax = 0.3f,
                ParticleScaleMin = 0.5f,
                ParticleScaleMax = 1.0f,
                ParticleVelocityMin = 50.0f,
                ParticleVelocityMax = 150.0f,
                Texture = TextureManager.GetTexture("hit_particle"),
                TextureOrigin = new Vector2(1, 1),
                TextureRect = new Rectangle(0, 0, 2, 2)
            };
            var playerHitParticleSystem = new ParticleSystem(playerHitParticleSystemInfo);

            psm.RegisterParticleSystem("sample", sampleParticleSystem);
            psm.RegisterParticleSystem("player_hit", playerHitParticleSystem);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.Back))
                LoadVariables();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                ResetRound();

            var scale = 1.0f + Mouse.GetState().ScrollWheelValue/12000.0f;
            GameVariables.CameraZoom = scale;

            if(Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                IsShowingDebugInformation = true;
            }
            else
            {
                IsShowingDebugInformation = false;
            }
#endif

            InputManager.BeginUpdate();
            _gameManager.Update(gameTime);
            InputManager.EndUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_sceneRT);
            _gameManager.DrawScene(_spriteBatch);
             
            _lightManager.DrawScene(_gameManager.GetLights(), GraphicsDevice, _spriteBatch);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
            DrawFullscreenQuad(_sceneRT, _spriteBatch, true);
            _spriteBatch.End();
            _lightManager.DrawLightDarkness(GraphicsDevice, _spriteBatch, _sceneRT);

#if DEBUG
            if(IsShowingDebugInformation)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(TextureManager.GetFont("debug"), String.Format("{0}", GameVariables.CameraZoom), Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
#endif
            string s = null;
            if (_gameManager.State == GameState.PlayersWin)
                s = "Round won!\nNext round in: "+_gameManager.TimeTillNextRound.ToString("0.0");
            else if (_gameManager.State == GameState.EnemyWins)
                s = "The guardian has won.\n(R) Retry?\n(T) Restart?";

            if (s != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(TextureManager.GetFont("big"), s, new Vector2(0, 300), Color.White);
                _spriteBatch.End();
            }

            var alphaToUse = 1.0f;

            var uiRect = new Rectangle(128,48,(int)DisplayResolution.X-128-100,(int)DisplayResolution.Y-48);
            
            if(!_gameManager.Players.All(p => uiRect.Contains((int)p.Position.X, (int)p.Position.Y)) || !uiRect.Contains((int)_gameManager.Guardian.Position.X, (int)_gameManager.Guardian.Position.Y) || !_gameManager.Props.All(p => uiRect.Contains((int)p.Position.X, (int)p.Position.Y))) {
                //alphaToUse = 0.25f;
            }

            DrawUI(alphaToUse);
            
            base.Draw(gameTime);
        }

        private void DrawUI(float alpha=1)
        {
            var allPlayers = _gameManager.Players.Union(_gameManager.DeadPlayers.Keys).OrderBy(p => p.ControllerIndex).ToList();

            for (int i = 0; i < allPlayers.Count(); i++) {
                var player = allPlayers[i];

                DrawPlayerInformation(new Vector2(66+360*i, DisplayResolution.Y-64), player, alpha);
            }

            DrawEnemyInformation(
                new Vector2(_gameManager.Guardian.Position.X - 16, _gameManager.Guardian.Position.Y - 32), alpha);
        }

        private void DrawEnemyInformation(Vector2 topLeft, float alpha)
        {
            _spriteBatch.Begin();
            DrawingHelper.DrawHorizontalFilledBar(topLeft, _spriteBatch, Color.White*alpha, Color.Blue*alpha, 32, 8, 1, _gameManager.Guardian.EnergyRemaining/GameVariables.EnemyAttackMaxRadius);
            _spriteBatch.End();
        }

        private void DrawPlayerInformation(Vector2 center, Player player, float alpha)
        {
            _spriteBatch.Begin();

            var str = "player" + (player.IsAlive ? "" + player.PlayerSpriteIndex : "_dead") + "_portrait";
            _spriteBatch.Draw(TextureManager.GetTexture("player_portrait"), center, null, Color.White * alpha, 0.0f, TextureManager.GetOrigin("player_portrait"), GameVariables.UIScale, SpriteEffects.None, 0);
            _spriteBatch.Draw(TextureManager.GetTexture(str), center, null, Color.White * alpha, 0.0f, TextureManager.GetOrigin(str), GameVariables.UIScale, SpriteEffects.None, 0);
            for (int i = 0; i < player.CrystalCount; i++)
            {
                _spriteBatch.Draw(TextureManager.GetTexture("crystal"), center + new Vector2(32 + 16 * i, 0), null, Color.White * alpha, 0.0f, TextureManager.GetOrigin("crystal"), GameVariables.UIScale*2, SpriteEffects.None, 0);
            }

            _spriteBatch.End();
        }

        private void DrawFullscreenQuad(Texture2D tex, SpriteBatch sb, bool useScreenShake=false)
        {
            sb.Draw(tex, new Rectangle(useScreenShake ? (int)(RandomGen.NextDouble() * GameVariables.ScreenShakeAmount) : 0, useScreenShake ? (int)(RandomGen.NextDouble() * GameVariables.ScreenShakeAmount) : 0, (int)DisplayResolution.X, (int)DisplayResolution.Y), Color.White);
        }
    }
}