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

namespace Luna.UI
{
    internal class UITheme
    {
        private ColourPalette mainColour = new();
        private ColourPalette mainColourSoft = new();
        private ColourPalette backgroundColour = new();
        private ColourPalette emergencyColour = new();
        private ColourPalette separatorColour = new();
        private ColourPalette shadowColour = new();
        private bool rounded;
        public enum ColorType { Main, MainSoft, Background, Emergency, Shadow, Separator, Placeholder };
        private ColorType colourType = ColorType.Background;
        private (int topLeft, int topRight, int bottomLeft, int bottomRight) cornerRadius;
        private Texture2D tlTexture, trTexture, blTexture, brTexture;
        private bool mainColourChanged, mainColourSoftChanged, backgroundColourChanged, emergencyColourChanged,
            separatorColourChanged, shadowColourChanged, roundedChanged, cornerRadiusChanged;

        public ColourPalette MainColour
        {
            get { return mainColour; }
            set { mainColour = value; mainColourChanged = true; }
        }
        
        public ColourPalette MainColourSoft
        {
            get { return mainColourSoft; }
            set { mainColourSoft = value; mainColourSoftChanged = true; }
        }

        public ColourPalette BackgroundColour
        {
            get { return backgroundColour; }
            set { backgroundColour = value; backgroundColourChanged = true; }
        }

        public ColourPalette EmergencyColour
        {
            get { return emergencyColour; }
            set { emergencyColour = value; emergencyColourChanged = true; }
        }

        public ColourPalette SeparatorColour
        {
            get { return separatorColour; }
            set { separatorColour = value; separatorColourChanged = true; }
        }

        public ColourPalette ShadowColour
        {
            get { return shadowColour; }
            set { shadowColour = value; shadowColourChanged = true; }
        }

        public bool Rounded
        {
            get { return rounded; }
            set { rounded = value; roundedChanged = true; }
        }

        public ColorType ColourType
        {
            get { return colourType; }
            set { colourType = value; }
        }

        public ColourPalette GetColourPalette(UITheme cascadeTheme)
        {
            if (cascadeTheme == null)
            {
                switch (colourType)
                {
                    case ColorType.Main: return mainColour;
                    case ColorType.MainSoft: return mainColourSoft;
                    case ColorType.Background: return backgroundColour;
                    case ColorType.Emergency: return emergencyColour;
                    case ColorType.Shadow: return shadowColour;
                    case ColorType.Separator: return separatorColour;
                    case ColorType.Placeholder: return ColourPalette.Transparent;
                }
                return new();
            }

            switch (colourType)
            {
                case ColorType.Main: return mainColourChanged ? mainColour : cascadeTheme.mainColour;
                case ColorType.MainSoft: return mainColourSoftChanged ? mainColourSoft : cascadeTheme.mainColourSoft;
                case ColorType.Background: return backgroundColourChanged ? backgroundColour : cascadeTheme.backgroundColour;
                case ColorType.Emergency: return emergencyColourChanged ? emergencyColour : cascadeTheme.emergencyColour;
                case ColorType.Shadow: return shadowColourChanged ? shadowColour : cascadeTheme.shadowColour;
                case ColorType.Separator: return separatorColourChanged ? separatorColour : cascadeTheme.separatorColour;
                case ColorType.Placeholder: return ColourPalette.Transparent;
            }

            return new();
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
                cornerRadiusChanged = true;
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

        public bool MainColourChanged
        {
            get { return mainColourChanged; }
        }

        public bool MainColourSoftChanged
        {
            get { return mainColourSoftChanged; }
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

        public bool RoundedChanged
        {
            get { return roundedChanged; }
        }

        public bool CornerRadiusChanged
        {
            get { return cornerRadiusChanged; }
        }

        public void UpdateTheme(UITheme theme)
        {
            if (theme.mainColourChanged) mainColour = theme.mainColour;
            if (theme.mainColourSoftChanged) mainColourSoft = theme.mainColourSoft;
            if (theme.backgroundColourChanged) backgroundColour = theme.backgroundColour;
            if (theme.emergencyColourChanged) emergencyColour = theme.emergencyColour;
            if (theme.separatorColourChanged) separatorColour = theme.separatorColour;
            if (theme.shadowColourChanged) shadowColour = theme.shadowColour;
            if (theme.roundedChanged) rounded = theme.rounded;
            if (theme.cornerRadiusChanged) cornerRadius = theme.cornerRadius;
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
