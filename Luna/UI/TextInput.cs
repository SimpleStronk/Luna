using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms.Design;
using Luna.HelperClasses;
using Luna.ManagerClasses;
using Luna.UI.LayoutSystem;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class TextInput : Button
    {
        private BlankUI internalPanel;
        private BlankUI caret;
        private Label label;
        private bool editing = false;
        private string text = "";
        private bool multiline = false;
        private int maxCharacters = 30;
        private (int character, int line) caretIndex;

        private Action<(int, int)> onCaretChanged;

        public TextInput() : base(UITheme.ColorType.Background)
        {
            overrideTheme.ColourType = UITheme.ColorType.Background;
            layout.Padding = new Tetra(2);

            internalPanel = new BlankUI(UITheme.ColorType.Background);
            internalPanel.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1), LayoutHeight = Sizing.Wrap(), HorizontalAlignment = Alignment.Middle, VerticalAlignment = Alignment.Middle, Padding = new Tetra(10) });
            internalPanel.SetTheme(new UITheme() { CornerRadius = (8, 8, 8, 8), Rounded = true });
            internalPanel.FocusIgnore = true;
            AddChild(internalPanel);

            label = new Label("", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            label.SetLayout(new Layout() { HorizontalAlignment = Alignment.Ignore, VerticalAlignment = Alignment.Ignore, ClipChildren = false });

            caret = new BlankUI(UITheme.ColorType.Main);
            caret.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed(2), LayoutHeight = Sizing.Fixed(30) });
            label.AddChild(caret);

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
                        RemoveCharacter();
                        break;
                    }
                    case '\n':
                    {
                        if (multiline) AddCharacter('\n');
                        else editing = false;
                        break;
                    }
                    // LEFT KEY
                    case (char)0:
                    {
                        MoveCaretLeft();
                        break;
                    }
                    // RIGHT KEY
                    case (char)1:
                    {
                        MoveCaretRight(false, false);
                        break;
                    }
                    case (char)2:
                    {
                        // TODO - IMPLEMENT UP KEY
                        break;
                    }
                    case (char)3:
                    {
                        // TODO - IMPLEMENT DOWN KEY
                        break;
                    }
                    default:
                    {
                        AddCharacter(c);
                        break;
                    }
                }
                label.SetText(text);
            }
        }

        private void AddCharacter(char c)
        {
            if (text.Length >= maxCharacters) return;

            text = UpToCaret + c + PostCaret;
            MoveCaretRight(true, c == '\n');
        }

        private void RemoveCharacter()
        {
            Console.WriteLine(text.Substring(0, Math.Max(UpToCaret.Length - 1, 0)));
            Console.WriteLine(PostCaret);
            string placeholderText = text.Substring(0, Math.Max(UpToCaret.Length - 1, 0)) + PostCaret;
            MoveCaretLeft();
            text = placeholderText;
        }

        private void MoveCaretLeft()
        {
            if (CaretIndex.Character > 0)
            {
                CaretIndex = (caretIndex.character - 1, caretIndex.line);
                return;
            }

            if (CaretIndex.Line > 0)
            {
                CaretIndex = (GetLine(caretIndex.line - 1).Length, caretIndex.line - 1);
            }
        }

        private void MoveCaretRight(bool adding, bool newline)
        {
            if (newline)
            {
                CaretIndex = (0, caretIndex.line + 1);
                return;
            }

            if (adding)
            {
                CaretIndex = (caretIndex.character + 1, caretIndex.line);
                return;
            }

            if (CaretIndex.Character == GetLine(CaretIndex.Line).Length)
            {
                if (CaretIndex.Line + 1 < text.Split('\n').Count())
                {
                    CaretIndex = (0, caretIndex.line + 1);
                }

                return;
            }

            CaretIndex = (CaretIndex.Character + 1, CaretIndex.Line);
        }

        private string GetLine(int index)
        {
            
            return text.Split('\n')[index];
        }

        public bool Multiline
        {
            get { return multiline; }
            set { multiline = value; }
        }

        public int MaxCharacters
        {
            get { return maxCharacters; }
            set { maxCharacters = value; }
        }

        private string PreCaret
        {
            get
            {
                if (caretIndex.line == 0) return "";

                string[] lines = text.Split('\n');
                string output = "";
                for (int i = 0; i < caretIndex.line; i++)
                {
                    if (i > 0) output += '\n';

                    output += lines[i];
                }
                return output;
            }
        }

        private string PostCaret
        {
            get { return text.Substring(UpToCaret.Length); }
        }

        private string CaretLine
        {
            get
            {
                return text.Split('\n')[caretIndex.line].Substring(0, caretIndex.character);
            }
        }

        private string UpToCaret
        {
            get { return caretIndex.line == 0 ? CaretLine : PreCaret + "\n" + CaretLine; }
        }
        
        public (int Character, int Line) CaretIndex
        {
            get { return caretIndex; }
            private set { caretIndex = value; onCaretChanged?.Invoke(caretIndex); SetCaretPosition(); }
        }

        public void OnCaretChanged(Action<(int, int)> e)
        {
            onCaretChanged += e;
        }

        private void SetCaretPosition()
        {
            float height = GraphicsHelper.GetDefaultFont().MeasureString(UpToCaret == "" ? "|" : UpToCaret).Y - GraphicsHelper.GetDefaultFont().MeasureString(CaretLine == "" ? "|" : CaretLine).Y;
            float width = GraphicsHelper.GetDefaultFont().MeasureString(CaretLine).X;

            caret.GetTransform().SetPositionComponentValue(width, LVector2.HORIZONTAL);
            caret.GetTransform().SetPositionComponentValue(height, LVector2.VERTICAL);
        }
    }
}