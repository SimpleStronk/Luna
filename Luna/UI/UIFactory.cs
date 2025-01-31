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

        public struct TopBarBlock
        {
            UIComponent root;
            Button dashboard, orders;

            public UIComponent Root
            {
                get { return root; }
                set { root = value; }
            }

            public Button Dashboard
            {
                get { return dashboard; }
                set { dashboard = value; }
            }

            public Button Orders
            {
                get { return orders; }
                set { orders = value; }
            }
        }

        public struct DashboardBlock
        {
            private UIComponent root;
            private UIComponent leftPanel, mainPanel;

            public UIComponent Root
            {
                get { return root; }
                set { root = value; }
            }

            public UIComponent LeftPanel
            {
                get { return leftPanel; }
                set { leftPanel = value; }
            }

            public UIComponent MainPanel
            {
                get { return mainPanel; }
                set { mainPanel = value; }
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

            Button root = new Button(UITheme.ColorType.Background);
            root.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                Padding = new Tetra(10),
                Spacing = 10,
                LayoutAxis = LVector2.HORIZONTAL
            });

            Button titleBox = new Button(UITheme.ColorType.Background);
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

            Label title = new Label($"{CustomerManager.GetCustomerByID(order.GetCustomerID()).FullName}", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            Label productInfo = new Label(order.GetOrderStatus().ToString(), GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);

            Button infoBox = new Button(UITheme.ColorType.Background);
            infoBox.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL,
                Padding = new Tetra(10)
            });
            infoBox.RenderDefaultRect = false;
            infoBox.FocusIgnore = true;

            Label productTitle = new Label(ProductManager.GetProductByID(order.GetProductIDs()[0]).GetName(), GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);

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
            Button panel = new Button(UITheme.ColorType.Background);
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

            Button buttonContainer = new Button(UITheme.ColorType.Background);
            buttonContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(250),
                LayoutHeight = Sizing.Fixed(40),
                Spacing = 20,
                LayoutAxis = LVector2.HORIZONTAL
            });
            buttonContainer.FocusIgnore = true;
            buttonContainer.RenderDefaultRect = false;

            Button yesButton = new Button(UITheme.ColorType.Main);
            yesButton.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            yesButton.OnClick(() => { importAction(preview.GetVisibleSubtexture()); panel.Destroy(); panel = null; });
            yesButton.SetTheme(new UITheme(){ ColourType = UITheme.ColorType.Main });

            Label yesLabel = new Label("Import", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);
            yesLabel.SetTheme(new UITheme(){ ColourType = UITheme.ColorType.Main });

            yesButton.AddChild(yesLabel);

            Button noButton = new Button(UITheme.ColorType.Emergency);
            noButton.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            noButton.OnClick(() => { cancelAction(); panel.Destroy(); panel = null; });
            noButton.SetTheme(new UITheme(){ ColourType = UITheme.ColorType.Emergency });

            Label noLabel = new Label("Cancel", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Emergency);
            noLabel.SetTheme(new UITheme(){ ColourType = UITheme.ColorType.Emergency });

            noButton.AddChild(noLabel);

            buttonContainer.AddChild(yesButton, noButton);
            panel.AddChild(preview, buttonContainer);

            panel.ForceSynchChildren();

            return new YesNoBlock() { Root = panel, Yes = yesButton, No = noButton };
        }

        public TopBarBlock CreateTopBar()
        {
            BlankUI root = new BlankUI(UITheme.ColorType.Main);
            root.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(65),
                Padding = new Tetra(5),
                Spacing = 5
            });

            Button logoContainer = new Button(UITheme.ColorType.Main);
            logoContainer.SetLayout(TopBarButtonLayout);

            Label logo = new Label("LUNΛ", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            Button dashboardContainer = new Button(UITheme.ColorType.Main);
            dashboardContainer.SetLayout(TopBarButtonLayout);

            Label dashboardLabel = new Label("Dashboard", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            Button ordersContainer = new Button(UITheme.ColorType.Main);
            ordersContainer.SetLayout(TopBarButtonLayout);

            Label ordersLabel = new Label("Orders", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            BlankUI topBarSeparator = new BlankUI(UITheme.ColorType.Background);
            topBarSeparator.SetLayout(SeparatorHorizontalLayout);
            
            logoContainer.AddChild(logo);

            dashboardContainer.AddChild(dashboardLabel);

            ordersContainer.AddChild(ordersLabel);

            root.AddChild(logoContainer);
            root.AddChild(dashboardContainer);
            root.AddChild(ordersContainer);

            return new TopBarBlock() { Root = root, Dashboard = dashboardContainer, Orders = ordersContainer };
        }

        public DashboardBlock CreateDashboard()
        {
            BlankUI mainPanel = new BlankUI(UITheme.ColorType.Background);
            mainPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.HORIZONTAL,
                Inline = false
            });

            BlankUI leftPanel = new BlankUI(UITheme.ColorType.Background);
            leftPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10),
                Spacing = 10
            });

            BlankUI rightPanel = new BlankUI(UITheme.ColorType.MainSoft);
            rightPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(2),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(20),
                LayoutAxis = LVector2.VERTICAL
            });

            BlankUI contentContainer = new BlankUI(UITheme.ColorType.Background);
            contentContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL,
                Padding = new Tetra(10),
                Spacing = 10
            });

            Button b1 = new Button(UITheme.ColorType.Background);
            b1.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(70),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Button b2 = new Button(UITheme.ColorType.Background);
            b2.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                Padding = new Tetra(10)
            });

            Button b3 = new Button(UITheme.ColorType.Background);
            b3.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(70),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            BlankUI contentSeparator = new(UITheme.ColorType.Background);
            contentSeparator.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(2),
                LayoutHeight = Sizing.Grow(1)
            });

            Label l1 = new Label("Testing", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            Label l2 = new Label("This label is definitely too big. Let's have a moment of silence. In fact, notice how this button is bigger because the text is longer!", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            l2.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1)
            });
            Label l3 = new Label("Holy hell!", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);

            b1.AddChild(l1);
            b2.AddChild(l2);
            b3.AddChild(l3);

            contentContainer.AddChild(b1);
            contentContainer.AddChild(b2);
            contentContainer.AddChild(b3);

            leftPanel.AddChild(contentContainer);

            mainPanel.AddChild(leftPanel);
            mainPanel.AddChild(contentSeparator);
            mainPanel.AddChild(rightPanel);

            mainPanel.ForceSynchChildren();

            return new DashboardBlock() { Root = mainPanel, LeftPanel = leftPanel, MainPanel = rightPanel };
        }

        public BlankUI CreateBlankUI()
        {
            BlankUI blank = new BlankUI(UITheme.ColorType.Background);
            blank.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Inline = false
            });

            Button b = new Button(UITheme.ColorType.Main);
            b.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(300),
                LayoutHeight = Sizing.Fixed(300)
            });

            blank.AddChild(b);

            blank.ForceSynchChildren();

            return blank;
        }

        private Layout TopBarButtonLayout
        {
            get
            {
                return new Layout()
                {
                    LayoutWidth = Sizing.Wrap(),
                    LayoutHeight = Sizing.Grow(1),
                    Padding = new Tetra(10),
                    HorizontalAlignment = Alignment.Middle,
                    VerticalAlignment = Alignment.Middle
                };
            }
        }

        private Layout SeparatorHorizontalLayout
        {
            get
            {
                return new Layout()
                {
                    LayoutWidth = Sizing.Grow(1),
                    LayoutHeight = Sizing.Fixed(2)
                };
            }
        }

        public static UITheme PlumTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new ColourPalette().SetMainColour(new Color(96, 30, 112)).SetHoveredColour(new Color(79, 25, 93)).SetSelectedColour(new Color(62, 19, 73)).SetTextColour(new Color(255, 255, 255)),
                    MainColourSoft = new ColourPalette().SetMainColour(new Color(240, 230, 242)),
                    BackgroundColour = new ColourPalette().SetMainColour(new Color(255, 255, 255)).SetHoveredColour(new Color(233, 218, 236)).SetSelectedColour(new Color(223, 201, 227)).SetTextColour(new Color(30, 30, 30)),
                    EmergencyColour = new ColourPalette().SetMainColour(new Color(255, 0, 0)).SetHoveredColour(new Color(235, 0, 0)).SetSelectedColour(new Color(215, 0, 0)).SetTextColour(new Color(255, 255, 255)),
                    SeparatorColour = new ColourPalette().SetMainColour(new Color(60, 36, 66)),
                    ShadowColour = new ColourPalette().SetMainColour(new Color(0, 0, 0) * 0.5f),
                    CornerRadius = (10, 10, 10, 10)
                };
            }
        }

        public static UITheme PlumTheme2
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new ColourPalette().SetMainColour(new Color(0, 92, 255)).SetHoveredColour(new Color(0, 84, 230)).SetSelectedColour(new Color(0, 75, 204)).SetTextColour(new Color(255, 255, 255)),
                    MainColourSoft = new ColourPalette().SetMainColour(new Color(217, 226, 242)),
                    BackgroundColour = new ColourPalette().SetMainColour(new Color(255, 255, 255)).SetHoveredColour(new Color(198, 212, 236)).SetSelectedColour(new Color(179, 197, 230)).SetTextColour(new Color(30, 30, 30)),
                    EmergencyColour = new ColourPalette().SetMainColour(new Color(255, 0, 0)).SetHoveredColour(new Color(235, 0, 0)).SetSelectedColour(new Color(215, 0, 0)).SetTextColour(new Color(255, 255, 255)),
                    SeparatorColour = new ColourPalette().SetMainColour(new Color(36, 47, 66)),
                    ShadowColour = new ColourPalette().SetMainColour(new Color(0, 0, 0) * 0.5f),
                    CornerRadius = (10, 10, 10, 10)
               };
            }
        }
    }
}