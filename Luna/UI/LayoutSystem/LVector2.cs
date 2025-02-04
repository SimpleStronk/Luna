using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Luna.UI.LayoutSystem
{
    internal class LVector2
    {
        public const int HORIZONTAL = 0;
        public const int VERTICAL = 1;

        private float[] components = [0,0];

        private Action onChanged;

        public LVector2()
        {
            components = [0, 0];
        }

        public LVector2(float x, float y)
        {
            components = [x, y];
        }

        public LVector2(Vector2 value)
        {
            components = [value.X, value.Y];
        }

        public float X
        {
            get { return components[HORIZONTAL]; }
            set { components[HORIZONTAL] = value; onChanged?.Invoke(); }
        }

        public float Y
        {
            get { return components[VERTICAL]; }
            set { components[VERTICAL] = value; onChanged?.Invoke(); }
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public float GetComponent(int axis)
        {
            return components[axis];
        }

        public void SetComponentValue(float component, int axis)
        {
            components[axis] = component;
            onChanged?.Invoke();
        }

        public static LVector2 SetComponentValue(LVector2 value, float component, int axis)
        {
            if (axis == HORIZONTAL) return new LVector2(component, value.Y);
            if (axis == VERTICAL) return new LVector2(value.X, component);
            return value;
        }

        public static LVector2 operator+(LVector2 left, LVector2 right)
        {
            return new LVector2(left.X + right.X, left.Y + right.Y);
        }

        public static LVector2 operator-(LVector2 left, LVector2 right)
        {
            return new LVector2(left.X - right.X, left.Y - right.Y);
        }

        public static LVector2 operator*(LVector2 left, LVector2 right)
        {
            return new LVector2(left.X * right.X, left.Y * right.Y);
        }

        public static LVector2 operator*(LVector2 left, float right)
        {
            return new LVector2(left.X * right, left.Y * right);
        }

        public static LVector2 operator/(LVector2 left, LVector2 right)
        {
            return new LVector2(left.X / right.X, left.Y / right.Y);
        }

        public static LVector2 operator/(LVector2 left, float right)
        {
            return new LVector2(left.X / right, left.Y / right);
        }

        public static LVector2 Zero
        {
            get { return new LVector2(0, 0); }
        }

        public static int AlternateAxis(int axis)
        {
            return 1 - axis;
        }

        public void OnChanged(Action e)
        {
            onChanged += e;
        }

        public static implicit operator LVector2 (Vector2 vector)
        {
            return new LVector2(vector);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}