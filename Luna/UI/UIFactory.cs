using Luna.DataClasses;
using Luna.HelperClasses;
using Luna.ManagerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Luna.UI.LayoutSystem;
using static Luna.UI.LayoutSystem.LUIVA;

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

            Button root = new Button();
            root.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                Padding = new Tetra(10),
                Spacing = 10,
                LayoutAxis = LVector2.HORIZONTAL
            });

            Button titleBox = new Button();
            titleBox.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(120),
                LayoutHeight = Sizing.Wrap(),
                HorizontalAlignment = Alignment.Middle,
                Padding = new Tetra(10),
                LayoutAxis = LVector2.VERTICAL
            });
            titleBox.RenderDefaultRect = false;
            titleBox.FocusIgnore = true;

            Label title = new Label($"{CustomerManager.GetCustomerByID(order.GetCustomerID()).FullName}", GraphicsHelper.GetDefaultFont());
            Label productInfo = new Label(order.GetOrderStatus().ToString(), GraphicsHelper.GetDefaultFont());

            Button infoBox = new Button();
            infoBox.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL,
                Padding = new Tetra(10)
            });
            infoBox.RenderDefaultRect = false;
            infoBox.FocusIgnore = true;

            Label productTitle = new Label(ProductManager.GetProductByID(order.GetProductIDs()[0]).GetName(), GraphicsHelper.GetDefaultFont());

            infoBox.AddChild(productTitle);

            titleBox.AddChild(title);
            titleBox.AddChild(productInfo);

            root.AddChild(titleBox);
            root.AddChild(infoBox);

            root.ForceSynchChildren();

            root.CascadeTheme(MainTheme);
            root.SetTheme(OrderTheme);
            return new OrderBlock() { Root = root };
        }

        public YesNoBlock CreateImageImporter(LTexture2D texture)
        {
            Button panel = new Button();
            panel.SetTheme(OrderTheme);
            panel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(400),
                LayoutHeight = Sizing.Fixed(400),
                Padding = new Tetra(20),
                LayoutAxis = LVector2.VERTICAL
            });

            UITexture preview = new UITexture(texture);
            preview.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                ImageFitMode = Layout.FitMode.MinFit,
                ImageAlignment = Alignment.Middle
            });
            preview.FocusIgnore = true;
            preview.RenderDefaultRect = false;

            Button buttonContainer = new Button();
            buttonContainer.SetTheme(OrderTheme);
            buttonContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(30),
                LayoutAxis = LVector2.HORIZONTAL
            });

            Button yesButton = new Button();
            yesButton.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Label yesLabel = new Label("Import", GraphicsHelper.GetDefaultFont());

            yesButton.AddChild(yesLabel);

            Button noButton = new Button();
            noButton.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Label noLabel = new Label("Cancel", GraphicsHelper.GetDefaultFont());

            noButton.AddChild(noLabel);

            buttonContainer.AddChild(yesButton, noButton);
            panel.AddChild(preview, buttonContainer);

            panel.ForceSynchChildren();

            panel.CascadeTheme(OrderTheme);
            yesButton.CascadeTheme(YesButtonTheme);
            noButton.CascadeTheme(NoButtonTheme);

            return new YesNoBlock() { Root = panel, Yes = yesButton, No = noButton };
        }

        private UITheme OrderTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(255, 255, 255, 255),
                    SecondaryColour = new Color(0, 0, 0, 255),
                    HoveredColour = new Color(240, 240, 255, 255),
                    CornerRadius = (7, 7, 7, 7)
                };
            }
        }

        private UITheme MainTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(255, 255, 255, 255),
                    SecondaryColour = new Color(0, 0, 0, 255),
                    HoveredColour = new Color(240, 240, 240, 255),
                    CornerRadius = (7, 7, 7, 7)
                };
            }
        }

        private UITheme YesButtonTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(0, 178, 255, 255),
                    SecondaryColour = new Color(255, 255, 255, 255),
                    HoveredColour = new Color(0, 170, 245, 255),
                    SelectedColour = new Color(0, 162, 235, 255),
                    CornerRadius = (7, 7, 7, 7)
                };
            }
        }

        private UITheme NoButtonTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(255, 0, 0, 255),
                    SecondaryColour = new Color(255, 255, 255, 255),
                    HoveredColour = new Color(245, 0, 0, 255),
                    SelectedColour = new Color(235, 0, 0, 255),
                    CornerRadius = (7, 7, 7, 7)
                };
            }
        }
    }
}