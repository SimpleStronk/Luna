using System;
using FontStashSharp;
using Luna.UI.InputFormat;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework.Graphics;

namespace Luna.HelperClasses
{
    internal class StringHelper
    {
        private static AlphanumericFormat alphanumericFormat = new AlphanumericFormat();
        private static CentiDecimalFormat centidecimalFormat = new CentiDecimalFormat();
        private static DecimalFormat decimalFormat = new DecimalFormat();
        private static NumericFormat numericFormat = new NumericFormat();
        private static UnformattedFormat unformattedFormat = new UnformattedFormat();

        /// <summary>
        /// Cuts the given text into lines
        /// </summary>
        /// <param name="text">Text to be cut</param>
        /// <param name="width">Maximum line width in pixels</param>
        /// <param name="atWhitespace">Determines whether to cut the text only at white space or at any character</param>
        /// <param name="font">The font the text is drawn with</param>
        /// <returns>The original string, with line delimiters to fit the text within the given space</returns>
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

        /// <summary>
        /// Cuts the given text into lines
        /// </summary>
        /// <param name="text">Text to be cut</param>
        /// <param name="width">Maximum line width in pixels</param>
        /// <param name="atWhitespace">Determines whether to cut the text only at white space or at any character</param>
        /// <param name="font">The font the text is drawn with</param>
        /// <returns>The original string, with line delimiters to fit the text within the given space</returns>
        public static string CutStringToBounds(string text, int width, bool atWhitespace, SpriteFontBase font)
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

        public static AlphanumericFormat GetAlphanumericFormat()
        {
            return alphanumericFormat;
        }

        public static CentiDecimalFormat GetCentiDecimalFormat()
        {
            return centidecimalFormat;
        }

        public static DecimalFormat GetDecimalFormat()
        {
            return decimalFormat;
        }

        public static NumericFormat GetNumericFormat()
        {
            return numericFormat;
        }

        public static UnformattedFormat GetUnformattedFormat()
        {
            return unformattedFormat;
        }
    }
}