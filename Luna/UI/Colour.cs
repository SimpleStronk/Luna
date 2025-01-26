using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework;

namespace Luna.UI
{
    internal class Colour
    {
        private Color value;

        public Colour()
        {
            value = new();
        }

        public Colour(Color color)
        {
            value = color;
        }

        public Colour(byte r, byte g, byte b)
        {
            value = new Color(r, g, b);
        }

        public Colour(float r, float g, float b)
        {
            value = new Color(r, g, b);
        }

        public Color Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public float R
        {
            get { return value.R; }
        }

        public float G
        {
            get { return value.G; }
        }

        public float B
        {
            get { return value.B; }
        }

        public Colour ScaleValue(float scale)
        {
            Color c = new Color(value.R, value.G, value.B) * scale;
            c.A = 255;
            return new Colour(c);
        }

        public static Colour operator*(Colour left, float right)
        {
            Color c = new Color(left.R, left.G, left.B);
            return new Colour(c * right);
        }

        public static implicit operator Colour((byte r, byte g, byte b) colour)
        {
            return new Colour(colour.r, colour.g, colour.b);
        }

        public static implicit operator Color(Colour colour)
        {
            return colour.Value;
        }
    }
}