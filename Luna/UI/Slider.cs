using System;
using Luna.ManagerClasses;
using Luna.UI.LayoutSystem;
using SharpDX.XAudio2;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class Slider : Button, ISlidable
    {
        private BlankUI sliderGroove;
        private BlankUI sliderKnob;
        private Action<float> onValueChanged;
        int axis;
        float minimumValue = 0;
        float maximumValue = 1;
        float increment = 0;
        float normalisedValue = 0;
        float value = 0;
        int knobSize = 30;

        public Slider(int axis, float minimumValue, float maximumValue, float increment, UITheme.ColorType colorType) : base(VisualResponse.ColourChange, colorType)
        {
            this.axis = axis;
            this.minimumValue = minimumValue;
            this.maximumValue = maximumValue;
            this.increment = increment;
            overrideTheme.ColourType = colorType;
            layout.HorizontalAlignment = layout.VerticalAlignment = Alignment.Middle;
            layout.Padding = new Tetra(15);

            sliderGroove = new BlankUI(UITheme.ColorType.Separator);
            sliderGroove.FocusIgnore = true;

            switch (axis)
            {
                case LVector2.HORIZONTAL: { layout.LayoutHeight = Sizing.Fixed(40);
                    sliderGroove.SetLayout(new Layout()
                    {
                        LayoutWidth = Sizing.Grow(1),
                        LayoutHeight = Sizing.Fixed(2),
                        HorizontalAlignment = Alignment.Ignore,
                        VerticalAlignment = Alignment.Middle,
                        ClipChildren = false
                        }); break; }
                case LVector2.VERTICAL: { layout.LayoutWidth = Sizing.Fixed(40);
                    sliderGroove.SetLayout(new Layout()
                    {
                        LayoutWidth = Sizing.Fixed(2),
                        LayoutHeight = Sizing.Grow(1),
                        HorizontalAlignment = Alignment.Middle,
                        VerticalAlignment = Alignment.Ignore,
                        ClipChildren = false
                    }); break; }
            }

            sliderKnob = new BlankUI(UITheme.ColorType.Main);
            sliderKnob.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed(knobSize), LayoutHeight = Sizing.Fixed(knobSize), Inline = false });
            sliderKnob.SetTheme(new UITheme() { Rounded = true });
            sliderKnob.FocusIgnore = true;

            sliderGroove.AddChild(sliderKnob);
            AddChild(sliderGroove);
            
            Initialise();
        }

        protected override void Update()
        {
            base.Update();

            if (clicked)
            {
                HardSetValue(NormalisedToScaledValue(Math.Clamp(CalculateNormalisedValue(MouseHandler.Position, axis), 0, 1)));
            }

            value = NormalisedToScaledValue(normalisedValue);
            SetKnobPosition(normalisedValue);
        }

        private void SetKnobPosition(float value)
        {
            float groovePosition = sliderGroove.GetTransform().GlobalPosition.GetComponent(axis);
            float grooveSize = sliderGroove.GetTransform().Size.GetComponent(axis);
            float position = (value * grooveSize) + groovePosition - (knobSize / 2f);

            sliderKnob.GetTransform().SetGlobalPositionComponentValue(position, axis);
        }

        private float CalculateNormalisedValue(LVector2 mousePosition, int axis)
        {
            float groovePosition = sliderGroove.GetTransform().GlobalPosition.GetComponent(axis);
            float grooveSize = sliderGroove.GetTransform().Size.GetComponent(axis);

            return (mousePosition.GetComponent(axis) - groovePosition) / grooveSize;
        }

        private float NormalisedToScaledValue(float normalisedValue)
        {
            return minimumValue + (normalisedValue * (maximumValue - minimumValue));
        }

        private float CalculateNormalisedIncrement()
        {
            return increment / (maximumValue - minimumValue);
        }

        public void OnValueChanged(Action<float> e)
        {
            onValueChanged += e;
        }

        public float MinimumValue
        {
            get { return minimumValue; }
            set { minimumValue = value; }
        }

        public float MaximumValue
        {
            get { return maximumValue; }
            set { maximumValue = value; }
        }

        public float Increment
        {
            get { return increment; }
            set { increment = value; }
        }

        public float GetValue()
        {
            return minimumValue + (normalisedValue * (maximumValue - minimumValue));
        }

        public void SoftSetValue(float value)
        {
            float tmp = (value - minimumValue) / (maximumValue - minimumValue);
            if (tmp == normalisedValue) return;

            normalisedValue = tmp;

            if (increment == 0) { normalisedValue = Math.Clamp(normalisedValue, 0, 1); }
            else { normalisedValue = Math.Clamp(((int)(normalisedValue / CalculateNormalisedIncrement() + 0.5f)) * CalculateNormalisedIncrement(), 0, 1); }
        }

        public void HardSetValue(float value)
        {
            SoftSetValue(value);
            onValueChanged?.Invoke(GetValue());
        }
    }
}