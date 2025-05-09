using System;
using System.Windows.Forms.Design;
using Luna.HelperClasses;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class Toggle : Button
    {
        private BlankUI indicatorContainer;
        private BlankUI indicator;
        private Label label;
        private bool isOn;
        private Action<bool> onValueChanged;

        public Toggle(UITheme.ColorType colourType) : base(VisualResponse.ColourChange, colourType)
        {
            SetLayout(new Layout()
            {
                HorizontalAlignment = Alignment.Begin,
                VerticalAlignment = Alignment.Middle,
                Padding = new Tetra(10),
                Spacing = 10
            });
            overrideTheme.ColourType = colourType;
            overrideTheme.Rounded = true;
            onClick += () => { IsOn = !IsOn; };
            Initialise();

            indicatorContainer = new BlankUI(UITheme.ColorType.Separator);
            indicatorContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(50),
                LayoutHeight = Sizing.Fixed(30),
                HorizontalAlignment = Alignment.Begin,
                Padding = new Tetra(5)
            });
            indicatorContainer.SetTheme(new UITheme() { Rounded = true });
            indicatorContainer.FocusIgnore = true;

            indicator = new BlankUI(UITheme.ColorType.MainSoft);
            indicator.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(20),
                LayoutHeight = Sizing.Grow(1),
            });
            indicator.SetTheme(new UITheme(){ Rounded = true });
            indicator.FocusIgnore = true;

            indicatorContainer.AddChild(indicator);

            label = new Label("", GraphicsHelper.GetDefaultFont(), colourType);
            label.SetText("Testing, testing, 123");

            AddChild(indicatorContainer);
            AddChild(label);
        }

        public bool IsOn
        {
            get { return isOn; }
            set { SetOn(value); }
        }

        private void SetOn(bool isOn)
        {
            if (this.isOn == isOn) return;

            this.isOn = isOn;
            onValueChanged?.Invoke(isOn);

            indicatorContainer.SetLayout(new Layout() { HorizontalAlignment = isOn ? Alignment.End : Alignment.Begin });

            indicator.SetTheme(new UITheme() { ColourType = isOn ? UITheme.ColorType.Main : UITheme.ColorType.MainSoft });
        }

        public void OnValueChanged(Action<bool> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
        }

        public Label Label
        {
            get { return label; }
        }
    }
}