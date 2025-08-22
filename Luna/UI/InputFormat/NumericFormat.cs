using Luna.ManagerClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.UI.InputFormat
{
    internal class NumericFormat : IInputFormat
    {
        public bool isInputValid(string currentString, char c)
        {
            return KeyboardHandler.IsNumber(c);
        }
    }
}
