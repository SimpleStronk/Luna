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
            layout.LayoutAxis = LVector2.HORIZONTAL;
            layout.Spacing = 0;

            Initialise();

            container = new BlankUI(UITheme.ColorType.Background);
            container.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap()
            });
            container.Scrollable = true;

            container.GetTransform().OnScrollChanged((float value) => scrollBar.SoftSetValue(value));

            scrollBar = new ScrollBar(LVector2.VERTICAL, UITheme.ColorType.Placeholder);
            scrollBar.SetLayout(new Layout()
            {
                LayoutHeight = Sizing.Grow(1)
            });
            scrollBar.OnValueChanged((float value) => container.GetTransform().SetScrollRatio(value));

            base.AddChild(container, scrollBar);
        }

        protected override void Update()
        {
            base.Update();

            if (container.GetTransform().IsOverflowing)
            {
                scrollBar.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed(15) });
                scrollBar.SetHandleSizeRatio(1f / container.GetTransform().GetOverflowRatio());
            }
            else
            {
                scrollBar.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed(0) });
            }
        }

        public override void AddChild(params UIComponent[] children)
        {
            container.AddChild(children);
        }
    }
}