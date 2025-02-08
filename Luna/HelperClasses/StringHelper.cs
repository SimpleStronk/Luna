using System;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework.Graphics;

namespace Luna.HelperClasses
{
    internal class StringHelper
    {
        public static string CutStringToBounds(string text, int width, bool atWhitespace, SpriteFont font)
        {
            string sampleText = text;
            if (width <= 0) return sampleText;

            string workingText = "";
            int index = 0;
            int lastSpace = -1;

            while (index < sampleText.Length)
            {
                if (sampleText[index] == ' ') lastSpace = index;

                if (font.MeasureString(workingText + sampleText[index]).X > width)
                {
                    if (atWhitespace && lastSpace != -1)
                    {
                        if (index == lastSpace)
                        {
                            workingText += "\n";
                            index++;
                            continue;
                        }

                        string start = workingText.Substring(0, lastSpace);
                        string end = workingText.Substring(lastSpace + 1, workingText.Length - lastSpace - 1);
                        workingText = start + "\n" + end;
                        lastSpace = -1;
                    }
                    else workingText += "\n";
                }
                workingText += sampleText[index];

                index++;
            }

            return workingText;
        }
    }
}