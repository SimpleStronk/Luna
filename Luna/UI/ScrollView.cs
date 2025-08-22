using System;
using Luna.UI.LayoutSystem;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class ScrollView : BlankUI
    {
        BlankUI container;
        ScrollBar scrollBar;

        public ScrollView(UITheme.ColorType colorType) : base(colorType)
        {
            overrideTheme.ColourType = colorType;
            // Set this object's layout to Horizontal (this is independent of the layout
            // within the scrollview content and is configured as such to arrange the view
            // and the vertical scrollbar
            layout.LayoutAxis = LVector2.HORIZONTAL;
            layout.Spacing = 0;

            Initialise();

            // Create the container element, this will hold all the child elements
            container = new BlankUI(UITheme.ColorType.Placeholder);
            container.Name = "ScrollView Container";
            
            // Fill the bounds horizontally, and match content vertically
            container.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap()
            });
            container.Scrollable = true;

            // Make sure scrollbar changes position when we scroll the mouse
            container.GetTransform().OnScrollChanged((float value) => scrollBar.SoftSetValue(value));

            // Create the scrollbar element
            scrollBar = new ScrollBar(LVector2.VERTICAL, UITheme.ColorType.Placeholder);
            scrollBar.Name = "ScrollView Scroll Bar";
            scrollBar.SetLayout(new Layout()
            {
                LayoutHeight = Sizing.Grow(1)
            });
            // Make sure this element's scroll value changes when we move the scrollbar
            scrollBar.OnValueChanged((float value) => container.GetTransform().SetScrollRatio(value));

            base.AddChild(container, scrollBar);
        }

        protected override void Update()
        {
            base.Update();

            if (container.GetTransform().IsOverflowing)
            {
                // Make scrollbar 15 pixels wide and reflect the size of the visible area relative to
                // the total size of the content
                scrollBar.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed(15) });
                scrollBar.SetHandleSizeRatio(1f / container.GetTransform().GetOverflowRatio());
            }
            else
            {
                // Hide the scrollbar by making it 0 pixels wide
                scrollBar.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed(0) });
            }
        }

        // Override base AddChild function and make it add children to the container element
        // instead of itself
        public override void AddChild(params UIComponent[] children)
        {
            container.AddChild(children);
        }

        public BlankUI Container
        {
            get { return container; }
        }
    }
}