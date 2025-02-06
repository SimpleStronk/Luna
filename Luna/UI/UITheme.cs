using System;
using System.Windows.Forms.VisualStyles;
using Luna.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Luna.UI.LayoutSystem;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using SharpDX.MediaFoundation.DirectX;
using System.Collections;

namespace Luna.UI
{
    internal class UITheme
    {
        private ColourPalatte mainColour = new();
        private ColourPalatte mainColourSoft = new();
        private ColourPalatte highlitColour = new();
        private ColourPalatte backgroundColour = new();
        private ColourPalatte emergencyColour = new();
        private ColourPalatte separatorColour = new();
        private ColourPalatte shadowColour = new();
        private ColourPalatte scrollbarColour = new();
        private bool rounded;
        public enum ColorType { Main, MainSoft, Highlit, Background, Emergency, Separator, Shadow, ScrollBar, Placeholder };
        private ColorType colourType = ColorType.Background;
        private (int topLeft, int topRight, int bottomLeft, int bottomRight) cornerRadius;
        private bool mainColourChanged, mainColourSoftChanged, highlitColourChanged, backgroundColourChanged, emergencyColourChanged,
            separatorColourChanged, shadowColourChanged, scrollbarColourChanged, roundedChanged, colourTypeChanged, cornerRadiusChanged;

        public ColourPalatte MainColour
        {
            get { return mainColour; }
            set { mainColour = value; mainColourChanged = true; }
        }
        
        public ColourPalatte MainColourSoft
        {
            get { return mainColourSoft; }
            set { mainColourSoft = value; mainColourSoftChanged = true; }
        }

        public ColourPalatte HighlitColour
        {
            get { return highlitColour; }
            set { highlitColour = value; }
        }

        public ColourPalatte BackgroundColour
        {
            get { return backgroundColour; }
            set { backgroundColour = value; backgroundColourChanged = true; }
        }

        public ColourPalatte EmergencyColour
        {
            get { return emergencyColour; }
            set { emergencyColour = value; emergencyColourChanged = true; }
        }

        public ColourPalatte SeparatorColour
        {
            get { return separatorColour; }
            set { separatorColour = value; separatorColourChanged = true; }
        }

        public ColourPalatte ShadowColour
        {
            get { return shadowColour; }
            set { shadowColour = value; shadowColourChanged = true; }
        }

        public ColourPalatte ScrollbarColour
        {
            get { return scrollbarColour; }
            set { scrollbarColour = value; }
        }

        public bool Rounded
        {
            get { return rounded; }
            set { rounded = value; roundedChanged = true; }
        }

        public ColorType ColourType
        {
            get { return colourType; }
            set { colourType = value; colourTypeChanged = true; }
        }

        public ColourPalatte GetColourPalatte(UITheme cascadeTheme)
        {
            if (cascadeTheme == null)
            {
                switch (colourType)
                {
                    case ColorType.Main: return mainColour;
                    case ColorType.MainSoft: return mainColourSoft;
                    case ColorType.Highlit: return highlitColour;
                    case ColorType.Background: return backgroundColour;
                    case ColorType.Emergency: return emergencyColour;
                    case ColorType.Shadow: return shadowColour;
                    case ColorType.Separator: return separatorColour;
                    case ColorType.ScrollBar: return scrollbarColour;
                    case ColorType.Placeholder: return ColourPalatte.Transparent;
                }
                return new();
            }

            switch (colourTypeChanged ? colourType : cascadeTheme.colourType)
            {
                case ColorType.Main: return mainColourChanged ? mainColour : cascadeTheme.mainColour;
                case ColorType.MainSoft: return mainColourSoftChanged ? mainColourSoft : cascadeTheme.mainColourSoft;
                case ColorType.Highlit: return highlitColourChanged ? highlitColour : cascadeTheme.highlitColour;
                case ColorType.Background: return backgroundColourChanged ? backgroundColour : cascadeTheme.backgroundColour;
                case ColorType.Emergency: return emergencyColourChanged ? emergencyColour : cascadeTheme.emergencyColour;
                case ColorType.Shadow: return shadowColourChanged ? shadowColour : cascadeTheme.shadowColour;
                case ColorType.Separator: return separatorColourChanged ? separatorColour : cascadeTheme.separatorColour;
                case ColorType.ScrollBar: return scrollbarColourChanged ? scrollbarColour : cascadeTheme.scrollbarColour;
                case ColorType.Placeholder: return ColourPalatte.Transparent;
            }

            return new();
        }

        public (int TopLeft, int TopRight, int BottomLeft, int BottomRight) CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                cornerRadiusChanged = true;
            }
        }

        public static Texture2D TopLeftTexture(UITheme theme, UITransform transform)
        {
            int radius = ClipRadius(theme.cornerRadius.topLeft, transform);
            return GraphicsHelper.GenerateCircleTexture(radius * 2);
        }

        public static Texture2D TopRightTexture(UITheme theme, UITransform transform)
        {
            int radius = ClipRadius(theme.cornerRadius.topRight, transform);
            return GraphicsHelper.GenerateCircleTexture(radius * 2);
        }

        public static Texture2D BottomLeftTexture(UITheme theme, UITransform transform)
        {
            int radius = ClipRadius(theme.cornerRadius.bottomLeft, transform);
            return GraphicsHelper.GenerateCircleTexture(radius * 2);
        }

        public static Texture2D BottomRightTexture(UITheme theme, UITransform transform)
        {
            int radius = ClipRadius(theme.cornerRadius.bottomRight, transform);
            return GraphicsHelper.GenerateCircleTexture(radius * 2);
        }

        public bool MainColourChanged
        {
            get { return mainColourChanged; }
        }

        public bool MainColourSoftChanged
        {
            get { return mainColourSoftChanged; }
        }

        public bool HighlitColourChanged
        {
            get { return highlitColourChanged; }
        }

        public bool BackgroundColourChanged
        {
            get { return backgroundColourChanged; }
        }

        public bool EmergencyColourChanged
        {
            get { return emergencyColourChanged; }
        }

        public bool SeparatorColourChanged
        {
            get { return separatorColourChanged; }
        }

        public bool ShadowColourChanged
        {
            get { return shadowColourChanged; }
        }

        public bool ScrollbarColourChanged
        {
            get { return scrollbarColourChanged; }
        }

        public bool RoundedChanged
        {
            get { return roundedChanged; }
        }

        public bool ColourTypeChanged
        {
            get { return colourTypeChanged; }
        }

        public bool CornerRadiusChanged
        {
            get { return cornerRadiusChanged; }
        }

        public static (int topLeft, int topRight, int bottomLeft, int bottomRight) GetCornerRadius(UITheme theme, UITransform transform)
        {
            return (ClipRadius(theme.cornerRadius.topLeft, transform), ClipRadius(theme.cornerRadius.topRight, transform),
                ClipRadius(theme.cornerRadius.bottomLeft, transform), ClipRadius(theme.cornerRadius.bottomRight, transform));
        }

        public void UpdateTheme(UITheme theme)
        {
            if (theme.mainColourChanged) MainColour = theme.mainColour;
            if (theme.mainColourSoftChanged) MainColourSoft = theme.mainColourSoft;
            if (theme.highlitColourChanged) HighlitColour = theme.highlitColour;
            if (theme.backgroundColourChanged) BackgroundColour = theme.backgroundColour;
            if (theme.emergencyColourChanged) EmergencyColour = theme.emergencyColour;
            if (theme.separatorColourChanged) SeparatorColour = theme.separatorColour;
            if (theme.shadowColourChanged) ShadowColour = theme.shadowColour;
            if (theme.roundedChanged) Rounded = theme.rounded;
            if (theme.colourTypeChanged) ColourType = theme.colourType;
            if (theme.cornerRadiusChanged) CornerRadius = theme.cornerRadius;
        }

        public static Rectangle TopLeftRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = ClipRadius(theme.cornerRadius.topLeft, transform);
            return new Rectangle(componentRect.X, componentRect.Y, radius, radius);
        }

        public static Rectangle TopRightRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = ClipRadius(theme.cornerRadius.topRight, transform);
            return new Rectangle(componentRect.X + componentRect.Width - radius, componentRect.Y, radius, radius);
        }

        public static Rectangle BottomLeftRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = ClipRadius(theme.cornerRadius.bottomLeft, transform);
            return new Rectangle(componentRect.X, componentRect.Y + componentRect.Height - radius, radius, radius);
        }

        public static Rectangle BottomRightRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int radius = ClipRadius(theme.cornerRadius.bottomRight, transform);
            return new Rectangle(componentRect.X + componentRect.Width - radius, componentRect.Y + componentRect.Height - radius, radius, radius);
        }

        public static Rectangle LeftRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int topRadius = ClipRadius(theme.cornerRadius.topLeft, transform);
            int bottomRadius = ClipRadius(theme.cornerRadius.bottomLeft, transform);
            return new Rectangle(componentRect.X, componentRect.Y + topRadius, Math.Max(topRadius, bottomRadius), componentRect.Height - topRadius - bottomRadius);
        }

        public static Rectangle TopRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int leftRadius = ClipRadius(theme.cornerRadius.topLeft, transform);
            int rightRadius = ClipRadius(theme.cornerRadius.topRight, transform);
            return new Rectangle(componentRect.X + leftRadius, componentRect.Y, componentRect.Width - leftRadius - rightRadius, Math.Max(leftRadius, rightRadius));
        }

        public static Rectangle BottomRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int leftRadius = ClipRadius(theme.cornerRadius.bottomLeft, transform);
            int rightRadius = ClipRadius(theme.cornerRadius.bottomRight, transform);
            return new Rectangle(componentRect.X + leftRadius, componentRect.Y + componentRect.Height - Math.Max(leftRadius, rightRadius), componentRect.Width - leftRadius - rightRadius, Math.Max(leftRadius, rightRadius));
        }

        public static Rectangle RightRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int topRadius = ClipRadius(theme.cornerRadius.topRight, transform);
            int bottomRadius = ClipRadius(theme.cornerRadius.bottomRight, transform);
            return new Rectangle(componentRect.X + componentRect.Width - Math.Max(topRadius, bottomRadius), componentRect.Y + topRadius, Math.Max(topRadius, bottomRadius), componentRect.Height - topRadius - bottomRadius);
        }

        public static Rectangle CentreRect(UITheme theme, UITransform transform)
        {
            Rectangle componentRect = transform.GetGlobalRect();
            int tlRadius = ClipRadius(theme.cornerRadius.topLeft, transform);
            int trRadius = ClipRadius(theme.cornerRadius.topRight, transform);
            int blRadius = ClipRadius(theme.cornerRadius.bottomLeft, transform);
            int brRadius = ClipRadius(theme.cornerRadius.bottomRight, transform);

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

        private static int ClipRadius(int radius, UITransform transform)
        {
            return Math.Min(radius, transform.MinDimension / 2);
        }
    }
}
