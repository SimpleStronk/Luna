
using Luna.HelperClasses;
using Luna.ManagerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Luna.ManagerClasses.MouseHandler;

namespace Luna.UI
{
    internal class Button : UIComponent
    {
        protected bool clicked;
        protected Action onClick, onUnclick;
        enum ButtonState { None, Hovered, Selected };
        ButtonState buttonState = ButtonState.None;

        public Button(UITheme theme, UITheme.ColorType colourType)
        {
            UITheme tmp = theme;
            tmp.ColourType = colourType;
            tmp.Rounded = true;
            SetTheme(tmp);
            onHover += () => colour = theme.GetColour().ScaleValue(theme.HoverValue);
            onClick += () => colour = theme.GetColour().ScaleValue(theme.SelectValue);
            onUnhover += () => { if (!clicked) colour = theme.GetColour(); };
            onUnclick += () => { colour = hovered ? theme.GetColour().ScaleValue(theme.HoverValue) : theme.GetColour(); };
            Initialise();
        }

        protected override void Update()
        {
            if (hovered) if (IsJustClicked(MouseButton.Left)) SetClicked(true);
            if (clicked) if (IsJustUnclicked(MouseButton.Left)) SetClicked(false);
        }

        protected override void Draw(SpriteBatch s)
        {
            //s.Draw
        }

        private void SetClicked(bool clicked)
        {
            if (!focused) return;

            if (clicked) onClick?.Invoke();
            else onUnclick?.Invoke();
        }

        public void OnClick(Action e)
        {
            onClick += e;
        }

        public void OnUnclick(Action e)
        {
            onUnclick += e;
        }

        protected override void SetFocused(bool focused)
        {
            base.SetFocused(focused);

            if (!focused) clicked = false;
        }

        protected override void DebugAction(string action)
        {
            Console.WriteLine($"Button: {action}");
        }

        protected override string GetComponentType()
        {
            return "Button";
        }
    }
}
