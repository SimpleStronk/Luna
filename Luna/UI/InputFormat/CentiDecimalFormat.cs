using Luna.ManagerClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.UI.InputFormat
{
    internal class CentiDecimalFormat : IInputFormat
    {
        public bool isInputValid(string currentString, char c)
        {
            if (!KeyboardHandler.IsNumberOrPoint(c)) return false;

            if (currentString.Split('.').Length > 1)
            {
                if (c == '.') return false;
                if (currentString.Split('.')[1].Length > 1) return false;
            }

            return true;
        }
    }
}
