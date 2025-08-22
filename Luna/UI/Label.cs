using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using Luna.HelperClasses;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class Label : UIComponent
    {
        string text;
        SpriteFont font;

        public Label(string text, SpriteFont font, UITheme.ColorType colourType)
        {
            textObject = true;
            this.font = font;
            SetText(text);
            overrideTheme.ColourType = colourType;
            RenderDefaultRect = false;
            FocusIgnore = true;

            layout = new Layout()
            {
                LayoutWidth = Sizing.Ignore(),
                LayoutHeight = Sizing.Ignore()
            };
        }

        protected override void Update()
        {
            // If height isn't determined by LUIVA, keep the width and measure the height
            if (layout.LayoutHeight.ScalingMode == Sizing.Mode.Ignore)
            {
                transform.Size = new LVector2(transform.Size.X, MeasureText(DisplayText).Y);
            }
        }

        protected override void Draw(SpriteBatch s)
        {
            Rectangle globalRect = GetTransform().GetGlobalRect();

            // Draw text to screen
            s.DrawString(font, DisplayText, new Vector2(globalRect.X, globalRect.Y), overrideTheme.GetColourPalatte(cascadeTheme).TextColour);
        }

        public override string GetTag()
        {
            return $"{base.GetTag()} \"{text}\"";
        }

        public void SetText(string text)
        {
            this.text = text;
            if (text == null) return;

            transform.Size = MeasureText(text);
        }

        private LVector2 MeasureText(string text)
        {
            string tmp = text;

            // Blank characters affect the measured size of the string, so add full-height elements
            // to make MeasureString acknowledge the whitespace
            if (tmp == "") tmp = "|";
            else if (tmp.Split('\n')[0] == "") tmp = "|" + text;
            else if (tmp.Split('\n').Last() == "") tmp = text + "|";
            else tmp = text;

            return font.MeasureString(tmp);
        }

        private string DisplayText
        {
            get
            {
                if (text == null) return "";
                return StringHelper.CutStringToBounds(text, (int)transform.Size.X, true, font);
            }
        }
    }
}