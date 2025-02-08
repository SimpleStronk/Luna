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
        string displayText;
        SpriteFont font;

        public Label(string text, SpriteFont font, UITheme.ColorType colourType)
        {
            textObject = true;
            this.font = font;
            SetText(text);
            // UITheme tmp = theme;
            // tmp.ColourType = colourType;
            // SetTheme(tmp);
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
            displayText = DisplayText;

            if (layout.LayoutHeight.ScalingMode == Sizing.Mode.Ignore)
            {
                transform.Size = new LVector2(transform.Size.X, MeasureText(displayText).Y);
            }
        }

        protected override void Draw(SpriteBatch s)
        {
            Rectangle globalRect = GetTransform().GetGlobalRect();
            s.DrawString(font, displayText, new Vector2(globalRect.X, globalRect.Y), overrideTheme.GetColourPalatte(cascadeTheme).TextColour);
        }

        public override string GetTag()
        {
            return $"{base.GetTag()} \"{text}\"";
        }

        public void SetText(string text)
        {
            this.text = text;
            transform.Size = MeasureText(text);
        }

        private LVector2 MeasureText(string text)
        {
            string tmp = text;

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
                return StringHelper.CutStringToBounds(text, (int)transform.Size.X, true, font);
            }
        }
    }
}