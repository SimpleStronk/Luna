using System;
using Luna.UI.LayoutSystem;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class ScrollView : BlankUI
    {
        BlankUI container;
        Slider slider;

        public ScrollView(UITheme.ColorType colorType) : base(colorType)
        {
            overrideTheme.ColourType = colorType;
            layout.LayoutAxis = LVector2.HORIZONTAL;

            Initialise();

            container = new BlankUI(UITheme.ColorType.Background);
            container.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap()
            });
            container.Scrollable = true;

            // NOT WORKING
            //container.GetTransform().OnScrollChanged((float value) => slider.SoftSetValue(value));

            slider = new Slider(LVector2.VERTICAL, UITheme.ColorType.Background);
            slider.SetLayout(new Layout()
            {
                LayoutHeight = Sizing.Grow(1)
            });
            slider.MinimumValue = 0;
            slider.MaximumValue = 1;
            slider.Increment = 0;
            slider.OnValueChanged((float value) => container.GetTransform().SetScrollRatio(value));

            base.AddChild(container, slider);
        }

        public override void AddChild(params UIComponent[] children)
        {
            container.AddChild(children);
        }
    }
}