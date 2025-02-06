using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
        private int maxCharacters = 30;
        private (int character, int line) caretIndex;

        public TextInput() : base(UITheme.ColorType.Background)
        {
            overrideTheme.ColourType = UITheme.ColorType.Background;
            layout.Padding = new Tetra(2);

            internalPanel = new BlankUI(UITheme.ColorType.Background);
            internalPanel.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1), LayoutHeight = Sizing.Grow(1), HorizontalAlignment = Alignment.Middle, VerticalAlignment = Alignment.Middle });
            internalPanel.SetTheme(new UITheme() { CornerRadius = (8, 8, 8, 8), Rounded = true });
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
                        Console.WriteLine(caretIndex);
                        break;
                    }
                    // RIGHT KEY
                    case (char)1:
                    {
                        MoveCaretRight(false, false);
                        Console.WriteLine(caretIndex);
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

            text += c;
            MoveCaretRight(true, c == '\n');

            Console.WriteLine(caretIndex);
        }

        private void RemoveCharacter()
        {
            text = text.Substring(0, Math.Max(text.Length - 1, 0));
            MoveCaretLeft();

            Console.WriteLine(caretIndex);
        }

        private void MoveCaretLeft()
        {
            if (caretIndex.character > 0)
            {
                caretIndex.character--;
                return;
            }

            if (caretIndex.line > 0)
            {
                caretIndex.line--;
                caretIndex.character = GetLine(caretIndex.line).Length;
            }
        }

        private void MoveCaretRight(bool adding, bool newline)
        {
            if (newline)
            {
                caretIndex.character = 0;
                caretIndex.line++;
                return;
            }

            if (adding)
            {
                caretIndex.character++;
                return;
            }

            if (caretIndex.character == GetLine(caretIndex.line).Length)
            {
                if (caretIndex.line + 1 < text.Split('\n').Count())
                {
                    caretIndex = (caretIndex.line + 1, 0);
                }

                return;
            }

            caretIndex = (caretIndex.character + 1, caretIndex.line);
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

        private string CaretLine
        {
            get
            {
                return text.Split('\n')[caretIndex.line].Substring(0, caretIndex.character);
            }
        }
    }
}