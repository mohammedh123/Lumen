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

        private Dictionary<Keys, ActionType> _inputMap;

        public Player(string textureKey, Vector2 position) : base(textureKey, position)
        {
            NumCandlesLeft = GameVariables.PlayerInitialCandles;

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
            if(PlayerNum == PlayerIndex.Two)
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
        }

        private void ProcessControllerInput(float dt)
        {
            if (GamePad.GetState(PlayerNum).IsConnected)
            {
                var changeLeft = InputManager.GamepadLeft(PlayerNum);

                AdjustVelocity(changeLeft.X * GameVariables.PlayerSpeed * dt, -changeLeft.Y * GameVariables.PlayerSpeed * dt);

                IsInteractingWithProp = InputManager.GamepadButtonPressed(PlayerNum, Buttons.A);
            }
        }

        private void ProcessKeyDownAction(float dt, ActionType value)
        {
            switch (value)
            {
                case ActionType.MoveLeft:
                    AdjustVelocity(-GameVariables.PlayerSpeed*dt, 0);

                    break;
                case ActionType.MoveRight:
                    AdjustVelocity(GameVariables.PlayerSpeed * dt, 0);

                    break;
                case ActionType.MoveUp:
                    AdjustVelocity(0, -GameVariables.PlayerSpeed * dt);

                    break;
                case ActionType.MoveDown:
                    AdjustVelocity(0, GameVariables.PlayerSpeed * dt);

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

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
