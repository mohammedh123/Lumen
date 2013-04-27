using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Light_System
{
    class LightManager
    {
        private VertexPositionColorTexture[] _verts = { new VertexPositionColorTexture(), new VertexPositionColorTexture() };
        private RenderTarget2D _accumulatorRT;
        private Texture2D _screenTex;
        private Effect _lightAccumulatorFX, _lightCombinerFX;
        
        public void LoadContent(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _lightAccumulatorFX = content.Load<Effect>("LightDrawer");
            _lightCombinerFX = content.Load<Effect>("Combiner");

            _accumulatorRT = new RenderTarget2D(graphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _screenTex = new Texture2D(graphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        private void AccumulateLights(IEnumerable<ILightProvider> lights, SpriteBatch sb, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_accumulatorRT);
            graphicsDevice.Clear(Color.Black);


            foreach (var light in lights)
            {
                if (!light.IsVisible) continue;

                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, _lightAccumulatorFX, GameVariables.CameraZoomMatrix);
                var normalizedPosition = new Vector2(light.Position.X / _accumulatorRT.Width,
                                                     light.Position.Y / _accumulatorRT.Height);

                _lightAccumulatorFX.Parameters["lightPosition"].SetValue(normalizedPosition);
                _lightAccumulatorFX.Parameters["lightRadius"].SetValue(light.LightRadius);
                _lightAccumulatorFX.Parameters["lightIntensity"].SetValue(light.LightIntensity);

                sb.Draw(_screenTex, new Rectangle(0, 0, _accumulatorRT.Width, _accumulatorRT.Height), Color.White);
                sb.End();
            }
            
            graphicsDevice.SetRenderTarget(null);

            //if (lights.Any())
            //{
            //    using (var stream = new FileStream("output.png", FileMode.OpenOrCreate))
            //    {
            //        _accumulatorRT.SaveAsPng(stream, _accumulatorRT.Width, _accumulatorRT.Height);
            //    }
            //}
        }

        public Texture2D GetAccumulatedLights()
        {
            return _accumulatorRT;
        }

        public void DrawLightDarkness(GraphicsDevice graphicsDevice, SpriteBatch sb, RenderTarget2D sceneRt)
        {
            graphicsDevice.SetRenderTarget(null);

            sb.Begin(SpriteSortMode.Immediate, null, null, null, null, _lightCombinerFX);

            sb.Draw(_accumulatorRT, new Rectangle(0, 0, _accumulatorRT.Width, _accumulatorRT.Height), Color.White);

            sb.End();
        }

        public void DrawScene(IEnumerable<ILightProvider> lights, GraphicsDevice graphicsDevice, SpriteBatch sb)
        {
            AccumulateLights(lights, sb, graphicsDevice);
        }
    }
}
