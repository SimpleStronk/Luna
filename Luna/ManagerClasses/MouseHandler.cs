using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Luna.ManagerClasses
{
    internal class MouseHandler
    {
        static MouseState mouse, oldMouse;
        public enum MouseButton { Left, Middle, Right }
        private static bool firstFrame = true;

        public static Vector2 Position
        {
            get
            {
                return mouse.Position.ToVector2();
            }
        }

        /// <summary>
        /// The distance the mouse has travelled between the previous frame and this frame
        /// </summary>
        public static Vector2 DeltaPosition
        {
            get
            {
                return mouse.Position.ToVector2() - oldMouse.Position.ToVector2();
            }
        }

        /// <summary>
        /// Tests whether the specified mouse button was not clicked during the previous frame and now is
        /// </summary>
        public static bool IsJustClicked(MouseButton button)
        {
            ButtonState buttonState, buttonStateLast;

            buttonState = ButtonStateForButton(mouse, button);
            buttonStateLast = ButtonStateForButton(oldMouse, button);
            bool justClicked = buttonState == ButtonState.Pressed && buttonStateLast == ButtonState.Released;

            return justClicked;
        }

        /// <summary>
        /// Tests whether the specified mouse button was clicked during the previous frame and now isn't
        /// </summary>
        public static bool IsJustUnclicked(MouseButton button)
        {
            ButtonState buttonState, buttonStateLast;

            buttonState = ButtonStateForButton(mouse, button);
            buttonStateLast = ButtonStateForButton(oldMouse, button);
            bool justUnclicked = buttonState == ButtonState.Released && buttonStateLast == ButtonState.Pressed;

            return justUnclicked;
        }

        public static bool IsClicked(MouseButton button)
        {
            return ButtonStateForButton(mouse, button) == ButtonState.Pressed;
        }

        private static ButtonState ButtonStateForButton(MouseState mouse, MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left: return mouse.LeftButton;
                case MouseButton.Middle: return mouse.MiddleButton;
                case MouseButton.Right: return mouse.RightButton;
            }
            return ButtonState.Released;
        }

        public static int DeltaScroll
        {
            get { return firstFrame ? 0 : mouse.ScrollWheelValue - oldMouse.ScrollWheelValue; }
        }

        public static void SetMouse()
        {
            mouse = Mouse.GetState();
        }

        public static void SetOldMouse()
        {
            oldMouse = mouse;
            firstFrame = false;
        }
    }
}
