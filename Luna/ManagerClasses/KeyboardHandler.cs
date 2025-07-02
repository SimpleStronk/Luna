using System;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;

namespace Luna.ManagerClasses
{
    // This class was stolen from a previous project I did, originally I believe from a StackOverflow response.
    // Since then, the class has been heavily modified and improved.
    internal class KeyboardHandler
    {
        static KeyboardState keyboard, oldKeyboard;
        private static float keyTimer = -1;
        private static float keypressInitialDelay = 0.7f, keypressPersistentDelay = 0.04f;
        private static Keys? currentKey;

        public static bool TryConvertKeyboardInput(out char key)
        {
            Keys[] keys = keyboard.GetPressedKeys();

            // Is the current key still being pressed? (I.e. holding a specific key down)
            if (Array.IndexOf(keys, currentKey) > -1)
            {
                // Only allow an output if keyTimer has elapsed
                if (keyTimer < 0)
                {
                    // Reset keyTimer
                    keyTimer = keypressPersistentDelay;

                    // Outputs the character of the current key (or 0 if not pressed) and whether or not it is currently pressed
                    (char output, bool result) = TestKey(currentKey.GetValueOrDefault(), false);

                    if (result) { key = output; return true; }
                }
            }
            else
            {
                // Elapse keyTimer
                keyTimer = -1;
                currentKey = null;
            }

            foreach (Keys k in keys)
            {
                // Outputs the character of the current key and whether or not it is currently pressed (and wasn't during the previous frame
                (char output, bool result) = TestKey(k, true);

                if (result) { key = output; currentKey = k; keyTimer = keypressInitialDelay; return true; }
            }

            key = (char)0;
            if (keys.Length == 0) currentKey = null;
            return false;
        }

        /// <summary>
        /// Tests the given key to see whether it is pressed
        /// </summary>
        /// <param name="k">Key to test</param>
        /// <param name="firstFrameTest">If true, tests specifically for a new keypress</param>
        /// <returns>A tuple representing the character of the current key (or 0 if not pressed) and whether the key is pressed</returns>
        private static (char key, bool result) TestKey(Keys k, bool firstFrameTest)
        {
            
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            bool caps = shift != System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock);

            if (!oldKeyboard.IsKeyDown(k) || !firstFrameTest)
            {
                switch (k)
                {
                    //Alphabet keys
                    case Keys.A: return (caps ? 'A' : 'a', true);
                    case Keys.B: return (caps ? 'B' : 'b', true);
                    case Keys.C: return (caps ? 'C' : 'c', true);
                    case Keys.D: return (caps ? 'D' : 'd', true);
                    case Keys.E: return (caps ? 'E' : 'e', true);
                    case Keys.F: return (caps ? 'F' : 'f', true);
                    case Keys.G: return (caps ? 'G' : 'g', true);
                    case Keys.H: return (caps ? 'H' : 'h', true);
                    case Keys.I: return (caps ? 'I' : 'i', true);
                    case Keys.J: return (caps ? 'J' : 'j', true);
                    case Keys.K: return (caps ? 'K' : 'k', true);
                    case Keys.L: return (caps ? 'L' : 'l', true);
                    case Keys.M: return (caps ? 'M' : 'm', true);
                    case Keys.N: return (caps ? 'N' : 'n', true);
                    case Keys.O: return (caps ? 'O' : 'o', true);
                    case Keys.P: return (caps ? 'P' : 'p', true);
                    case Keys.Q: return (caps ? 'Q' : 'q', true);
                    case Keys.R: return (caps ? 'R' : 'r', true);
                    case Keys.S: return (caps ? 'S' : 's', true);
                    case Keys.T: return (caps ? 'T' : 't', true);
                    case Keys.U: return (caps ? 'U' : 'u', true);
                    case Keys.V: return (caps ? 'V' : 'v', true);
                    case Keys.W: return (caps ? 'W' : 'w', true);
                    case Keys.X: return (caps ? 'X' : 'x', true);
                    case Keys.Y: return (caps ? 'Y' : 'y', true);
                    case Keys.Z: return (caps ? 'Z' : 'z', true);

                    //Decimal keys
                    case Keys.D0: return (shift ? ')' : '0', true);
                    case Keys.D1: return (shift ? '!' : '1', true);
                    case Keys.D2: return (shift ? '\"' : '2', true);
                    case Keys.D3: return (shift ? '£' : '3', true);
                    case Keys.D4: return (shift ? '$' : '4', true);
                    case Keys.D5: return (shift ? '%' : '5', true);
                    case Keys.D6: return (shift ? '^' : '6', true);
                    case Keys.D7: return (shift ? '&' : '7', true);
                    case Keys.D8: return (shift ? '*' : '8', true);
                    case Keys.D9: return (shift ? '(' : '9', true);

                    //Decimal numpad keys
                    case Keys.NumPad0: return ('0', true);
                    case Keys.NumPad1: return ('1', true);
                    case Keys.NumPad2: return ('2', true);
                    case Keys.NumPad3: return ('3', true);
                    case Keys.NumPad4: return ('4', true);
                    case Keys.NumPad5: return ('5', true);
                    case Keys.NumPad6: return ('6', true);
                    case Keys.NumPad7: return ('7', true);
                    case Keys.NumPad8: return ('8', true);
                    case Keys.NumPad9: return ('9', true);

                    //Special keys
                    case Keys.OemTilde: return (shift ? '@' : '\'', true);
                    case Keys.OemSemicolon: return (shift ? ':' : ';', true);
                    case Keys.OemQuotes: return (shift ? '~' : '#', true);
                    case Keys.OemQuestion: return (shift ? '?' : '/', true);
                    case Keys.OemPlus: return (shift ? '+' : '=', true);
                    case Keys.OemPipe: return (shift ? '|' : '\\', true);
                    case Keys.OemPeriod: return (shift ? '>' : '.', true);
                    case Keys.OemOpenBrackets: return (shift ? '{' : '[', true);
                    case Keys.OemCloseBrackets: return (shift ? '}' : ']', true);
                    case Keys.OemMinus: return (shift ? '_' : '-', true);
                    case Keys.OemComma: return (shift ? '<' : ',', true);
                    case Keys.Space: return (' ', true);
                    case Keys.Enter: return ('\n', true);
                    case Keys.Back: return ('\b', true);
                    case Keys.Left: return ((char)0, true);
                    case Keys.Right: return ((char)1, true);
                    case Keys.Up: return ((char)2, true);
                    case Keys.Down: return ((char)3, true);
                }
            }

            return ((char)0, false);
        }

        /// <summary>
        /// Checks the ASCII value of the given character to see if it is numeric
        /// </summary>
        public static bool IsNumber(char x)
        {
            if (x >= 48 && x <= 57) return true;
            return false;
        }

        /// <summary>
        /// Checks the ASCII value of the given character to see if it is numeric or a point
        /// </summary>
        public static bool IsNumberOrPoint(char x)
        {
            if ((x >= 48 && x <= 57) || x == '.') return true;
            return false;
        }

        /// <summary>
        /// Checks the ASCII value of the given character to see if it is alphabetic
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsAlphabet(char x)
        {
            if (((int)x >= 65 && (int)x <= 122) || (x == ' ')) return true;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if and only if the specified key is pressed, but wasn't during the previous frame
        /// </summary>
        public static bool IsKeyJustPressed(Keys key)
        {
            return keyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key);
        }

        /// <summary>Captures the current KeyboardState</summary>
        /// <remarks>To be called from the program's main PreUpdate</remarks>
        public static void SetKeyboard()
        {
            keyboard = Keyboard.GetState();
        }

        /// <summary>
        /// Sets KeyboardHandler's <c>oldKeyboard</c> to be the current Keyboard state, for inter-frame comparison
        /// </summary>
        /// <remarks>To be called from the program's main PostUpdate</remarks>
        public static void SetOldKeyboard()
        {
            oldKeyboard = Keyboard.GetState();
        }

        /// <summary>
        /// Steps forward the timer keeping track of the current keypress
        /// </summary>
        public static void IncrementTime(float deltaTime)
        {
            keyTimer -= deltaTime;

            if (keyTimer < 0)
            {
                keyTimer = -1;
            }
        }
    }
}
