using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lumen.Entities;
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
        public bool IsShowingCoinCount = false;
#endif

        public GameDriver()
        {
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

                        if (String.IsNullOrEmpty(line)) continue;

                        var variableName = line.Substring(0, line.IndexOf(' ')).Trim();
                        var variableValue = line.Substring(line.IndexOf('=') + 1).Trim();

                        var actualVariable = typeof(GameVariables).GetField(variableName);
                        var actualVariableValue = Convert.ChangeType(variableValue, actualVariable.FieldType);
                        actualVariable.SetValue(null, actualVariableValue);

                        if(variableName == "PlayerLanternRadius")
                        {
                            foreach (var lantern in _gameManager.Props.Where(p => p is AttachedCandle))
                                (lantern as AttachedCandle).Radius = (float)actualVariableValue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Log("Error was encountered with exception:" + Environment.NewLine + e);
            }
        }

        private void ResetLevel()
        {
            LoadVariables();
            _gameManager.Reset();

            var randomEnemyIdx = RandomGen.Next(0, 4);
            var randomSwordWielderIdx = RandomGen.Next(0, 4);
            while(randomSwordWielderIdx == randomEnemyIdx)
                randomSwordWielderIdx = RandomGen.Next(0, 4);

            for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                if (GamePad.GetState(i).IsConnected)
                {
                    _gameManager.AddPlayer(new Player("player", new Vector2(150 + (int)i * 150, 100 + (int)i * 100)), i);

                    if (i == PlayerIndex.One + randomEnemyIdx)
                    {
                        _gameManager.MarkPlayerAsEnemy(_gameManager.Players.Last());
                    }
                    else
                    {
                        if (i == PlayerIndex.One)
                            _gameManager.Players.Last().Color = Color.Yellow;
                        if (i == PlayerIndex.Two)
                            _gameManager.Players.Last().Color = Color.Green;
                        if (i == PlayerIndex.Three)
                            _gameManager.Players.Last().Color = Color.LightBlue;
                        if (i == PlayerIndex.Four)
                            _gameManager.Players.Last().Color = Color.Cyan;

                        if(i == PlayerIndex.One + randomSwordWielderIdx)
                            _gameManager.Players.Last().Weapon = PlayerWeaponType.Sword;
                    }
                }
            }

            for (var i = 0; i < GameVariables.CoinInitialCount; i++)
            {

                _gameManager.AddCoin(new Vector2(RandomGen.Next(16, (int)DisplayResolution.X), RandomGen.Next(16, (int)DisplayResolution.Y)));
            }

            //int size = 64;
            //for (int i = 0; i <= size*((int)(DisplayResolution.X/size)); i += size)
            //{
            //    _gameManager.AddBlock(new Vector2(i, 0), size);
            //}

        }
#endif

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadContent(Content);
            SoundManager.LoadContent(Content);
            _lightManager.LoadContent(_graphics, GraphicsDevice, Content);
            
            
#if DEBUG
            LoadVariables();
#endif
            ResetLevel();

            _sceneRT = new RenderTarget2D(GraphicsDevice, (int)DisplayResolution.X, (int)DisplayResolution.Y);
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
                ResetLevel();

            var scale = 1.0f + Mouse.GetState().ScrollWheelValue/12000.0f;
            GameVariables.CameraZoom = scale;

            if(Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                IsShowingCoinCount = true;
            }
            else
            {
                IsShowingCoinCount = false;
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

            _lightManager.DrawScene(_gameManager.Props.Where(p => p.PropType == PropTypeEnum.Candle).Cast<Candle>(), _gameManager.Players.Where(p => p.IsBurning), _gameManager.Players.Where(p => p.IsAttacking), GraphicsDevice, _spriteBatch);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
            DrawFullscreenQuad(_sceneRT, _spriteBatch);
            _spriteBatch.End();
            _lightManager.DrawLightDarkness(GraphicsDevice, _spriteBatch, _sceneRT);

#if DEBUG
            if(IsShowingCoinCount)
            {
                _spriteBatch.Begin();
                foreach(var player in _gameManager.Players)
                    _spriteBatch.DrawString(TextureManager.GetFont("debug"), String.Format("{0},{1}", player.CoinCount, player.NumCandlesLeft), new Vector2(player.Position.X - 16, player.Position.Y - 32), Color.White);
                _spriteBatch.DrawString(TextureManager.GetFont("debug"), String.Format("{0}", GameVariables.CameraZoom), Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
#endif
            string s = null;
            if (_gameManager.State == GameState.PlayersWin)
                s = "THE PLAYERS WIN, MAN";
            else if (_gameManager.State == GameState.EnemyWins)
                s = "THE ENEMY WON DUDE";

            if (s != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(TextureManager.GetFont("big"), s, new Vector2(0, 300), Color.White);
                _spriteBatch.End();
            }
            
            base.Draw(gameTime);
        }

        private void DrawFullscreenQuad(Texture2D tex, SpriteBatch sb)
        {
            sb.Draw(tex, new Rectangle(0, 0, (int)DisplayResolution.X, (int)DisplayResolution.Y), Color.White);
        }
    }
}