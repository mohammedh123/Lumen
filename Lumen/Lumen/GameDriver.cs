using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lumen.Entities;
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
                        actualVariable.SetValue(null, Convert.ChangeType(variableValue, actualVariable.FieldType));
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

            for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++ )
            {
                if (GamePad.GetState(i).IsConnected)
                    _gameManager.AddPlayer(new Player("player", new Vector2(100, 50)), i);
            }

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
            if(Keyboard.GetState().IsKeyDown(Keys.Back))
                LoadVariables();
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

            _lightManager.DrawScene(_gameManager.Props.Where(p => p.PropType == PropTypeEnum.Candle).Cast<Candle>(), GraphicsDevice, _spriteBatch);

            _spriteBatch.Begin();
            DrawFullscreenQuad(_sceneRT, _spriteBatch);
            _spriteBatch.End();
            _lightManager.DrawLightDarkness(GraphicsDevice, _spriteBatch, _sceneRT);

            base.Draw(gameTime);
        }

        private void DrawFullscreenQuad(Texture2D tex, SpriteBatch sb)
        {
            sb.Draw(tex, new Rectangle(0, 0, (int)DisplayResolution.X, (int)DisplayResolution.Y), Color.White);
        }
    }
}