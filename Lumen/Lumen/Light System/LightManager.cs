using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Light_System
{
    internal class LightManager
    {
        private RenderTarget2D _accumulatorRt;
        private Effect _lightAccumulatorFx, _lightCombinerFx;
        private Texture2D _screenTex;

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content, int width, int height)
        {
            _lightAccumulatorFx = content.Load<Effect>("Shaders/LightDrawer");
            _lightCombinerFx = content.Load<Effect>("Shaders/Combiner");

            _accumulatorRt = new RenderTarget2D(graphicsDevice, width, height);
            _screenTex = new Texture2D(graphicsDevice, width, height);
        }

        private void AccumulateLights(IEnumerable<ILightProvider> lights, SpriteBatch sb, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_accumulatorRt);
            graphicsDevice.Clear(Color.Black);


            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None,
                     RasterizerState.CullCounterClockwise, _lightAccumulatorFx, GameVariables.CameraZoomMatrix);
            foreach (ILightProvider light in lights) {
                if (!light.IsVisible) {
                    continue;
                }

                var normalizedPosition = new Vector2(light.Position.X/_accumulatorRt.Width,
                                                     light.Position.Y/_accumulatorRt.Height);

                _lightAccumulatorFx.Parameters["lightPosition"].SetValue(normalizedPosition);
                _lightAccumulatorFx.Parameters["lightRadius"].SetValue(light.LightRadius);
                _lightAccumulatorFx.Parameters["lightIntensity"].SetValue(light.LightIntensity);

                sb.Draw(_screenTex, new Rectangle(0, 0, _accumulatorRt.Width, _accumulatorRt.Height), Color.White);
            }
            sb.End();

            graphicsDevice.SetRenderTarget(null);
        }

        public void DrawLightDarkness(GraphicsDevice graphicsDevice, SpriteBatch sb, RenderTarget2D sceneRt)
        {
            graphicsDevice.SetRenderTarget(null);

            sb.Begin(SpriteSortMode.Immediate, null, null, null, null, _lightCombinerFx);

            sb.Draw(_accumulatorRt, new Rectangle(0, 0, _accumulatorRt.Width, _accumulatorRt.Height), Color.White);

            sb.End();
        }

        public void DrawScene(IEnumerable<ILightProvider> lights, GraphicsDevice graphicsDevice, SpriteBatch sb)
        {
            AccumulateLights(lights, sb, graphicsDevice);
        }
    }
}