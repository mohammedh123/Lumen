using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lumen.Entities;
using Lumen.Light_System;
using Lumen.Particle_System;
using Lumen.Props;
using Lumen.State_Management;
using Lumen.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen
{
    public class GameDriver : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        public static Random RandomGen;

        public static readonly Vector2 DisplayResolution = new Vector2(1024,768);

        public GameDriver()
        {
            Exiting += Cleanup;

            StateManager.Instance.Game = this;

            RandomGen = new Random();
            _graphics = new GraphicsDeviceManager(this);
            var graphicsOptions = new GraphicsOptions();
            graphicsOptions.ApplySettings(_graphics);


            Content.RootDirectory = "Content";
        }

#if DEBUG
        public static Vector2 GetPointWithinRect(Rectangle rect)
        {
            var g = rect.Width > rect.Height ? rect.Height : rect.Width;
            var a = rect.Width;
            var b = rect.Height;
            int am, bm;

            if(a > b) {
                bm = g;
                am = (int)Math.Floor((double)g*a/b);
            }
            else
            {
                am = g;
                bm = (int)Math.Floor((double)g * b / a);
            }

            var av = a*RandomGen.Next(0, am)/(float)am;
            var bv = b*RandomGen.Next(0, bm)/(float) bm;

            return new Vector2(rect.X + av, rect.Y + bv);
        }
#endif
        public static Vector2 GetFontPositionAtCenter(string str, SpriteFont font, Vector2 goalCenter)
        {
            var x = font.MeasureString(str);

            return goalCenter - x*0.5f;
        }

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
            LoadParticleSystems();

            StateManager.Instance.PushState(new MainMenuState());
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
                NumberOfParticlesPerSecond = 400.0f,
                ParticleAngle = 0.1f,
                ParticleAngleSpread = MathHelper.TwoPi,
                ParticleAngularVelocityMin = 0.0f,
                ParticleAngularVelocityMax = MathHelper.TwoPi,
                ParticleColorStart = Color.Cyan,
                ParticleColorEnd = Color.White,
                ParticleColorVariation = 0.5f,
                ParticleLifetimeMin = 0.2f,
                ParticleLifetimeMax = 0.5f,
                ParticleScaleMin = 0.7f,
                ParticleScaleMax = 1.2f,
                ParticleVelocityMin = 50.0f,
                ParticleVelocityMax = 170.0f,
                Texture = TextureManager.GetTexture("hit_particle"),
                TextureOrigin = new Vector2(1, 1),
                TextureRect = new Rectangle(0, 0, 2, 2)
            };
            var playerHitParticleSystem = new ParticleSystem(playerHitParticleSystemInfo);

            var guardianAttackParticleSystemInfo = new ParticleSystemInfo
            {
                FiringDuration = 0.2f,
                NumberOfParticlesPerSecond = 600.0f,
                ParticleAngle = 0.0f,
                ParticleAngleSpread = MathHelper.TwoPi,
                ParticleAngularVelocityMin = 0.0f,
                ParticleAngularVelocityMax = 0.0f,
                ParticleColorStart = Color.LightPink,
                ParticleColorEnd = Color.MistyRose,
                ParticleColorVariation = 0.0f,
                ParticleLifetimeMin = 0.3f,
                ParticleLifetimeMax = 0.5f,
                ParticleScaleMin = 0.8f,
                ParticleScaleMax = 1.3f,
                ParticleVelocityMin = 250.0f,
                ParticleVelocityMax = 300.0f,
                Texture = TextureManager.GetTexture("hit_particle"),
                TextureOrigin = new Vector2(1, 1),
                TextureRect = new Rectangle(0, 0, 2, 2)
            };
            var guardianAttackParticleSystem = new ParticleSystem(guardianAttackParticleSystemInfo);

            psm.RegisterParticleSystem("sample", sampleParticleSystem);
            psm.RegisterParticleSystem("player_hit", playerHitParticleSystem);
            psm.RegisterParticleSystem("guardian_attack", guardianAttackParticleSystem);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.BeginUpdate();
            StateManager.Instance.Update(gameTime);
            InputManager.EndUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            StateManager.Instance.Draw(_spriteBatch, GraphicsDevice);

            base.Draw(gameTime);
        }

        public static void DrawFullscreenQuad(Texture2D tex, SpriteBatch sb, bool useScreenShake = false)
        {
            sb.Draw(tex, new Rectangle(useScreenShake ? (int)(RandomGen.NextDouble() * GameVariables.ScreenShakeAmount) : 0, useScreenShake ? (int)(RandomGen.NextDouble() * GameVariables.ScreenShakeAmount) : 0, (int)DisplayResolution.X, (int)DisplayResolution.Y), Color.White);
        }
    }
}