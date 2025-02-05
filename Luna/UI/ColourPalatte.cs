using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework;

namespace Luna.UI
{
    internal class ColourPalatte
    {
        private Color mainColour;
        private Color hoveredColour;
        private Color selectedColour;
        private Color textColour;

        public ColourPalatte SetMainColour(Color color)
        {
            mainColour = color;
            return this;
        }

        public ColourPalatte SetHoveredColour(Color color)
        {
            hoveredColour = color;
            return this;
        }

        public ColourPalatte SetSelectedColour(Color color)
        {
            selectedColour = color;
            return this;
        }

        public ColourPalatte SetTextColour(Color color)
        {
            textColour = color;
            return this;
        }

        public Color MainColour
        {
            get { return mainColour; }
            set { mainColour = value; }
        }

        public Color HoveredColour
        {
            get { return hoveredColour; }
            set { hoveredColour = value; }
        }

        public Color SelectedColour
        {
            get { return selectedColour; }
            set { selectedColour = value; }
        }

        public Color TextColour
        {
            get { return textColour; }
            set { textColour = value; }
        }

        public static ColourPalatte Transparent
        {
            get
            {
                ColourPalatte cp = new ColourPalatte();
                cp.MainColour = cp.HoveredColour = cp.selectedColour = cp.TextColour = new Color(0, 0, 0, 0);
                return cp;
            }
        }
    }
}