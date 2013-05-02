using System;
using System.Collections.Generic;
using Lumen.Light_System;
using Lumen.Particle_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.Entities
{
    class Guardian : Entity, IControllerCapable, IKeyboardCapable, ILightProvider
    {
        public PlayerIndex ControllerIndex { get; set; }
        public Color LightColor { get; set; }

        public float LightRadius
        {
            get
            {
                if(IsAttacking) {
                    return ChargingAttackRadius * (_attackTimer / GameVariables.EnemyAttackTotalDuration);
                }

                return IsChargingUp ? GameVariables.EnemyLightRadiusWhileCharging : 0.0f;
            }
            set { }
        }

        public float LightIntensity { get; set; }
        public float EnergyRemaining { get; set; }

        public float ChargingAttackRadius
        {
            get { return (GameVariables.EnemyLightRadiusWhileCharging + FinalRadiusOfAttack); }
        }

        public float InternalAttackRadius
        {
            get
            {
                return Math.Min(_chargingTimer*(GameVariables.EnemyAttackRadiusGrowth +
                                                _chargingTimer*GameVariables.EnemyAttackRadiusAcceleration),
                                GameVariables.EnemyAttackMaxRadius);
            }
        }

        public float ChargingSpeed
        {
            get { return MathHelper.Lerp(GameVariables.EnemySpeedWhileCharging, GameVariables.EnemySpeed, (1 - Math.Min(4 * InternalAttackRadius / GameVariables.EnemyAttackMaxRadius, 1.0f))); }
        }

        public float FinalRadiusOfAttack { get; private set; }
        private float _chargingTimer = 0.0f;
        private float _chargeCooldown = 0.0f;
        private float _attackTimer = -1.0f;

        public OrbitingRing OrbitRing;

        public List<Player> PlayersHitThisAttack = new List<Player>();

        public bool IsAttacking
        {
            get { return _attackTimer >= 0.0f; }
        }

        public bool CanChargeUp
        {
            get { return _chargeCooldown <= 0.0f; }
        }

        public bool IsChargingUp
        {
            get { return _chargingTimer > 0.0f; }
        }

        public bool IsFullyCharged
        {
            get { return FinalRadiusOfAttack+float.Epsilon >= GameVariables.EnemyAttackMaxRadius; }
        }

        public Guardian(Vector2 position) : base("guardian", position)
        {
            LightIntensity = 1.0f;
            Health = Int32.MaxValue;
            EnergyRemaining = GameVariables.EnemyAttackMaxRadius;

            OrbitRing = new OrbitingRing(0, 10, 1.0f, 0.5f, "hit_particle", new Rectangle(0,0,2,2), this);
            OrbitRing.IsVisible = false;
        }

        public override void Update(float dt)
        {
#if DEBUG
            if (ControllerIndex <= PlayerIndex.Two && !GamePad.GetState(ControllerIndex).IsConnected)
            {
                ProcessKeyDownAction(dt);
                ProcessKeyUpAction(dt);
                ProcessKeyPressAction(dt);
            }
#endif
            ProcessControllerInput(dt);

            if(IsChargingUp) {
                _chargingTimer += dt;
                SetOrbitRingProperties(true);
            }
            else {
                _chargingTimer = 0.0f;
            }

            if (IsAttacking)
            {
                Velocity = Vector2.Zero;
                _attackTimer += dt;
                SetOrbitRingProperties(true);
                if(_attackTimer >= GameVariables.EnemyAttackTotalDuration)
                    StopAttack();
            }
            else {
                _attackTimer = -1.0f;
            }

            if (Velocity != Vector2.Zero)
                Angle = (float)Math.Atan2(Velocity.Y, Velocity.X);

            if(!IsChargingUp)// || IsChargingUp && EnergyRemaining == 0)
                EnergyRemaining = Math.Min(GameVariables.EnemyAttackMaxRadius, EnergyRemaining + GameVariables.EnemyEnergyRegeneration*dt);

            _chargeCooldown = Math.Max(_chargeCooldown - dt, 0.0f);
        }

        private void SetOrbitRingProperties(bool visible)
        {
            OrbitRing.IsVisible = visible;
            OrbitRing.Radius = IsAttacking ? LightRadius : ChargingAttackRadius;
            OrbitRing.OrbitPeriod = MathHelper.Lerp(1.0f, 0.4f, OrbitRing.Radius / GameVariables.EnemyAttackMaxRadius);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            OrbitRing.Draw(sb);
        }   
        
        public void ProcessControllerInput(float dt)
        {
            if (!GamePad.GetState(ControllerIndex).IsConnected)
                return;

            if(InputManager.GamepadButtonDown(ControllerIndex, Buttons.A))
                ChargeUpAttack(dt);
            else if(InputManager.GamepadButtonUp(ControllerIndex, Buttons.A))
                StopChargingAndRelease();

            var changeLeft = InputManager.GamepadLeft(ControllerIndex);

            var speedToUse = !IsChargingUp ? GameVariables.EnemySpeed : ChargingSpeed;

            AdjustVelocity(changeLeft.X * speedToUse * dt, -changeLeft.Y * speedToUse * dt);
        }

        public void ProcessKeyDownAction(float dt)
        {
            var speedToUse = GameVariables.EnemySpeed;

            if (InputManager.KeyDown(Keys.Left))
                AdjustVelocity(-speedToUse * dt, 0);
            if (InputManager.KeyDown(Keys.Right))
                AdjustVelocity(speedToUse * dt, 0);
            if (InputManager.KeyDown(Keys.Up))
                AdjustVelocity(0, -speedToUse * dt);
            if (InputManager.KeyDown(Keys.Down))
                AdjustVelocity(0, speedToUse * dt);
            if(InputManager.KeyDown(Keys.Space))
                ChargeUpAttack(dt);

            if (IsAttacking)
                Velocity = Vector2.Zero;
        }

        public void ProcessKeyUpAction(float dt)
        {
            if (InputManager.KeyUp(Keys.Space))
                StopChargingAndRelease();
        }

        public void ProcessKeyPressAction(float dt)
        {
        }

        private void StopChargingAndRelease()
        {
            if (IsChargingUp) {
                BeginAttack();
                _chargingTimer = 0.0f;
            }
            SoundManager.GetSoundInstance("guardian_charge").Stop();

        }

        private void BeginAttack()
        {
            PlayersHitThisAttack.Clear();

            _attackTimer = 0.0f;
            SoundManager.GetSoundInstance("guardian_release").Play();
            ParticleSystemManager.Instance.FireParticleSystem("guardian_attack", Position.X, Position.Y);
        }

        private void StopAttack()
        {
            OrbitRing.IsVisible = false;
            _attackTimer = -1.0f;

            LightSpawner.Instance.AddStaticLight(Position, 1.0f, ChargingAttackRadius, 0.10f);

            FinalRadiusOfAttack = 0.0f;
        }

        public void ResetAllAttackData()
        {
            _chargingTimer = 0.0f;
            _attackTimer = -1.0f;
            _chargeCooldown = 0.0f;
            OrbitRing.IsVisible = false;

            FinalRadiusOfAttack = 0.0f;
            PlayersHitThisAttack.Clear();
        }

        private void ChargeUpAttack(float dt)
        {
            if(IsChargingUp) {
                _chargingTimer += dt;
                var radiusChange = Math.Min(EnergyRemaining, dt * (GameVariables.EnemyAttackRadiusGrowth +
                                       _chargingTimer * GameVariables.EnemyAttackRadiusAcceleration));

                EnergyRemaining = Math.Max(0,EnergyRemaining-radiusChange);
                FinalRadiusOfAttack = Math.Min(GameVariables.EnemyAttackMaxRadius, FinalRadiusOfAttack+radiusChange);
                
                SoundManager.GetSoundInstance("guardian_charge").Play();
            }
            else if(!IsAttacking) {
                if(CanChargeUp) {
                    _chargeCooldown = GameVariables.EnemyAttackCooldown;
                    _chargingTimer += dt;
                }
            }
        }
    }
}
