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
            this.text = text;
            this.font = font;
            // UITheme tmp = theme;
            // tmp.ColourType = colourType;
            // SetTheme(tmp);
            overrideTheme.ColourType = colourType;
            transform.Size = new LVector2(font.MeasureString(text));
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
                transform.Size = new LVector2(transform.Size.X, font.MeasureString(displayText).Y);
            }
        }

        protected override void Draw(SpriteBatch s)
        {
            Rectangle globalRect = GetTransform().GetGlobalRect();
            s.DrawString(font, displayText, new Vector2(globalRect.X, globalRect.Y), overrideTheme.GetColourPalette(cascadeTheme).TextColour);
        }

        public override string GetTag()
        {
            return $"{base.GetTag()} \"{text}\"";
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