using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Entities;
using Lumen.Light_System;
using Microsoft.Xna.Framework;
using Lumen.Particle_System;

namespace Lumen.Props
{
    internal class Crystal : Prop, ILightProvider
    {
        private enum FadingLightState
        {
            FadingIn,
            FadingOut,
            On,
            Off
        }

        private FadingLightState _fadeState = FadingLightState.Off;

        private readonly List<Player> _collectors = new List<Player>();
        public Player Collector;

        private float _fadeRadius = 0.0f;
        private float _collectionTimeLeft = GameVariables.CrystalCollectionTime;
        private float _fadeTimer = -1.0f;

        private bool IsDoneFadingIn
        {
            get { return _fadeTimer >= GameVariables.CrystalFadeInDuration; }
        }

        private bool IsDoneFadingOut
        {
            get { return _fadeTimer >= GameVariables.CrystalFadeOutDuration; }
        }

        public void TurnOn()
        {
            if (_fadeState == FadingLightState.Off)
            {
                _fadeTimer = 0.0f;
            }
            else if (_fadeState == FadingLightState.FadingOut)
            {
                _fadeTimer = GameVariables.CrystalFadeInDuration - _fadeTimer;
            }

            if (_fadeState == FadingLightState.FadingOut || _fadeState == FadingLightState.Off)
            {
                _fadeState = FadingLightState.FadingIn;
            }
        }

        public void TurnOff()
        {
            if (_fadeState == FadingLightState.FadingIn)
            {
                _fadeTimer = GameVariables.CrystalFadeOutDuration - _fadeTimer;
            }

            if (_fadeState == FadingLightState.FadingIn || _fadeState == FadingLightState.On)
            {
                _fadeState = FadingLightState.FadingOut;
            }
        }

        public void AbruptlyTurnOff()
        {
            _fadeTimer = -1.0f;
            _fadeState = FadingLightState.Off;
            IsVisible = true;
            LightIntensity = 0.0f;
        }

        private int _collectorCount;

        public Crystal(Vector2 position)
            : base("crystal", position)
        {
            IsVisible = true;
            LightIntensity = 0.0f;
            PropType = PropTypeEnum.Crystal;

            Health = GameVariables.CrystalHarvestRequirement;
        }

        public bool IsSomeoneCollectingThis
        {
            get { return _collectorCount > 0; }
        }

        public override bool CanCollide
        {
            get { return false; }
        }

        #region ILightProvider Members

        public Color LightColor { get; set; }

        public float LightRadius
        {
            get
            {
                if (_fadeState == FadingLightState.FadingOut)
                    return _fadeRadius;

                if (!IsSomeoneCollectingThis) {
                    return 0;
                }

                return (1 - (_collectionTimeLeft/GameVariables.CrystalCollectionTime))*
                       GameVariables.CrystalGlowRadius;
            }
            set { }
        }

        public float LightIntensity { get; set; }

        #endregion

        public void DecrementCollectorCount(Player p)
        {
            if (_collectorCount == 1) {
                _fadeRadius = (1 - (_collectionTimeLeft / GameVariables.CrystalCollectionTime)) *
                       GameVariables.CrystalGlowRadius;
                TurnOff();   
            }
            _collectorCount = Math.Max(_collectorCount - 1, 0);
            _collectors.Remove(p);
        }

        public void IncrementCollectorCount(Player p)
        {
            _collectorCount++;
            _collectors.Add(p);
            if(_collectorCount == 1)
                TurnOn();
        }

        public override void OnCollide(Entity collider)
        {
            var player = collider as Player;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (IsSomeoneCollectingThis) {
                SoundManager.GetSoundInstance("crystal_charge").Play();
                ParticleSystemManager.Instance.FireParticleSystem("crystal_charge", Position.X, Position.Y);

                _collectionTimeLeft -= dt*_collectorCount;

                if (_collectionTimeLeft <= 0) {
                    DecrementCount();
                    Collector = _collectors.First();
                    _collectors.ForEach(p => p.ResetCollecting());
                }
            }

            UpdateLightProperties(dt);
        }

        private void UpdateLightProperties(float dt)
        {
            if (_fadeState == FadingLightState.FadingIn)
            {
                LightIntensity = MathHelper.SmoothStep(0.0f, 1.0f, _fadeTimer / (GameVariables.CrystalFadeInDuration));

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.CrystalFadeInDuration);

                if (IsDoneFadingIn)
                {
                    _fadeState = FadingLightState.On;
                    _fadeTimer = 0.0f;
                }
            }
            else if (_fadeState == FadingLightState.On)
            {
                LightIntensity = 1.0f;
            }
            else if (_fadeState == FadingLightState.FadingOut)
            {
                IsVisible = true;
                LightIntensity = MathHelper.SmoothStep(1.0f, 0.0f, _fadeTimer / (GameVariables.CrystalFadeOutDuration));

                _fadeTimer = Math.Min(_fadeTimer + dt, GameVariables.CrystalFadeOutDuration);

                if (IsDoneFadingOut)
                {
                    _fadeState = FadingLightState.Off;
                    _fadeTimer = -1.0f;
                    LightIntensity = 0;
                }
            }
            else if (_fadeState == FadingLightState.Off)
            {
                LightIntensity = 0.0f;
            }
        }

        public void ResetCollectionTimeLeft(int numPlayersRemaining, int roundNumber)
        {
            switch(numPlayersRemaining) {
                case 1:
                    _collectionTimeLeft = GameVariables.OnePlayerCollectionRate*
                                          GameVariables.GetCollectionRateScale(roundNumber);
                    break;
                case 2:
                    _collectionTimeLeft = GameVariables.TwoPlayersCollectionRate*
                                          GameVariables.GetCollectionRateScale(roundNumber);
                    break;
                case 3:
                    _collectionTimeLeft = GameVariables.ThreePlayersCollectionRate*
                                          GameVariables.GetCollectionRateScale(roundNumber);
                    break;
                default:
                    throw new Exception("How the hell did you reach this part of the code?");
            }
        }

        public void DecrementCount()
        {
            Health--;

            if (Health <= 0) {
                IsToBeRemoved = true;
            }

            LightSpawner.Instance.AddStaticLight(Position, 1.0f, GameVariables.CrystalGlowRadius, 1);
            ParticleSystemManager.Instance.FireParticleSystem("crystal_collect", Position.X, Position.Y);
        }
    }
}