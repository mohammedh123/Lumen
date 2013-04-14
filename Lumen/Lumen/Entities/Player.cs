using System;
using System.Collections.Generic;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.Entities
{
    class Player : Entity, IKeyboardCapable, IControllerCapable
    {
        public PlayerIndex PlayerNum;

        public bool IsInteractingWithProp = false;

        public Crystal CollectionTarget;
        public float CollectingTime = -1;
        public int CrystalCount = 0;
        
        public bool IsCollecting
        {
            get { return CollectingTime >= 0.0f; } 
        }

        public Player(string textureKey, Vector2 position) : base(textureKey, position)
        {
            Health = 6;
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

        public void ProcessControllerInput(float dt)
        {
            if (!GamePad.GetState(PlayerNum).IsConnected)
                return;
            
            var changeLeft = InputManager.GamepadLeft(PlayerNum);

            if (InputManager.GamepadButtonPressed(PlayerNum, Buttons.X))
                Collect();

            if (InputManager.GamepadButtonUp(PlayerNum, Buttons.X))
                ResetCollecting();

            var speedToUse = GameVariables.PlayerSpeed;

            if(!IsCollecting)
                AdjustVelocity(changeLeft.X*speedToUse*dt, -changeLeft.Y*speedToUse*dt);
                
            if(Velocity != Vector2.Zero)
                Angle = (float) Math.Atan2(Velocity.Y, Velocity.X);

            IsInteractingWithProp = InputManager.GamepadButtonPressed(PlayerNum, Buttons.A);
        }

        public void ProcessKeyDownAction(float dt)
        {
            var speedToUse = GameVariables.PlayerSpeed;

            if (!IsCollecting)
            {
                if(InputManager.KeyDown(Keys.Left))
                    AdjustVelocity(-speedToUse*dt, 0);
                if(InputManager.KeyDown(Keys.Right))
                    AdjustVelocity(speedToUse*dt, 0);
                if(InputManager.KeyDown(Keys.Up))
                    AdjustVelocity(0, -speedToUse*dt);
                if(InputManager.KeyDown(Keys.Down))
                    AdjustVelocity(0, speedToUse*dt);
            }

            if(InputManager.KeyDown(Keys.Z))
                IsInteractingWithProp = true;
        }

        public void ProcessKeyUpAction(float dt)
        {
            if(!GamePad.GetState(PlayerNum).IsConnected && InputManager.KeyUp(Keys.Space))
                ResetCollecting();

            if(InputManager.KeyUp(Keys.Z))
                IsInteractingWithProp = false;
        }

        public void ProcessKeyPressAction(float dt)
        {
            if (InputManager.KeyPressed(Keys.Space))
                Collect();
        }

        public void Collect()
        {
            CollectingTime = 0.0f;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if(IsCollecting)
            {
                var outerRect = new Rectangle((int)Position.X - 32, (int)Position.Y - 32, 64, 16);
                var innerRect = new Rectangle(outerRect.X+2, outerRect.Y+2, (int)((64-4)*CollectingTime/GameVariables.CrystalCollectionTime), 12);

                sb.Draw(TextureManager.GetTexture("blank"), outerRect, Color.White);
                sb.Draw(TextureManager.GetTexture("blank"), innerRect, Color.Cyan);
            }
        }

        public void ResetCollecting()
        {
            CollectionTarget = null;
            CollectingTime = -1;
        }
    }
}
