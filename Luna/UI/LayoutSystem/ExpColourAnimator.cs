using System;
using System.CodeDom;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D11;

namespace Luna.UI.LayoutSystem
{
    internal class ExpColourAnimator : IColourAnimator
    {
        private Colour currentColour;
        private Colour targetColour;
        private float dampingFactor = 5f;
        Action onTransitionAction;

        public void Update()
        {
            currentColour += (targetColour - currentColour) / dampingFactor;

            if (GetColour() == Colour.ToColor(targetColour) && onTransitionAction != null)
            {
                onTransitionAction.Invoke();
                onTransitionAction = null;
            }
        }

        public void SetColour(Color colour)
        {
            targetColour = Colour.FromColor(colour);
        }

        public void ForceColour(Color colour)
        {
            currentColour = targetColour = Colour.FromColor(colour);
        }

        public Color GetColour()
        {
            return Colour.ToColor(currentColour);
        }

        public void OnTransitionAction(Action e)
        {
            onTransitionAction += e;
        }

        struct Colour
        {
            float r, g, b, a;

            public int Ri
            {
                get { return (int)((r * 255) + 0.01f); }
            }

            public int Gi
            {
                get { return (int)((g * 255) + 0.01f); }
            }

            public int Bi
            {
                get { return (int)((b * 255) + 0.01f); }
            }

            public int Ai
            {
                get { return (int)((a * 255) + 0.01f); }
            }

            public static Colour FromColor(Color color)
            {
                return FromInts(color.R, color.G, color.B, color.A);
            }

            public static Color ToColor(Colour colour)
            {
                return new Color(colour.Ri, colour.Gi, colour.Bi, colour.Ai);
            }

            public static Colour FromInts(int r, int g, int b, int a)
            {
                Colour c = new Colour();
                c.r = r / 255f;
                c.g = g / 255f;
                c.b = b / 255f;
                c.a = a / 255f;
                return c;
            }

            public static Colour FromFloats(float r, float g, float b, float a)
            {
                Colour c = new Colour();
                c.r = r;
                c.g = g;
                c.b = b;
                c.a = a;
                return c;
            }

            public static Colour operator +(Colour left, Colour right)
            {
                return FromFloats(left.r + right.r, left.g + right.g, left.b + right.b, left.a + right.a);
            }

            public static Colour operator -(Colour left, Colour right)
            {
                return FromFloats(left.r - right.r, left.g - right.g, left.b - right.b, left.a - right.a);
            }

            public static Colour operator *(Colour left, float right)
            {
                return FromFloats(left.r * right, left.g * right, left.b * right, left.a * right);
            }

            public static Colour operator /(Colour left, float right)
            {
                return FromFloats(left.r / right, left.g / right, left.b / right, left.a / right);
            }
        }
    }
}