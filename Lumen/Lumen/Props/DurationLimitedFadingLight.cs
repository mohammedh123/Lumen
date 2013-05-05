using System;
using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    internal class DurationLimitedFadingLight : Light
    {
        private float _durationTimer;
        private BlinkingLightFadeState _fadeState = BlinkingLightFadeState.None;
        private float _fadeTimer = -1.0f;

        public DurationLimitedFadingLight(string textureKeyName, Entity owner, float lightRadius)
            : base(textureKeyName, lightRadius, owner.Position, owner)
        {
            IsVisible = true;
            LightIntensity = 0.0f;
        }

        private bool IsLightOn
        {
            get { return _durationTimer > 0.0f; }
        }

        private bool IsDoneFadingIn
        {
            get { return _fadeTimer >= GameVariables.PlayerLightFadeInDuration; }
        }

        private bool IsDoneFadingOut
        {
            get { return _fadeTimer >= GameVariables.PlayerLightFadeOutDuration; }
        }

        public void TurnOn()
        {
            if (_fadeState == BlinkingLightFadeState.None) {
                _fadeTimer = 0.0f;
            }
            else if (_fadeState == BlinkingLightFadeState.FadingOut) {
                _fadeTimer = GameVariables.PlayerLightFadeInDuration - _fadeTimer;
            }

            if (_fadeState == BlinkingLightFadeState.None && IsLightOn) {
                _durationTimer = GameVariables.PlayerLightDuration;
            }

            if (_fadeState == BlinkingLightFadeState.FadingOut ||
                (_fadeState == BlinkingLightFadeState.None && !IsLightOn)) {
                _fadeState = BlinkingLightFadeState.FadingIn;
            }
        }

        public override void Update(float dt)
        {
            Position = EntityAttachedTo.Position;
            if (_fadeState == BlinkingLightFadeState.FadingIn) {
                IsVisible = true;
                LightIntensity = MathHelper.SmoothStep(0.0f, 1.0f, _fadeTimer/(GameVariables.PlayerLightFadeInDuration));

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.PlayerLightFadeInDuration);

                if (IsDoneFadingIn) {
                    _fadeState = BlinkingLightFadeState.None;
                    _fadeTimer = 0.0f;
                    _durationTimer = GameVariables.PlayerLightDuration;
                }
            }
            else if (_fadeState == BlinkingLightFadeState.None && IsLightOn) {
                LightIntensity = IsLightOn ? 1.0f : 0;
                IsVisible = IsLightOn;

                _durationTimer = Math.Max(_durationTimer - dt, 0);

                if (!IsLightOn) {
                    _fadeState = BlinkingLightFadeState.FadingOut;
                    _fadeTimer = 0.0f;
                }
            }
            else if (_fadeState == BlinkingLightFadeState.FadingOut) {
                IsVisible = true;
                LightIntensity = MathHelper.SmoothStep(1.0f, 0.0f, _fadeTimer/(GameVariables.PlayerLightFadeOutDuration));

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.PlayerLightFadeOutDuration);

                if (IsDoneFadingOut) {
                    _fadeState = BlinkingLightFadeState.None;
                    _fadeTimer = -1.0f;
                    LightIntensity = 0;
                    IsVisible = false;
                }
            }
        }
    }
}