using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.Entities
{
    class Guardian : Entity, IControllerCapable, IKeyboardCapable, ILightProvider
    {
        public PlayerIndex PlayerNum;

        public Color LightColor { get; set; }

        public float LightRadius
        {
            get
            {
                if(IsAttacking) {
                    return GameVariables.EnemyCollisionRadius + FinalRadiusOfAttack * (_attackTimer / GameVariables.EnemyAttackTotalDuration);
                }

                return IsChargingUp ? GameVariables.EnemyLightRadiusWhileCharging : 0.0f;
            }
            set { }
        }

        public float EnergyRemaining { get; set; }

        public float InternalAttackRadius
        {
            get
            {
                return Math.Min(_chargingTimer*(GameVariables.EnemyAttackRadiusGrowth +
                                                _chargingTimer*GameVariables.EnemyAttackRadiusAcceleration),
                                GameVariables.EnemyAttackMaxRadius);
            }
        }

        public float FinalRadiusOfAttack { get; private set; }
        private float _chargingTimer = 0.0f;
        private float _chargeCooldown = 0.0f;
        private float _attackTimer = -1.0f;

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
            Health = Int32.MaxValue;
            EnergyRemaining = GameVariables.EnemyAttackMaxRadius;
        }

        public override void Update(float dt)
        {
#if DEBUG
            if (PlayerNum <= PlayerIndex.Two && !GamePad.GetState(PlayerNum).IsConnected)
            {
                ProcessKeyDownAction(dt);
                ProcessKeyUpAction(dt);
                ProcessKeyPressAction(dt);
            }
#endif
            ProcessControllerInput(dt);

            if(IsChargingUp) {
                _chargingTimer += dt;
            }
            else {
                _chargingTimer = 0.0f;
            }

            if(IsAttacking) {
                _attackTimer += dt;
                if(_attackTimer > GameVariables.EnemyAttackTotalDuration)
                    StopAttack();
            }
            else {
                _attackTimer = -1.0f;
            }

            if(!IsChargingUp || IsChargingUp && EnergyRemaining == 0)
                EnergyRemaining = Math.Min(GameVariables.EnemyAttackMaxRadius, EnergyRemaining + GameVariables.EnemyEnergyRegeneration*dt);

            _chargeCooldown = Math.Max(_chargeCooldown - dt, 0.0f);

        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public void ProcessControllerInput(float dt)
        {
            if (!GamePad.GetState(PlayerNum).IsConnected)
                return;

            if(InputManager.GamepadButtonDown(PlayerNum, Buttons.A))
                ChargeUpAttack(dt);
            else if(InputManager.GamepadButtonUp(PlayerNum, Buttons.A))
                StopChargingAndRelease();

            var changeLeft = InputManager.GamepadLeft(PlayerNum);

            var speedToUse = !IsChargingUp ? GameVariables.EnemySpeed : GameVariables.EnemySpeedWhileCharging;

            AdjustVelocity(changeLeft.X * speedToUse * dt, -changeLeft.Y * speedToUse * dt);

            if (IsAttacking)
                Velocity = Vector2.Zero;

            if (Velocity != Vector2.Zero)
                Angle = (float)Math.Atan2(Velocity.Y, Velocity.X);
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
            }

            _chargingTimer = 0.0f;
        }

        private void BeginAttack()
        {
            PlayersHitThisAttack.Clear();

            _attackTimer = 0.0f;
        }

        private void StopAttack()
        {
            _attackTimer = -1.0f;
            FinalRadiusOfAttack = 0.0f;
        }

        private void ChargeUpAttack(float dt)
        {
            if(IsChargingUp) {
                _chargingTimer += dt;
                var radiusChange = Math.Min(EnergyRemaining, dt * (GameVariables.EnemyAttackRadiusGrowth +
                                       _chargingTimer * GameVariables.EnemyAttackRadiusAcceleration));

                EnergyRemaining = Math.Max(0,EnergyRemaining-radiusChange);
                FinalRadiusOfAttack += radiusChange;
            }
            else {
                if(CanChargeUp) {
                    _chargeCooldown = GameVariables.EnemyAttackCooldown;
                    _chargingTimer += dt;
                }
            }
        }
    }
}
