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

        public static readonly Vector2 DisplayResolution = new Vector2(1024,768);

#if DEBUG
        public bool IsShowingCoinCount = false;
#endif

        public GameDriver()
        {
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
#endif

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadContent(Content);
            _lightManager.LoadContent(_graphics, GraphicsDevice, Content);

            ResetLevel();

            //for (var x = 16.0f; x < DisplayResolution.X; x += 32.0f)
            //{
            //    for (var y = 16.0f; y < DisplayResolution.Y; y += 32.0f) {
            //        _gameManager.AddCoin(new Vector2(x, y));
            //    }
            //}


            _sceneRT = new RenderTarget2D(GraphicsDevice, (int)DisplayResolution.X, (int)DisplayResolution.Y);
        }

        private void ResetLevel()
        {
#if DEBUG
            LoadVariables();
#endif
            _gameManager.Clear();
            _gameManager.GameState = GameState.Playing;

            int size = 32;

            var tex = Content.Load<Texture2D>("level");
            var arr = new Color[tex.Width * tex.Height];

            tex.GetData(arr);

            for (int i = 0; i < tex.Width; i++)
                for (int j = 0; j < tex.Height; j++)
                {
                    if (arr[j * tex.Width + i] == Color.Black)
                    {
                        _gameManager.AddBlock(new Vector2(i * size, j * size), size);
                    }
                    else if (arr[j * tex.Width + i] == Color.Red)
                    {
                        _gameManager.AddGoalArea(new Vector2(i * size, j * size), size);
                    }
                }

            var rand = new Random();
            for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                if (GamePad.GetState(i).IsConnected || i == PlayerIndex.Two)
                {
                    Rectangle rect;

                    while (true)
                    {
                        var randomBlockX = rand.Next(1, (int)DisplayResolution.X / size - 1);
                        var randomBlockY = rand.Next(1, (int)DisplayResolution.Y / size - 1);
                        rect = new Rectangle(randomBlockX * size, randomBlockY * size, size, size);

                        var collidesAny = _gameManager.Blocks.Any(block => block.BoundingBox.Intersects(rect));

                        if (!collidesAny)
                            break;
                    }


                    _gameManager.AddPlayer(new Player("player", new Vector2(rect.X + size / 2, rect.Y + size / 2), _gameManager.World), i);

                    if (i == PlayerIndex.One)
                        _gameManager.Players.Last().Color = Color.Red;
                    if (i == PlayerIndex.Two)
                        _gameManager.Players.Last().Color = Color.Green;
                    if (i == PlayerIndex.Three)
                        _gameManager.Players.Last().Color = Color.Blue;
                    if (i == PlayerIndex.Four)
                        _gameManager.Players.Last().Color = Color.Cyan;

                    if (i == PlayerIndex.Three)
                    {
                        _gameManager.Players.Last().CanPickUpCoins = false;
                        var idx = _gameManager.Props.FindLastIndex(p => p is AttachedCandle);
                        _gameManager.Props.RemoveAt(idx);
                    }
                }
            }

            Rectangle r;
            while (true)
            {
                var randomBlockX = rand.Next(1, (int)DisplayResolution.X / size - 1);
                var randomBlockY = rand.Next(1, (int)DisplayResolution.Y / size - 1);
                r = new Rectangle(randomBlockX * size, randomBlockY * size, size, size);
                var candles = _gameManager.Props.Where(p => p is AttachedCandle).Cast<AttachedCandle>();
                var collidesAny = _gameManager.Blocks.Any(block => block.BoundingBox.Intersects(r)) || _gameManager.Players.Any(p => Collider.Collides(p, r)) || !candles.Any(p => Collider.Collides(p, r));

                if (!collidesAny)
                    break;
            }

            _gameManager.AddDrunkard(new Vector2(r.Center.X, r.Center.Y));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.BeginUpdate();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

#if DEBUG
            if(Keyboard.GetState().IsKeyDown(Keys.Back))
                LoadVariables();
            if(InputManager.KeyPressed(Keys.Enter))
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

            _gameManager.Update(gameTime);
            InputManager.EndUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_sceneRT);
            _gameManager.DrawScene(_spriteBatch);

            _lightManager.DrawScene(_gameManager.Props.Where(p => p.PropType == PropTypeEnum.Candle).Cast<Candle>(), GraphicsDevice, _spriteBatch);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
            DrawFullscreenQuad(_sceneRT, _spriteBatch);
            _spriteBatch.End();
            _lightManager.DrawLightDarkness(GraphicsDevice, _spriteBatch, _sceneRT);

#if DEBUG
            if(IsShowingCoinCount)
            {
                _spriteBatch.Begin();
                foreach(var player in _gameManager.Players)
                    _spriteBatch.DrawString(TextureManager.GetFont("debug"), String.Format("{0}", player.CoinCount), new Vector2(player.Position.X - 16, player.Position.Y - 32), Color.White);
                _spriteBatch.DrawString(TextureManager.GetFont("debug"), String.Format("{0}", GameVariables.CameraZoom), Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
#endif

            if(_gameManager.GameState != GameState.Playing) {
                _spriteBatch.Begin();
                var str =
                _gameManager.GameState == GameState.Victory ? "WON" : "LOST";

                _spriteBatch.DrawString(TextureManager.GetFont("large_font"), String.Format("YOU {0} MAN",str), new Vector2(0, DisplayResolution.Y*0.5f), Color.Yellow);
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