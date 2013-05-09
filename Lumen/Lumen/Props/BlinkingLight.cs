using System;
using Lumen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Props
{
    internal class BlinkingLight : Light
    {
        private readonly float _lightRadius;
        private float _durationTimer;
        private BlinkingLightFadeState _fadeState = BlinkingLightFadeState.None;
        private float _fadeTimer = -1.0f;
        private float _frequency = 1.0f;
        private float _timer;

        public float Duration;
        
        public BlinkingLight(string textureKeyName, Entity owner, float lightRadius)
            : base(textureKeyName, lightRadius, owner.Position, owner)
        {
            var invFreq = (1.0f / _frequency);
            Duration = GameVariables.BlinkingDuration*invFreq;
            _timer = (float) GameDriver.RandomGen.NextDouble()*0.25f;
            LightRadius = 0;
            LightIntensity = 1.0f;
            _lightRadius = lightRadius;
        }

        private bool CanBeginBlink
        {
            get { return _timer == 0.0f; }
        }

        private bool IsLightOn
        {
            get { return _durationTimer > 0.0f; }
        }

        private bool IsDoneFadingIn
        {
            get { return _fadeTimer >= GameVariables.BlinkingFadeInDuration*(1/_frequency); }
        }

        private bool IsDoneFadingOut
        {
            get { return _fadeTimer >= GameVariables.BlinkingFadeOutDuration*(1/_frequency); }
        }

        public void IncreaseFrequency(float factor)
        {
            _frequency *= factor;
            _frequency = Math.Min(_frequency, GameVariables.BlinkingMaxFrequency);
        }

        public void ResetFrequency()
        {
            _frequency = 1.0f;
        }

        public override void Draw(SpriteBatch sb)
        {
        }

        public override void Update(float dt)
        {
            Position = EntityAttachedTo.Position;
            var invFreq = (1.0f/_frequency);
            if (CanBeginBlink) //begin fading in
            {
                _timer = GameVariables.BlinkingPeriod*invFreq;
                _durationTimer = Duration*invFreq;
                _fadeTimer = 0.0f;
                _fadeState = BlinkingLightFadeState.FadingIn;
            }
            else if (_fadeState == BlinkingLightFadeState.FadingIn) {
                LightRadius = _lightRadius*
                              MathHelper.SmoothStep(0.0f, 1.0f,
                                                    _fadeTimer/(GameVariables.BlinkingFadeInDuration*invFreq));

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.BlinkingFadeInDuration*invFreq);

                if (IsDoneFadingIn) {
                    _fadeState = BlinkingLightFadeState.None;
                    _fadeTimer = 0.0f;
                }
            }
            else if (_fadeState == BlinkingLightFadeState.None && IsLightOn) {
                LightRadius = IsLightOn ? _lightRadius : 0;

                _durationTimer = Math.Max(_durationTimer - dt, 0);

                if (!IsLightOn) {
                    _fadeState = BlinkingLightFadeState.FadingOut;
                    _fadeTimer = 0.0f;
                }
            }
            else if (_fadeState == BlinkingLightFadeState.FadingOut) {
                LightRadius = _lightRadius*
                              MathHelper.SmoothStep(1.0f, 0.0f,
                                                    _fadeTimer/(GameVariables.BlinkingFadeOutDuration*invFreq));

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.BlinkingFadeOutDuration);

                if (IsDoneFadingOut) {
                    _fadeState = BlinkingLightFadeState.None;
                    _fadeTimer = -1.0f;
                    LightRadius = 0;
                }
            }

            _timer = Math.Max(_timer - dt, 0.0f);
        }
    }
}