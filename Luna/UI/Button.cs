﻿
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
        protected enum ButtonState { None, Hovered, Selected };
        protected ButtonState buttonState = ButtonState.None;
        public enum VisualResponse { None, ColourChange };

        public Button(UITheme.ColorType colourType)
        {
            AddVisualResponse();
            overrideTheme.ColourType = colourType;
            overrideTheme.Rounded = true;
            Initialise();
        }

        public Button(VisualResponse visualResponse, UITheme.ColorType colourType)
        {
            if (visualResponse == VisualResponse.ColourChange) AddVisualResponse();
            overrideTheme.ColourType = colourType;
            overrideTheme.Rounded = true;
            Initialise();
        }

        protected override void Update()
        {
            if (hovered) if (IsJustClicked(MouseButton.Left)) SetClicked(true);
            if (clicked) if (IsJustUnclicked(MouseButton.Left)) { SetClicked(false); }

            switch (buttonState)
            {
                case ButtonState.None:
                {
                    colourAnimator.SetColour(overrideTheme.GetColourPalatte(cascadeTheme).MainColour);
                    break;
                }
                case ButtonState.Hovered:
                {
                    colourAnimator.SetColour(overrideTheme.GetColourPalatte(cascadeTheme).HoveredColour);
                    break;
                }
                case ButtonState.Selected:
                {
                    colourAnimator.SetColour(overrideTheme.GetColourPalatte(cascadeTheme).SelectedColour);
                    break;
                }
            }
        }

        protected override void Draw(SpriteBatch s)
        {
            //s.Draw
        }

        protected void AddVisualResponse()
        {
            onHover += () => { if (clicked) return; buttonState = ButtonState.Hovered; colourAnimator.SetColour(overrideTheme.GetColourPalatte(cascadeTheme).HoveredColour); };
            onClick += () => { clicked = true; buttonState = ButtonState.Selected; };
            onUnhover += () => { if (!clicked) buttonState = ButtonState.None; };
            onUnclick += () => { buttonState = hovered ? ButtonState.Hovered : ButtonState.None; clicked = false; };
        }

        private void SetClicked(bool clicked)
        {
            this.clicked = clicked;
            if (!clicked) { onUnclick?.Invoke(); return; }

            if (!focused) return;

            onClick?.Invoke();
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
