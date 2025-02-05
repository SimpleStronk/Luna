using System;
using System.Windows.Forms.Design;
using Luna.HelperClasses;
using Luna.ManagerClasses;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class TextInput : Button
    {
        private BlankUI internalPanel;
        private Label label;
        private bool editing = false;
        private string text = "";
        private bool multiline = false;

        public TextInput() : base(UITheme.ColorType.Background)
        {
            overrideTheme.ColourType = UITheme.ColorType.Background;
            layout.Padding = new Tetra(2);

            internalPanel = new BlankUI(UITheme.ColorType.Background);
            internalPanel.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1), LayoutHeight = Sizing.Grow(1), HorizontalAlignment = Alignment.Middle, VerticalAlignment = Alignment.Middle });
            internalPanel.SetTheme(new UITheme() { Rounded = true });
            internalPanel.FocusIgnore = true;
            AddChild(internalPanel);

            label = new Label("", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);

            internalPanel.AddChild(label);

            OnClick(() => editing = true );

            Initialise();
        }

        protected override void Update()
        {
            base.Update();

            if (MouseHandler.IsJustClicked(MouseHandler.MouseButton.Left) && !clicked)
            {
                editing = false;
            }

            if (editing)
            {
                EditText();   
            }
        }

        private void EditText()
        {
            char c;
            if (KeyboardHandler.TryConvertKeyboardInput(out c))
            {
                switch (c)
                {
                    case '\b':
                    {
                        text = text.Substring(0, Math.Max(text.Length - 1, 0));
                        break;
                    }
                    case '\n':
                    {
                        if (multiline) text += '\n';
                        else editing = false;
                        break;
                    }
                    default:
                    {
                        text += c;
                        break;
                    }
                }
                label.SetText(text);
            }
        }
    }
}