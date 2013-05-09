using System;
using Lumen.Light_System;
using Lumen.Particle_System;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.Entities
{
    internal sealed class Player : Entity, IKeyboardCapable, IControllerCapable
    {
        public BlinkingLight AttachedBlinkingLight = null;
        public DurationLimitedFadingLight AttachedLight = null;
        public Crystal CollectionTarget;
        public int CrystalCount = 0;
        public bool IsInteractingWithProp = false;

        public OrbitingRing OrbitRing;
        public int PlayerSpriteIndex;

        private float _lightModulationSoundTimer;
        private float _recentlyHitTimer = -1.0f;
        private float _blinkingTimer = -1.0f;

        public Player(string textureKey, Vector2 position) : base(textureKey, position)
        {
            Health = GameVariables.PlayerStartingHealth;

            OrbitRing = new OrbitingRing(GameVariables.PlayerOrbsDistance, Health, 0.5f, GameVariables.PlayerOrbsPeriod,
                                         textureKey, new Rectangle(0, 0, 32, 32), this);
        }

        public void ResetRecentlyHitTimer()
        {
            _recentlyHitTimer = -1.0f;
        }

        public void ResetBlinkingTimer()
        {
            _blinkingTimer = -1.0f;
        }

        private bool IsLightOn
        {
            get { return AttachedLight.IsVisible; }
        }

        public bool IsAlive
        {
            get { return Health > 0; }
        }

        private bool WasRecentlyHit
        {
            get { return _recentlyHitTimer >= 0.0f; }
        }

        private bool IsBlinking
        {
            get { return _blinkingTimer >= 0.0f; }
        }

        #region IControllerCapable Members

        public PlayerIndex ControllerIndex { get; set; }

        public void ProcessControllerInput(float dt)
        {
            if (!GamePad.GetState(ControllerIndex).IsConnected) {
                return;
            }

            var changeLeft = InputManager.GamepadLeft(ControllerIndex);

            var speedToUse = GameVariables.PlayerSpeed;

            AdjustVelocity(changeLeft.X*speedToUse*dt, -changeLeft.Y*speedToUse*dt);

            if (InputManager.GamepadButtonDown(ControllerIndex, Buttons.A)) {
                TurnOnLight();
            }

            IsInteractingWithProp = InputManager.GamepadButtonPressed(ControllerIndex, Buttons.A);
        }

        #endregion

        #region IKeyboardCapable Members

        public void ProcessKeyDownAction(float dt)
        {
            var speedToUse = GameVariables.PlayerSpeed;

            if (InputManager.KeyDown(Keys.Left)) {
                AdjustVelocity(-speedToUse*dt, 0);
            }
            if (InputManager.KeyDown(Keys.Right)) {
                AdjustVelocity(speedToUse*dt, 0);
            }
            if (InputManager.KeyDown(Keys.Up)) {
                AdjustVelocity(0, -speedToUse*dt);
            }
            if (InputManager.KeyDown(Keys.Down)) {
                AdjustVelocity(0, speedToUse*dt);
            }
            if (InputManager.KeyDown(Keys.Space)) {
                TurnOnLight();
            }

            if (InputManager.KeyDown(Keys.Z)) {
                IsInteractingWithProp = true;
            }
        }

        public void ProcessKeyUpAction(float dt)
        {
            if (InputManager.KeyUp(Keys.Z)) {
                IsInteractingWithProp = false;
            }
        }

        public void ProcessKeyPressAction(float dt)
        {
        }

        #endregion

        public override void SetTexture(string textureKeyName)
        {
            base.SetTexture(textureKeyName);
            OrbitRing = new OrbitingRing(GameVariables.PlayerOrbsDistance, Health, 0.5f, GameVariables.PlayerOrbsPeriod,
                                         textureKeyName, new Rectangle(0, 0, 32, 32), this);
        }

        public void ResetOrbs()
        {
            OrbitRing.IsVisible = true;
        }

        public void Update(float dt)
        {
            if (IsAlive) {
#if DEBUG
                if (ControllerIndex <= PlayerIndex.Two) {
                    ProcessKeyDownAction(dt);
                    ProcessKeyUpAction(dt);
                    ProcessKeyPressAction(dt);
                }
#endif
                ProcessControllerInput(dt);

                if (IsLightOn) {
                    _lightModulationSoundTimer += dt;
                    if (_lightModulationSoundTimer <= MathHelper.TwoPi/3) {
                        SoundManager.GetSound("player_light").Play(1.0f,
                                                                   0.25f*
                                                                   (float) Math.Cos(_lightModulationSoundTimer*3.0f),
                                                                   0);
                    }
                }
                else {
                    _lightModulationSoundTimer = 0.0f;
                }
            }

            if (WasRecentlyHit) {
                _recentlyHitTimer -= dt;
                if (GamePad.GetState(ControllerIndex).IsConnected) {
                    GamePad.SetVibration(ControllerIndex, 1.0f, 1.0f);
                }
            }
            else {
                _recentlyHitTimer = -1.0f;
                if (GamePad.GetState(ControllerIndex).IsConnected) {
                    GamePad.SetVibration(ControllerIndex, 0, 0);
                }
            }

            if(IsBlinking) {
                _blinkingTimer -= dt;

                if (_blinkingTimer <= 1.0f) {
                    AttachedBlinkingLight.Duration = GameVariables.BlinkingDuration*5;
                }
                else if (_blinkingTimer <= 2.0f)
                    AttachedBlinkingLight.Duration = GameVariables.BlinkingDuration * 2.5f;
                else if (_blinkingTimer <= 3.0f)
                    AttachedBlinkingLight.Duration = GameVariables.BlinkingDuration * 1;

                AttachedBlinkingLight.IsVisible = true;
            }
            else {
                _blinkingTimer = -1.0f;
                AttachedBlinkingLight.IsVisible = false;
            }
        }

        private void TurnOnLight()
        {
            AttachedLight.TurnOn();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            OrbitRing.Draw(sb);
        }

        public void ResetCollecting()
        {
            if (CollectionTarget != null) {
                CollectionTarget.DecrementCollectorCount(this);
            }

            CollectionTarget = null;
        }

        public void TakeDamage(int n)
        {
            Health -= n;

            _recentlyHitTimer = GameVariables.PlayerHitVibrationDuration;
            _blinkingTimer = GameVariables.PlayerHitBlinkingDuration;

            var count = 0;
            foreach (var orb in OrbitRing.Satellites) {
                if (count >= n) {
                    break;
                }
                if (orb.IsVisible) {
                    count++;
                    orb.IsVisible = false;
                    LightSpawner.Instance.AddStaticLight(orb.Position, 1.0f, 12.0f, 2.0f);
                }
            }
            ResetCollecting();
            AttachedBlinkingLight.IsVisible = true;
            ParticleSystemManager.Instance.FireParticleSystem("player_hit", Position.X, Position.Y);
        }

        public void IncrementCrystalCount()
        {
            CrystalCount++;
        }
    }
}