
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

        /// <summary>
        /// Creates a new Button with the given colour type, and a visual response
        /// </summary>
        public Button(UITheme.ColorType colourType) : base(false)
        {
            AddVisualResponse();
            overrideTheme.ColourType = colourType;
            overrideTheme.Rounded = true;
            Initialise();
        }

        /// <summary>
        /// Creates a new Button with the given colour type and the given visual response
        /// </summary>
        public Button(VisualResponse visualResponse, UITheme.ColorType colourType) : base(false)
        {
            if (visualResponse == VisualResponse.ColourChange) AddVisualResponse();
            overrideTheme.ColourType = colourType;
            overrideTheme.Rounded = true;
            Initialise();
        }

        protected override void Update()
        {
            // Click logic
            if (hovered) if (IsJustClicked(MouseButton.Left)) SetClicked(true);
            if (clicked) if (IsJustUnclicked(MouseButton.Left)) { SetClicked(false); }

            // Make sure button has the right colour type
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

        /// <summary>
        /// Link visual activity to actions happening to the button
        /// </summary>
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

            // Give unclick response if mouse is up
            if (!clicked) { onUnclick?.Invoke(); return; }

            // Don't give click response if not focused
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
