using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lumen
{
    public class InputManager
    {
        #region Fields

        private static InputManager instance = null;
        private static KeyboardState oldKeyState, newKeyState;
        private static MouseState oldMouseState, newMouseState;
        private static readonly GamePadState?[] oldGamePadStates = new GamePadState?[4];
        private static readonly GamePadState?[] newGamePadStates = new GamePadState?[4];

        //access InputManager via InputManager.Instance
        private static InputManager I
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputManager();
                }

                return instance;
            }
        }

        #endregion

        //to avoid accidental construction
        private InputManager() { oldMouseState = newMouseState = Mouse.GetState(); oldKeyState = newKeyState = Keyboard.GetState(); }

        #region Key, Mouse, Gamepad Getters


        public static bool KeyDown(Keys key)
        { return newKeyState.IsKeyDown(key); }

        public static bool KeyUp(Keys key)
        { return newKeyState.IsKeyUp(key); }

        public static bool KeyPressed(Keys key)
        { return newKeyState.IsKeyDown(key) && oldKeyState.IsKeyUp(key); }

        public static bool GamepadButtonDown(PlayerIndex playerIndex, Buttons b)
        {
            return newGamePadStates[(int) playerIndex] != null
                       ? newGamePadStates[(int) playerIndex].Value.IsButtonDown(b)
                       : false;
        }

        public static bool GamepadButtonUp(PlayerIndex playerIndex, Buttons b)
        {
            return newGamePadStates[(int)playerIndex] != null
                       ? newGamePadStates[(int)playerIndex].Value.IsButtonUp(b)
                       : false;
        }

        public static bool GamepadButtonPressed(PlayerIndex playerIndex, Buttons b)
        {
            if (oldGamePadStates[(int)playerIndex] == null || newGamePadStates[(int)playerIndex] == null)
                return false;

            return newGamePadStates[(int)playerIndex].Value.IsButtonDown(b) && oldGamePadStates[(int)playerIndex].Value.IsButtonUp(b);
        }

        public static Vector2 GamepadLeft(PlayerIndex playerIndex)
        {
            if (newGamePadStates[(int)playerIndex] == null)
                return Vector2.Zero;

            return newGamePadStates[(int) playerIndex].Value.ThumbSticks.Left;
        }

        public static bool LeftMouseDown()
        { return newMouseState.LeftButton == ButtonState.Pressed; }

        public static bool LeftMouseUp()
        { return newMouseState.LeftButton == ButtonState.Released; }

        public static bool LeftMousePressed()
        { return newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released; }

        public static bool RightMouseDown()
        { return newMouseState.RightButton == ButtonState.Pressed; }

        public static bool RightMouseUp()
        { return newMouseState.RightButton == ButtonState.Released; }

        public static bool RightMousePressed()
        { return newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released; }

        public static bool MouseMoved()
        { return newMouseState.X != oldMouseState.X || newMouseState.Y != oldMouseState.Y; }

        public static bool MouseInRect(Rectangle rect)
        { return newMouseState.X >= rect.Left && newMouseState.X <= rect.Right && newMouseState.Y >= rect.Top && newMouseState.Y <= rect.Bottom; }


        #endregion
        #region Public Methods


        public static void BeginUpdate()
        {
            newKeyState = Keyboard.GetState();
            newMouseState = Mouse.GetState();

            for (var i = 0; i < 4; i++) {
                var state = GamePad.GetState((PlayerIndex) ((int) PlayerIndex.One + i));
                if(state.IsConnected) {
                    newGamePadStates[i] = state;
                }
                else {
                    newGamePadStates[i] = null;
                }
            }
        }

        public static void EndUpdate()
        {
            oldKeyState = newKeyState;
            oldMouseState = newMouseState;

            for(var i = 0; i < 4; i++) {
                oldGamePadStates[i] = newGamePadStates[i];
            }
        }


        #endregion
    }
}
