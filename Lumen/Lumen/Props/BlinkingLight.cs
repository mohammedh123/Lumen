using System;
using Lumen.Entities;
using Microsoft.Xna.Framework;

namespace Lumen.Props
{
    class BlinkingLight : Light
    {
        private enum BlinkingLightFadeState
        {
            None,
            FadingIn,
            FadingOut
        }

        public Entity Owner = null;

        private BlinkingLightFadeState _fadeState = BlinkingLightFadeState.None;
        private float _timer, _durationTimer, _fadeTimer=-1.0f;
        private readonly float _lightRadius;

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
            get { return _fadeTimer >= GameVariables.BlinkingFadeInDuration; }
        }

        private bool IsDoneFadingOut
        {
            get { return _fadeTimer >= GameVariables.BlinkingFadeOutDuration; }
        }

        public BlinkingLight(string textureKeyName, Entity owner, float lightRadius) : base(textureKeyName, owner.Position, 0)
        {
            Owner = owner;
            IsVisible = false;
            LightRadius = 0;
            _lightRadius = lightRadius;
        }

        public override void Update(float dt)
        {
            Position = Owner.Position;
            
            if(CanBeginBlink) //begin fading in
            {
                _timer = GameVariables.BlinkingPeriod;
                _durationTimer = GameVariables.BlinkingDuration;
                _fadeTimer = 0.0f;
                _fadeState = BlinkingLightFadeState.FadingIn;
            }
            else if(_fadeState == BlinkingLightFadeState.FadingIn)
            {
                LightRadius = _lightRadius * MathHelper.SmoothStep(0.0f, 1.0f, _fadeTimer / GameVariables.BlinkingFadeInDuration);

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.BlinkingFadeInDuration);

                if(IsDoneFadingIn)
                {
                    _fadeState = BlinkingLightFadeState.None;
                    _fadeTimer = 0.0f;
                }
            }
            else if(_fadeState == BlinkingLightFadeState.None && IsLightOn)
            {
                LightRadius = IsLightOn ? _lightRadius : 0;

                _durationTimer = Math.Max(_durationTimer - dt, 0);

                if(!IsLightOn)
                {
                    _fadeState = BlinkingLightFadeState.FadingOut;
                    _fadeTimer = 0.0f;
                }

            }
            else if(_fadeState == BlinkingLightFadeState.FadingOut)
            {
                LightRadius = _lightRadius*MathHelper.SmoothStep(1.0f, 0.0f, _fadeTimer / GameVariables.BlinkingFadeOutDuration);

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.BlinkingFadeOutDuration);

                if (IsDoneFadingOut)
                {
                    _fadeState = BlinkingLightFadeState.None;
                    _fadeTimer = -1.0f;
                }
            }

            _timer = Math.Max(_timer - dt, 0.0f);
        }
    }
}
