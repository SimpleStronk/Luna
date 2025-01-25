using System;
using System.Windows.Forms.VisualStyles;
using Luna.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Luna.UI.LayoutSystem;

namespace Luna.UI
{
    internal class UITheme
    {
        private Color mainColour;
        private Color secondaryColour;
        private Color hoveredColour;
        private Color selectedColour;
        private (int topLeft, int topRight, int bottomLeft, int bottomRight) cornerRadius;
        private Texture2D tlTexture, trTexture, blTexture, brTexture;

        public Color MainColour
        {
            get { return mainColour; }
            set { mainColour = value; }
        }

        public Color SecondaryColour
        {
            get { return secondaryColour; }
            set { secondaryColour = value; }
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

        public (int TopLeft, int TopRight, int BottomLeft, int BottomRight) CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                tlTexture = GraphicsHelper.GenerateCircleTexture(value.TopLeft * 2);
                trTexture = GraphicsHelper.GenerateCircleTexture(value.TopRight * 2);
                blTexture = GraphicsHelper.GenerateCircleTexture(value.BottomLeft * 2);
                brTexture = GraphicsHelper.GenerateCircleTexture(value.BottomRight * 2);
            }
        }

        public Texture2D TopLeftTexture
        {
            get { return tlTexture; }
        }

        public Texture2D TopRightTexture
        {
            get { return trTexture; }
        }

        public Texture2D BottomLeftTexture
        {
            get { return blTexture; }
        }

        public Texture2D BottomRightTexture
        {
            get { return brTexture; }
        }

        public static Rectangle TopLeftRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = theme.cornerRadius.topLeft;
            return new Rectangle(componentRect.X, componentRect.Y, radius, radius);
        }

        public static Rectangle TopRightRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = theme.cornerRadius.topRight;
            return new Rectangle(componentRect.X + componentRect.Width - radius, componentRect.Y, radius, radius);
        }

        public static Rectangle BottomLeftRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = theme.cornerRadius.bottomLeft;
            return new Rectangle(componentRect.X, componentRect.Y + componentRect.Height - radius, radius, radius);
        }

        public static Rectangle BottomRightRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = theme.cornerRadius.bottomRight;
            return new Rectangle(componentRect.X + componentRect.Width - radius, componentRect.Y + componentRect.Height - radius, radius, radius);
        }

        public static Rectangle LeftRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int topRadius = theme.cornerRadius.topLeft;
            int bottomRadius = theme.cornerRadius.bottomLeft;
            return new Rectangle(componentRect.X, componentRect.Y + topRadius, Math.Max(topRadius, bottomRadius), componentRect.Height - topRadius - bottomRadius);
        }

        public static Rectangle TopRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int leftRadius = theme.cornerRadius.topLeft;
            int rightRadius = theme.cornerRadius.topRight;
            return new Rectangle(componentRect.X + leftRadius, componentRect.Y, componentRect.Width - leftRadius - rightRadius, Math.Max(leftRadius, rightRadius));
        }

        public static Rectangle BottomRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int leftRadius = theme.cornerRadius.bottomLeft;
            int rightRadius = theme.cornerRadius.bottomRight;
            return new Rectangle(componentRect.X + leftRadius, componentRect.Y + componentRect.Height - Math.Max(leftRadius, rightRadius), componentRect.Width - leftRadius - rightRadius, Math.Max(leftRadius, rightRadius));
        }

        public static Rectangle RightRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int topRadius = theme.cornerRadius.topRight;
            int bottomRadius = theme.cornerRadius.bottomRight;
            return new Rectangle(componentRect.X + componentRect.Width - Math.Max(topRadius, bottomRadius), componentRect.Y + topRadius, Math.Max(topRadius, bottomRadius), componentRect.Height - topRadius - bottomRadius);
        }

        public static Rectangle CentreRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int tlRadius = theme.cornerRadius.topLeft;
            int trRadius = theme.cornerRadius.topRight;
            int blRadius = theme.cornerRadius.bottomLeft;
            int brRadius = theme.cornerRadius.bottomRight;

            int left = Math.Max(tlRadius, blRadius);
            int right = Math.Max(trRadius, brRadius);
            int top = Math.Max(tlRadius, trRadius);
            int bottom = Math.Max(blRadius, brRadius);

            return new Rectangle(componentRect.X + left, componentRect.Y + top, componentRect.Width - right - left, componentRect.Height - bottom - top);
        }

        public static Rectangle[] FillRectangles(UITheme theme, UITransform transform)
        {
            return [TopRect(theme, transform), BottomRect(theme, transform), LeftRect(theme, transform), RightRect(theme, transform), CentreRect(theme, transform)];
        }
    }
}
