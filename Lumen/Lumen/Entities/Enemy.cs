using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.Entities
{
    class Enemy : Entity, IControllerCapable, IKeyboardCapable
    {
        public PlayerIndex PlayerNum;

        public Enemy(Vector2 position) : base("enemy", position)
        {
            Health = Int32.MaxValue;
        }

        public override void Update(float dt)
        {
#if DEBUG
            if (PlayerNum <= PlayerIndex.Two)
            {
                ProcessKeyDownAction(dt);
                ProcessKeyUpAction(dt);
                ProcessKeyPressAction(dt);
            }
#endif

            ProcessControllerInput(dt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public void ProcessControllerInput(float dt)
        {
            if (!GamePad.GetState(PlayerNum).IsConnected)
                return;

            var changeLeft = InputManager.GamepadLeft(PlayerNum);

            var speedToUse = GameVariables.EnemySpeed;

            AdjustVelocity(changeLeft.X * speedToUse * dt, -changeLeft.Y * speedToUse * dt);

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
        }

        public void ProcessKeyUpAction(float dt)
        {
        }

        public void ProcessKeyPressAction(float dt)
        {
        }
    }
}
