using Luna.DataClasses;
using Luna.HelperClasses;
using Luna.ManagerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Luna.UI.LayoutSystem;
using static Luna.UI.LayoutSystem.LUIVA;
using System;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Permissions;

namespace Luna.UI
{
    internal class UIFactory
    {
        public struct TextButton
        {
            private Button root;
            private Label label;

            public Button Root
            {
                get { return root; }
                set { root = value; }
            }

            public Label Label
            {
                get { return label; }
                set { label = value; }
            }
        }

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

        public struct YesBlock
        {
            private UIComponent root;
            private Button yes;

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
        }

        public struct TopBarBlock
        {
            UIComponent root;
            TextButton dashboard, orders, products, about;

            public UIComponent Root
            {
                get { return root; }
                set { root = value; }
            }

            public TextButton Dashboard
            {
                get { return dashboard; }
                set { dashboard = value; }
            }

            public TextButton Orders
            {
                get { return orders; }
                set { orders = value; }
            }

            public TextButton Products
            {
                get { return products; }
                set { products = value; }
            }

            public TextButton About
            {
                get { return about; }
                set { about = value; }
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

        public struct OrdersBlock
        {
            private UIComponent root;

            public UIComponent Root
            {
                get { return root; }
                set { root = value; }
            }
        }

        public struct ProductsBlock
        {
            private UIComponent root;
            private Button addProducts;

            public UIComponent Root
            {
                get { return root; }
                set { root = value; }
            }

            public Button AddProducts
            {
                get { return addProducts; }
                set { addProducts = value; }
            }
        }

        public struct ProductCreator
        {
            private UIComponent root;
            private Button selectIcon, ok, close;
            private TextInput name, cost;
            private UITexture icon;

            public UIComponent Root
            {
                get { return root; }
                set { root = value; }
            }

            public Button SelectIcon
            {
                get { return selectIcon; }
                set { selectIcon = value; }
            }

            public Button OKButton
            {
                get { return ok; }
                set { ok = value; }
            }

            public Button CloseButton
            {
                get { return close; }
                set { close = value; }
            }

            public TextInput Name
            {
                get { return name; }
                set { name = value; }
            }

            public TextInput Cost
            {
                get { return cost; }
                set { cost = value; }
            }

            public UITexture Icon
            {
                get { return icon; }
                set { icon = value; }
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
                LayoutWidth = Sizing.Fixed(200),
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

            BlankUI logoContainer = new BlankUI(UITheme.ColorType.Main);
            logoContainer.SetLayout(TopBarButtonLayout);
            logoContainer.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed(150), HorizontalAlignment = Alignment.Begin });

            Label logo = new Label("LUNΛ", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            BlankUI separator = new BlankUI(UITheme.ColorType.Separator);
            separator.SetLayout(SeparatorVerticalLayout);

            Button dashboardButton = new Button(UITheme.ColorType.Main);
            dashboardButton.SetLayout(TopBarButtonLayout);

            Label dashboardLabel = new Label("Dashboard", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            Button ordersButton = new Button(UITheme.ColorType.Main);
            ordersButton.SetLayout(TopBarButtonLayout);

            Label ordersLabel = new Label("Orders", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            Button productsButton = new Button(UITheme.ColorType.Main);
            productsButton.SetLayout(TopBarButtonLayout);

            Label productsLabel = new Label("Products", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            Button aboutButton = new Button(UITheme.ColorType.Main);
            aboutButton.SetLayout(TopBarButtonLayout);

            Label aboutLabel = new Label("About", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);

            BlankUI topBarSeparator = new BlankUI(UITheme.ColorType.Background);
            topBarSeparator.SetLayout(SeparatorHorizontalLayout);

            BlankUI topBarSpacer = new BlankUI(UITheme.ColorType.Placeholder);
            topBarSpacer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1)
            });
            
            logoContainer.AddChild(logo);

            dashboardButton.AddChild(dashboardLabel);

            ordersButton.AddChild(ordersLabel);

            productsButton.AddChild(productsLabel);

            aboutButton.AddChild(aboutLabel);

            root.AddChild(logoContainer, separator, dashboardButton, ordersButton, productsButton, topBarSpacer, aboutButton);

            return new TopBarBlock()
            {
                Root = root,
                Dashboard = new TextButton() { Root = dashboardButton, Label = dashboardLabel },
                Orders = new TextButton() { Root = ordersButton, Label = ordersLabel },
                Products = new TextButton() { Root = productsButton, Label = productsLabel },
                About = new TextButton() { Root = aboutButton, Label = aboutLabel }
            };
        }

        public DashboardBlock CreateDashboard()
        {
            BlankUI mainPanel = new BlankUI(UITheme.ColorType.MainSoft);
            mainPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.HORIZONTAL,
                Inline = false
            });

            ScrollView leftPanel = new ScrollView(UITheme.ColorType.Background);
            leftPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10),
                Spacing = 10
            });
            leftPanel.Scrollable = true;

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

            BlankUI contentSeparator = new(UITheme.ColorType.Separator);
            contentSeparator.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(2),
                LayoutHeight = Sizing.Grow(1)
            });

            Label l1 = new Label("Testing", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            Label l2 = new Label("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);//new Label("Enjoy the money, I hope it makes you happy. Dear lord, what a sad little life, Jane. You ruined my night completely so you could have the money and I hope now you can spend it on lessons in grace and decorum. Because you have all the grace of a reversing dump truck without any tyres on.", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
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

            Toggle t = new Toggle(UITheme.ColorType.Background);
            t.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1), LayoutHeight = Sizing.Wrap() });
            t.SetTheme(new UITheme() { Rounded = true });

            Slider s = new Slider(LVector2.HORIZONTAL, 0, 5, 1, UITheme.ColorType.Placeholder);
            s.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1) });
            s.SetTheme(new UITheme() { Rounded = true });
            s.OnValueChanged((float value) => t.Label.SetText($"{value} spiders"));

            Slider s2 = new Slider(LVector2.HORIZONTAL, 0, 1, 0, UITheme.ColorType.Placeholder);
            s2.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1) });
            s2.SetTheme(new UITheme() { Rounded = true });

            Label label = new Label("1 Slider", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);

            TextInput textInput = new TextInput();
            textInput.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1), LayoutHeight = Sizing.Wrap() });
            textInput.InputType = TextInput.InputFormat.CentiDecimal;
            textInput.Prefix = "£";
            textInput.Placeholder = "Monetary Value";

            contentContainer.AddChild(s2);
            contentContainer.AddChild(label);
            contentContainer.AddChild(s);
            contentContainer.AddChild(t);

            contentContainer.AddChild(textInput);

            leftPanel.AddChild(contentContainer);

            mainPanel.AddChild(leftPanel);
            mainPanel.AddChild(contentSeparator);
            mainPanel.AddChild(rightPanel);

            return new DashboardBlock() { Root = mainPanel, LeftPanel = leftPanel, MainPanel = rightPanel };
        }

        public OrdersBlock CreateOrders()
        {
            BlankUI blank = new BlankUI(UITheme.ColorType.MainSoft);
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

            return new OrdersBlock() { Root = blank };
        }

        public ProductsBlock CreateProducts()
        {
            BlankUI blank = new BlankUI(UITheme.ColorType.MainSoft);
            blank.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Inline = false,
                LayoutAxis = LVector2.HORIZONTAL
            });

            BlankUI leftPanel = new BlankUI(UITheme.ColorType.Background);
            leftPanel.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1), LayoutHeight = Sizing.Grow(1), Padding = new Tetra(15) });

            BlankUI separator = new BlankUI(UITheme.ColorType.Separator);
            separator.SetLayout(SeparatorVerticalLayout);

            BlankUI rightPanel = new BlankUI(UITheme.ColorType.MainSoft);
            rightPanel.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(3), LayoutHeight = Sizing.Grow(1) });

            blank.AddChild(leftPanel, separator, rightPanel);

            Button addProduct = new Button(UITheme.ColorType.Main);
            addProduct.SetLayout(new Layout() { LayoutWidth = Sizing.Grow(1), LayoutHeight = Sizing.Fixed(40), VerticalAlignment = Alignment.Middle, HorizontalAlignment = Alignment.Middle });

            Label addProductLabel = new Label("Add product", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);
            addProduct.AddChild(addProductLabel);

            leftPanel.AddChild(addProduct);

            return new ProductsBlock() { Root = blank, AddProducts = addProduct };
        }

        public ProductCreator CreateProductCreator()
        {
            BlankUI root = new BlankUI(UITheme.ColorType.Background);
            root.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(700),
                LayoutHeight = Sizing.Fixed(500),
                Padding = new Tetra(20),
                LayoutAxis = LVector2.VERTICAL,
                Spacing = 20,
                ClipChildren = false
            });
            root.SetTheme(new UITheme()
            {
                Rounded = true,
                CornerRadius = (20, 20, 20, 20)
            });

            BlankUI iconContainer = new BlankUI(UITheme.ColorType.Placeholder);
            iconContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(100),
                LayoutAxis = LVector2.HORIZONTAL
            });

            UITexture icon = new UITexture();
            icon.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                ImageFitMode = Layout.FitMode.MaxFit
            });
            LTexture2D iconTexture = new LTexture2D(GraphicsHelper.GeneratePixelTexture());
            iconTexture.LockAspectRatio = true;
            icon.Texture = iconTexture;
            iconContainer.AddChild(icon);
            Button selectIcon = new Button(UITheme.ColorType.Main);
            selectIcon.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Wrap(),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(0, 20, 0, 20),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            Label selectIconText = new Label("Select Icon", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);
            selectIcon.AddChild(selectIconText);
            iconContainer.AddChild(selectIcon);

            BlankUI nameContainer = new BlankUI(UITheme.ColorType.Placeholder);
            nameContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                Spacing = 20,
                VerticalAlignment = Alignment.Middle
            });
            Label nameIndicator = new Label("Name", GraphicsHelper.GetBoldFont(), UITheme.ColorType.Background);
            TextInput nameInput = new TextInput();
            nameInput.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
            });
            nameContainer.AddChild(nameIndicator, nameInput);

            BlankUI costContainer = new BlankUI(UITheme.ColorType.Placeholder);
            costContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                Spacing = 20,
                VerticalAlignment = Alignment.Middle,
                ClipChildren = false
            });
            Label costIndicator = new Label("Cost", GraphicsHelper.GetBoldFont(), UITheme.ColorType.Background);
            TextInput costInput = new TextInput();
            costInput.InputType = TextInput.InputFormat.CentiDecimal;
            costInput.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
            });
            costContainer.AddChild(costIndicator, costInput);

            BlankUI confirmationContainer = new BlankUI(UITheme.ColorType.Placeholder);
            confirmationContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.HORIZONTAL,
                Spacing = 20
            });
            Button ok = new Button(Button.VisualResponse.ColourChange, UITheme.ColorType.Main);
            ok.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(50),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            Label okLabel = new Label("OK", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);
            ok.AddChild(okLabel);
            Button cancel = new Button(Button.VisualResponse.ColourChange, UITheme.ColorType.Emergency);
            cancel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(50),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            Label cancelLabel = new Label("Cancel", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Emergency);
            cancel.AddChild(cancelLabel);
            confirmationContainer.AddChild(ok, cancel);

            root.AddChild(iconContainer, nameContainer, costContainer, confirmationContainer);

            return new ProductCreator { Root = root, SelectIcon = selectIcon, OKButton = ok, CloseButton = cancel, Name = nameInput, Cost = costInput, Icon = icon };
        }

        public YesBlock CreateAboutPage()
        {
            BlankUI root = new BlankUI(UITheme.ColorType.Background);
            root.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(700),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL
            });

            ScrollView scrollView = new ScrollView(UITheme.ColorType.Placeholder);
            scrollView.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(400),
                Padding = new Tetra(20)
            });
            scrollView.SetTheme(new UITheme()
            {
                Rounded = true,
                CornerRadius = (20, 20, 20, 20)
            });

            BlankUI contentContainer = new BlankUI(UITheme.ColorType.Placeholder);
            contentContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL
            });

            BlankUI logoContainer = new BlankUI(UITheme.ColorType.Placeholder);
            logoContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                Padding = new Tetra(20)
            });

            UITexture logo = new UITexture(new LTexture2D(GraphicsHelper.LuivaLogo));
            logo.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(50),
                ImageFitMode = Layout.FitMode.MinFit
            });
            //LTexture2D tex = new LTexture2D(GraphicsHelper.LuivaLogo);
            //tex.LockAspectRatio = true;
            //logo.Texture = tex;
            logoContainer.AddChild(logo);

            BlankUI textContainer = new BlankUI(UITheme.ColorType.Background);
            textContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL,
                Padding = new Tetra(20)
            });
            Label header = new Label("About", GraphicsHelper.GetBoldFont(), UITheme.ColorType.Background);
            Label content = new Label("Luna is a product/order management system deigned from the ground up by Bill Shepherd. " +
                "It runs on LUIVA, a custom UI layout system created in C# with the Monogame framework. ", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            content.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1)
            });
            textContainer.AddChild(header, content);

            BlankUI okButtonContainer = new BlankUI(UITheme.ColorType.Placeholder);
            okButtonContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                Padding = new Tetra(20),
                HorizontalAlignment = Alignment.Middle
            });

            Button okButton = new Button(UITheme.ColorType.Main);
            okButton.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Wrap(),
                LayoutHeight = Sizing.Wrap(),
                Padding = new Tetra(10)
            });

            Label okLabel = new Label("OK", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Main);
            okButton.AddChild(okLabel);

            okButtonContainer.AddChild(okButton);

            contentContainer.AddChild(logoContainer, textContainer);
            scrollView.AddChild(contentContainer);
            root.AddChild(scrollView, okButtonContainer);

            return new YesBlock() { Root = root, Yes = okButton };
        }

        private Layout TopBarButtonLayout
        {
            get
            {
                return new Layout()
                {
                    LayoutWidth = Sizing.Wrap(),
                    LayoutHeight = Sizing.Grow(1),
                    Padding = new Tetra(15),
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

        private Layout SeparatorVerticalLayout
        {
            get
            {
                return new Layout()
                {
                    LayoutWidth = Sizing.Fixed(2),
                    LayoutHeight = Sizing.Grow(1)
                };
            }
        }

        public static UITheme PlumTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new ColourPalatte().SetMainColour(new Color(96, 30, 112)).SetHoveredColour(new Color(79, 25, 93)).SetSelectedColour(new Color(62, 19, 73)).SetTextColour(new Color(255, 255, 255)),
                    MainColourSoft = new ColourPalatte().SetMainColour(new Color(240, 230, 242)),
                    HighlitColour = new ColourPalatte().SetMainColour(new Color(131, 41, 153)).SetHoveredColour(new Color(131, 41, 153)).SetSelectedColour(new Color(131, 41, 153)).SetTextColour(new Color(255, 255, 255)),
                    BackgroundColour = new ColourPalatte().SetMainColour(new Color(255, 255, 255)).SetHoveredColour(new Color(233, 218, 236)).SetSelectedColour(new Color(223, 201, 227)).SetTextColour(new Color(30, 30, 30)),
                    EmergencyColour = new ColourPalatte().SetMainColour(new Color(255, 0, 0)).SetHoveredColour(new Color(235, 0, 0)).SetSelectedColour(new Color(215, 0, 0)).SetTextColour(new Color(255, 255, 255)),
                    SeparatorColour = new ColourPalatte().SetMainColour(new Color(30, 30, 30) * 0.2f),
                    ShadowColour = new ColourPalatte().SetMainColour(new Color(0, 0, 0) * 0.5f),
                    ScrollbarColour = new ColourPalatte().SetMainColour(new Color(233, 218, 236)).SetHoveredColour(new Color(223, 201, 227)).SetSelectedColour(new Color(223, 201, 227)),
                    PlaceholderTextColour = new ColourPalatte().SetTextColour(new Color(96, 30, 112) * 0.5f),
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
                    MainColour = new ColourPalatte().SetMainColour(new Color(0, 92, 255)).SetHoveredColour(new Color(0, 84, 230)).SetSelectedColour(new Color(0, 75, 204)).SetTextColour(new Color(255, 255, 255)),
                    MainColourSoft = new ColourPalatte().SetMainColour(new Color(217, 226, 242)),
                    HighlitColour = new ColourPalatte().SetMainColour(new Color(51, 126, 255)).SetHoveredColour(new Color(51, 126, 255)).SetSelectedColour(new Color(51, 126, 255)).SetTextColour(new Color(255, 255, 255)),
                    BackgroundColour = new ColourPalatte().SetMainColour(new Color(255, 255, 255)).SetHoveredColour(new Color(198, 212, 236)).SetSelectedColour(new Color(179, 197, 230)).SetTextColour(new Color(30, 30, 30)),
                    EmergencyColour = new ColourPalatte().SetMainColour(new Color(255, 0, 0)).SetHoveredColour(new Color(235, 0, 0)).SetSelectedColour(new Color(215, 0, 0)).SetTextColour(new Color(255, 255, 255)),
                    SeparatorColour = new ColourPalatte().SetMainColour(new Color(30, 30, 30) * 0.2f),
                    ShadowColour = new ColourPalatte().SetMainColour(new Color(0, 0, 0) * 0.5f),
                    ScrollbarColour = new ColourPalatte().SetMainColour(new Color(198, 212, 236)).SetHoveredColour(new Color(179, 197, 230)).SetSelectedColour(new Color(179, 197, 230)),
                    PlaceholderTextColour = new ColourPalatte().SetTextColour(new Color(0, 92, 255) * 0.5f),
                    CornerRadius = (10, 10, 10, 10)
               };
            }
        }
    }
}