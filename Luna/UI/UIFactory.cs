using Luna.DataClasses;
using Luna.HelperClasses;
using Luna.ManagerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Luna.UI.LayoutSystem;
using static Luna.UI.LayoutSystem.LUIVA;
using System;

namespace Luna.UI
{
    internal class UIFactory
    {
        public struct OrderBlock
        {
            Button root;

            public Button Root
            {
                get { return root; }
                set { root = value; }
            }
        }

        public struct YesNoBlock
        {
            UIComponent root;
            Button yes;
            Button no;

            public UIComponent Root
            {
                get { return root; }
                set { root = value; }
            }

            public Button Yes
            {
                get { return yes; }
                set { yes = value; }
            }

            public Button No
            {
                get { return no; }
                set { no = value; }
            }
        }

        public OrderBlock CreateOrder(Order order)
        {
            //
            //  OverviewBox
            //  ├─Name
            //  ├─Status
            //  InfoBox
            //  ├─Product 1
            //    ├─Product Name
            //  ├─Product 2
            //

            Button root = new Button(PlumTheme, UITheme.ColorType.Background);
            root.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                Padding = new Tetra(10),
                Spacing = 10,
                LayoutAxis = LVector2.HORIZONTAL
            });

            Button titleBox = new Button(PlumTheme, UITheme.ColorType.Background);
            titleBox.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(150),
                LayoutHeight = Sizing.Wrap(),
                HorizontalAlignment = Alignment.Middle,
                Padding = new Tetra(10),
                LayoutAxis = LVector2.VERTICAL
            });
            titleBox.RenderDefaultRect = false;
            titleBox.FocusIgnore = true;

            Label title = new Label($"{CustomerManager.GetCustomerByID(order.GetCustomerID()).FullName}", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Background);
            Label productInfo = new Label(order.GetOrderStatus().ToString(), GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Background);

            Button infoBox = new Button(PlumTheme, UITheme.ColorType.Background);
            infoBox.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL,
                Padding = new Tetra(10)
            });
            infoBox.RenderDefaultRect = false;
            infoBox.FocusIgnore = true;

            Label productTitle = new Label(ProductManager.GetProductByID(order.GetProductIDs()[0]).GetName(), GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Background);

            infoBox.AddChild(productTitle);

            titleBox.AddChild(title);
            titleBox.AddChild(productInfo);

            root.AddChild(titleBox);
            root.AddChild(infoBox);

            root.ForceSynchChildren();
            return new OrderBlock() { Root = root };
        }

        public YesNoBlock CreateImageImporter(LTexture2D texture, Action<Texture2D> importAction, Action cancelAction)
        {
            Button panel = new Button(PlumTheme, UITheme.ColorType.Background);
            panel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Wrap(),
                LayoutHeight = Sizing.Wrap(),
                Padding = new Tetra(20),
                Spacing = 20,
                LayoutAxis = LVector2.VERTICAL,
                HorizontalAlignment = Alignment.Middle
            });
            panel.FocusIgnore = true;

            UIDraggableTexture preview = new UIDraggableTexture(texture);
            preview.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(400),
                LayoutHeight = Sizing.Fixed(400),
                ImageFitMode = Layout.FitMode.MaxFit
            });
            preview.FocusIgnore = false;
            preview.RenderDefaultRect = false;

            Button buttonContainer = new Button(PlumTheme, UITheme.ColorType.Background);
            buttonContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(250),
                LayoutHeight = Sizing.Fixed(40),
                Spacing = 20,
                LayoutAxis = LVector2.HORIZONTAL
            });
            buttonContainer.FocusIgnore = true;

            Button yesButton = new Button(PlumTheme, UITheme.ColorType.Main);
            yesButton.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            yesButton.OnClick(() => { importAction(preview.GetVisibleSubtexture()); panel.Destroy(); panel = null; });
            yesButton.GetTheme().ColourType = UITheme.ColorType.Main;

            Label yesLabel = new Label("Import", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Main);
            yesLabel.GetTheme().ColourType = UITheme.ColorType.Main;

            yesButton.AddChild(yesLabel);

            Button noButton = new Button(PlumTheme, UITheme.ColorType.Emergency);
            noButton.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            noButton.OnClick(() => { cancelAction(); panel.Destroy(); panel = null; });
            noButton.GetTheme().ColourType = UITheme.ColorType.Emergency;

            Label noLabel = new Label("Cancel", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Emergency);
            noLabel.GetTheme().ColourType = UITheme.ColorType.Emergency;

            noButton.AddChild(noLabel);

            buttonContainer.AddChild(yesButton, noButton);
            panel.AddChild(preview, buttonContainer);

            panel.ForceSynchChildren();

            return new YesNoBlock() { Root = panel, Yes = yesButton, No = noButton };
        }

        public static UITheme PlumTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Colour(96, 30, 112),
                    MainColourSoft = new Colour(240, 230, 242),
                    MainTextColour = new Colour(255, 255, 255),
                    BackgroundColour = new Colour(255, 255, 255),
                    BackgroundTextColour = new Colour(30, 30, 30),
                    EmergencyColour = new Colour(255, 0, 0),
                    EmergencyTextColour = new Colour(255, 255, 255),
                    SeparatorColour = new Colour(227, 218, 230),
                    ShadowColour = new Colour(0, 0, 0) * 0.5f,
                    HoverValue = 0.9f,
                    SelectValue = 0.8f,
                    CornerRadius = (7, 7, 7, 7)
                };
            }
        }
    }
}