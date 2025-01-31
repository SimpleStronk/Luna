using System;
using Microsoft.Xna.Framework;

namespace Luna.UI.LayoutSystem
{
    internal interface ColourAnimator
    {
        public void Update();

        public void SetColour(Color colour);

        public void ForceColour(Color color);

        public Color GetColour();
    }
}