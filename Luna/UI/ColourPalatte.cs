using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework;

namespace Luna.UI
{
    internal class ColourPalette
    {
        private Color mainColour;
        private Color hoveredColour;
        private Color selectedColour;
        private Color textColour;

        public ColourPalette SetMainColour(Color color)
        {
            mainColour = color;
            return this;
        }

        public ColourPalette SetHoveredColour(Color color)
        {
            hoveredColour = color;
            return this;
        }

        public ColourPalette SetSelectedColour(Color color)
        {
            selectedColour = color;
            return this;
        }

        public ColourPalette SetTextColour(Color color)
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

        public static ColourPalette Transparent
        {
            get
            {
                ColourPalette cp = new ColourPalette();
                cp.MainColour = cp.HoveredColour = cp.selectedColour = cp.TextColour = new Color(0, 0, 0, 0);
                return cp;
            }
        }
    }
}