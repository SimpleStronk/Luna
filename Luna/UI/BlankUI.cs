using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.UI
{
    internal class BlankUI : UIComponent
    {
        public BlankUI(UITheme.ColorType colourType)
        {
            overrideTheme.ColourType = colourType;
        }

        protected override void Update()
        {

        }

        protected override void Draw(SpriteBatch s)
        {

        }
    }
}
