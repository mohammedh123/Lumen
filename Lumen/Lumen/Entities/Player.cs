using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.Entities
{
    class Player : Entity
    {
        public int NumCandlesLeft;
        public bool IsInteractingWithProp = false;
        public PlayerIndex PlayerNum;
        public int CoinCount = 0;
        public bool CanPickUpCoins = true;
        public bool HasCollidedWithPlayerThisFrame = false;
        public bool IsEnemy = false;
        public float TimeWithinEnemyProximity = 0.0f;
        public PlayerWeaponType Weapon = PlayerWeaponType.Torch;

        private float _dashTimer;
        private float _dashCooldownTimer;

        private float _attackTimer;
        private float _attackCooldownTimer;
        public List<Player> CollidedPlayersThisAttack;

        private float _burningTimer;

        public bool IsAttacking
        {
            get { return _attackTimer > 0.0f; }
        }

        public bool IsDashing
        {
            get { return _dashTimer > 0.0f; }
        }

        public bool IsBurning
        {
            get { return _burningTimer > 0.0f; }
        }

        public bool HasSpentTooMuchTimeNearEnemy
        {
            get { return TimeWithinEnemyProximity >= GameVariables.EnemyKillTimeRequirement; }
        }

        private Dictionary<Keys, ActionType> _inputMap;

        public Player(string textureKey, Vector2 position) : base(textureKey, position)
        {
            Health = 6;
            NumCandlesLeft = GameVariables.PlayerInitialCandles;
            CollidedPlayersThisAttack = new List<Player>(4);

            _inputMap = new Dictionary<Keys, ActionType>
                            {
                                {Keys.Left, ActionType.MoveLeft},
                                {Keys.Right, ActionType.MoveRight},
                                {Keys.Up, ActionType.MoveUp},
                                {Keys.Down, ActionType.MoveDown},
                                {Keys.Space, ActionType.InteractWithProp}
                            };
        }

        public override void Update(float dt)
        {
            if(PlayerNum == PlayerIndex.One)
            foreach (var kvp in _inputMap) {
#if DEBUG
                if (InputManager.KeyDown(kvp.Key)) {
                    ProcessKeyDownAction(dt, kvp.Value);
                }
                else if (InputManager.KeyUp(kvp.Key)) {
                    ProcessKeyUpAction(dt, kvp.Value);
                }
#endif
            }

            ProcessControllerInput(dt);

            if (IsDashing)
                _dashTimer -= dt;
            else
            {
                _dashTimer = 0.0f;
            }

            _dashCooldownTimer = Math.Max(_dashCooldownTimer - dt, 0);

            if (IsAttacking)
                _attackTimer -= dt;
            else
            {
                _attackTimer = 0.0f;
            }

            if (IsBurning)
                _burningTimer -= dt;
            else
            {
                _burningTimer = 0.0f;
            }

            _attackCooldownTimer = Math.Max(_attackCooldownTimer - dt, 0);
        }

        private void ProcessControllerInput(float dt)
        {
            if (GamePad.GetState(PlayerNum).IsConnected) {
                var changeLeft = InputManager.GamepadLeft(PlayerNum);

                var speedToUse = IsEnemy
                                     ? (IsDashing ? GameVariables.EnemyDashSpeed : GameVariables.EnemySpeed)
                                     : GameVariables.PlayerSpeed;

                AdjustVelocity(changeLeft.X*speedToUse*dt, -changeLeft.Y*speedToUse*dt);
                if(Velocity != Vector2.Zero)
                    Angle = (float) Math.Atan2(Velocity.Y, Velocity.X);

                IsInteractingWithProp = InputManager.GamepadButtonPressed(PlayerNum, Buttons.A);

                if (InputManager.GamepadButtonDown(PlayerNum, Buttons.X)) {
                    Dash();
                    Attack();
                }
            }
        }

        private void ProcessKeyDownAction(float dt, ActionType value)
        {
            var speedToUse = IsEnemy ? GameVariables.EnemySpeed : GameVariables.PlayerSpeed;

            switch (value)
            {
                case ActionType.MoveLeft:
                    AdjustVelocity(-speedToUse * dt, 0);

                    break;
                case ActionType.MoveRight:
                    AdjustVelocity(speedToUse * dt, 0);

                    break;
                case ActionType.MoveUp:
                    AdjustVelocity(0, -speedToUse * dt);

                    break;
                case ActionType.MoveDown:
                    AdjustVelocity(0, speedToUse * dt);

                    break;
                case ActionType.InteractWithProp:
                    IsInteractingWithProp = true;
                    
                    break;
                default:
                    throw new Exception(
                        String.Format(
                            "If you are reading this message, then something disastrous happened or you forgot to add a key down handler for this type of ActionType: {0}.",
                            value));
            }
        }

        private void ProcessKeyUpAction(float dt, ActionType value)
        {
            switch (value)
            {
                case ActionType.MoveLeft:
                case ActionType.MoveRight:
                case ActionType.MoveUp:
                case ActionType.MoveDown: //dont do jack shit for these 4 actions
                    break;
                case ActionType.InteractWithProp:
                    IsInteractingWithProp = false;
                    
                    break;
                default:
                    throw new Exception(
                        String.Format(
                            "If you are reading this message, then something disastrous happened or you forgot to add a key up handler for this type of ActionType: {0}.",
                            value));
            }
        }

        public void Attack()
        {
            if (IsEnemy) return;

            if (_attackCooldownTimer == 0.0f) {
                CollidedPlayersThisAttack.Clear();

                switch(Weapon) {
                    case PlayerWeaponType.Torch:
                        _attackTimer = GameVariables.PlayerTorchAttackDuration;
                        _attackCooldownTimer = GameVariables.PlayerTorchAttackCooldown;
                        break;
                    case PlayerWeaponType.Sword:
                        _attackTimer = GameVariables.PlayerSwordAttackDuration;
                        _attackCooldownTimer = GameVariables.PlayerSwordAttackCooldown;
                        break;
                }
            }
        }

        public void Dash()
        {
            if (!IsEnemy) return;

            if (_dashCooldownTimer == 0.0f) {
                _dashTimer = GameVariables.EnemyDashDuration;
                _dashCooldownTimer = GameVariables.EnemyDashCooldown;
            }
        }

        public void SetOnFire()
        {
            _burningTimer = GameVariables.ImmolatedLightDuration;
        }

        public void ResetProximityTime()
        {
            TimeWithinEnemyProximity = 0.0f;
        }

        public override void Draw(SpriteBatch sb)
        {
            var oldColor = Color;
            if(IsEnemy) {
                Color = Color.DarkRed;
            }
            else
                Color = Color.Lerp(oldColor, Color.Black, TimeWithinEnemyProximity/GameVariables.EnemyKillTimeRequirement);

            base.Draw(sb);
            Color = oldColor;

            if (IsAttacking) {
                var wpn = "";

                if (Weapon == PlayerWeaponType.Sword) {
                    wpn = "sword";
                }
                else if (Weapon == PlayerWeaponType.Torch) {
                    wpn = "torch";
                }

                sb.Draw(TextureManager.GetTexture(wpn),
                        Position + new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle)) * 18.0f, null, Color.White, Angle, TextureManager.GetOrigin(wpn), 1.0f, SpriteEffects.None, 0);
            }
        }
    }
}
