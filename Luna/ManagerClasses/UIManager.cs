using Luna.UI.LayoutSystem;
using Luna.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Luna.UI.LayoutSystem.LUIVA;
using Luna.DataClasses;
using static Luna.UI.UIFactory;
using Luna.UI;
using System.IO;
using System.Collections.Generic;
using Luna.DataClasses.IDClasses;

namespace Luna.ManagerClasses
{
    internal class UIManager
    {
        private Texture2D pixelTexture;
        private SystemManager systemManager;

        private UIComponent rootComponent;
        private UIComponent mainWindowContainer;
        private UIComponent windowControlsPanel, windowControlsParentTop;
        private (Action alertFocus, Action alertUnfocus, int priority) focusedComponent;
        private Action quitAction;
        private LUIVA luiva;
        private GameWindow window;
        private UIFactory uiFactory = new UIFactory();
        private bool windowBorderless;
        bool themeToggle = true;

        private UIComponent productsContainer;
        private Dictionary<ProductID, UIComponent> products = new Dictionary<ProductID, UIComponent>();

        enum MainWindowState { Dashboard, Orders, Products };
        MainWindowState mainWindowState = MainWindowState.Dashboard;
        private TopBarBlock topBarBlock;
        UIComponent currentWindow;

        /// <summary>
        /// Generates and populates the permanent UI for Luna and sets up interactions for UI elements
        /// </summary>
        public UIManager(GameWindow window, Action quitAction, GraphicsDevice graphicsDevice, SystemManager systemManager)
        {
            this.systemManager = systemManager;
            this.window = window;
            this.quitAction = quitAction;
            pixelTexture = GraphicsHelper.GeneratePixelTexture();

            // The UIComponent which all others will be child objects of
            rootComponent = SetupRootComponent();

            // The UIComponent which will contain the minimise, maximise and quit buttons if the window is
            // set to borderless
            windowControlsPanel = SetupWindowControlsPanel();

            if (window.IsBorderless) AddWindowControls();
            windowBorderless = window.IsBorderless;

            // The header bar containing the Luna logo, Dashboard, Orders, Products and About buttons
            topBarBlock = uiFactory.CreateTopBar();
            UIComponent topBar = topBarBlock.Root;
            topBarBlock.Orders.Root.OnClick(() => { SetMainWindowState(MainWindowState.Orders); } );
            topBarBlock.Dashboard.Root.OnClick(() => { SetMainWindowState(MainWindowState.Dashboard); });
            topBarBlock.Products.Root.OnClick(() => SetMainWindowState(MainWindowState.Products));
            topBarBlock.About.Root.OnClick(() => { YesBlock about = uiFactory.CreateAboutPage(); about.Root.ForceTransparent(); AddOverlay(about.Root); about.Yes.OnClick(about.Root.Destroy); });

            //The UIComponent which will be the parent to the Dashboard, Orders, or Products window
            mainWindowContainer = new BlankUI(UITheme.ColorType.Placeholder);
            mainWindowContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
            });

            // Create and setup the Dashboard window
            UIComponent dashboard = SetupDashboard(uiFactory.CreateDashboard());

            // Acknowledge Dashboard as the current window, and populate the main window with it
            currentWindow = dashboard;
            HighlightTopBar(MainWindowState.Dashboard);
            mainWindowContainer.AddChild(dashboard);

            rootComponent.AddChild(topBar, mainWindowContainer);

            rootComponent.AddChild(windowControlsPanel);

            RecalculatePriority();
            // Set the callback to work out which UIComponents to give priority to based on if they're hovered and their layout order
            rootComponent.SetCheckFocusCallback(CheckFocus);

            // Setup LUIVA - the layout engine behind all the UIComponents
            luiva = new LUIVA();
            luiva.SetRootLayout(rootComponent);
            luiva.SetDisplayDimensions(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }

        public void Update(int displayWidth, int displayHeight)
        {
            luiva.SetDisplayDimensions(displayWidth, displayHeight);
            PreUpdate();
            rootComponent.BaseUpdate();
            PostUpdate();
        }

        public void Draw(SpriteBatch spritebatch)
        {
            rootComponent.BaseDraw(spritebatch);
        }

        public void SetPixelTexture(Texture2D pixelTexture)
        {
            UIComponent.SetPixelTexture(pixelTexture);
        }

        private void RecalculatePriority()
        {
            rootComponent.SetPriority(0);
        }

        private void CheckFocus(Action alertFocus, Action alertUnfocus, int priority)
        {
            // Higher priority means we need to consider a new UIComponent
            if (priority > focusedComponent.priority)
            {
                // Alert the old component that it's not focused anymore
                if (focusedComponent.alertUnfocus != null) focusedComponent.alertUnfocus();

                // Update focusedComponent so that it can be informed that it's focused, or alerted when it's unfocused
                focusedComponent = (alertFocus, alertUnfocus, priority);
            }
        }

        private void PreUpdate()
        {
            KeyboardHandler.SetKeyboard();
            MouseHandler.SetMouse();

            if (window.IsBorderless != windowBorderless)
            {
                if (window.IsBorderless) AddWindowControls();
                else RemoveWindowControls();
                windowBorderless = window.IsBorderless;
            }

            rootComponent.CascadeTheme(themeToggle ? UIFactory.PlumTheme : UIFactory.PlumTheme2);

            RecalculatePriority();
            luiva.CalculateLayout();
            // Unfocus the current thing so we can re-test for the correct component
            focusedComponent.alertUnfocus?.Invoke();
            focusedComponent = (null, null, -1);
            rootComponent.PreUpdate();
            // Tell newly focused object that it's focused
            focusedComponent.alertFocus?.Invoke();
        }

        private void PostUpdate()
        {
            rootComponent.PostUpdate();
            KeyboardHandler.SetOldKeyboard();
            MouseHandler.SetOldMouse();
        }

        /// <summary>
        /// Saves, and returns the system path of, a texture imported via a TextureImporter structure
        /// </summary>
        /// <param name="texture">The texture to load</param>
        private string ImportTextureAction(Texture2D texture)
        {
            Console.WriteLine("Importing Texture!");
            //overlayComponent.Visible = false;

            string filePath = $"C:\\Users\\bill\\Documents\\C#\\{LunaDateTime.Now.ShortDisplayAlt}.png";
            Console.WriteLine(filePath);
            FileStream s = new FileStream(filePath, FileMode.Create);
            texture.SaveAsPng(s, texture.Width, texture.Height);
            s.Close();
            return filePath;
        }

        private void AddWindowControls()
        {
            WindowControls windowControls = uiFactory.CreateWindowControls();
            windowControls.Minimise.OnClick(() => NativeWindowHelper.ShowWindow(window.Handle, NativeWindowHelper.SW_MINIMISE));
            windowControls.Maximise.OnClick(() => NativeWindowHelper.ShowWindow(window.Handle, NativeWindowHelper.IsZoomed(window.Handle) ? NativeWindowHelper.SW_RESTORE : NativeWindowHelper.SW_MAXIMISE));
            windowControls.Quit.OnClick(quitAction);

            windowControlsParentTop = windowControls.Root;
            windowControlsPanel.AddChild(windowControls.Root);
        }

        private void RemoveWindowControls()
        {
            windowControlsPanel.RemoveChild(windowControlsParentTop);
        }

		private void SetMainWindowState(MainWindowState mainWindowState)
		{
			if (this.mainWindowState == mainWindowState) return;

			this.mainWindowState = mainWindowState;

			switch (mainWindowState)
			{
				case MainWindowState.Dashboard:
				{
					InitiateFadeTransition(SetupDashboard(uiFactory.CreateDashboard()));
					HighlightTopBar(MainWindowState.Dashboard);
					break;
				}
				case MainWindowState.Orders:
				{
					InitiateFadeTransition(SetupOrders(uiFactory.CreateOrders()));
					HighlightTopBar(MainWindowState.Orders);
					break;
				}
				case MainWindowState.Products:
				{
					InitiateFadeTransition(SetupProducts(uiFactory.CreateProducts()));
					HighlightTopBar(MainWindowState.Products);
					ProductManager.SetUpdateProductCallback(UpdateProducts);
					break;
				}
			}
		}

        private BlankUI SetupRootComponent()
        {
            BlankUI rootComponent = new BlankUI(UITheme.ColorType.MainSoft);
            rootComponent.CascadeTheme(PlumTheme);
            rootComponent.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.VERTICAL
            });

            return rootComponent;
        }

        private BlankUI SetupWindowControlsPanel()
        {
            BlankUI windowControlsPanel = new BlankUI(UITheme.ColorType.Placeholder);
            windowControlsPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Inline = false,
            });
            windowControlsPanel.FocusIgnore = true;

            return windowControlsPanel;
        }

        private UIComponent SetupOrders(OrdersBlock ordersBlock)
        {
            return ordersBlock.Root;
        }

        private UIComponent SetupProducts(ProductsBlock productsBlock)
        {
            productsContainer = productsBlock.ProductsContainer;

            productsBlock.AddProducts.OnClick(() => {
                AddOverlay(SetupProductCreator(uiFactory.CreateProductCreator()));
            });
            Dictionary<ProductID, Product> products = ProductManager.GetProducts();
            UpdateProducts(products);
            return productsBlock.Root;
        }

        private void UpdateProducts(Dictionary<ProductID, Product> productData)
        {
            foreach (ProductID id in products.Keys)
            {
                products[id].Destroy();
            }

            products.Clear();

            foreach (ProductID id in productData.Keys)
            {
                ProductBlock productBlock = uiFactory.CreateProduct();
                productBlock.Name.SetText(productData[id].GetName());
                productBlock.Cost.SetText("£" + productData[id].GetCost().ToString());
                productBlock.RemoveButton.OnClick(() => { ProductManager.RemoveProduct(id, false); products[id].Destroy(); products.Remove(id); });
                products[id] = productBlock.Root;

                productsContainer.AddChild(productBlock.Root);
            }
        }

        private UIComponent SetupDashboard(DashboardBlock dashboardBlock)
        {
            foreach (Order o in systemManager.GetOrders())
            {
                if (dashboardBlock.MainPanel.GetChildCount() != 0)
                {
                    BlankUI separator = new BlankUI(UITheme.ColorType.Background);
                    separator.SetLayout(new Layout()
                    {
                        LayoutWidth = Sizing.Grow(1),
                        LayoutHeight = Sizing.Fixed(2)
                    });
                    separator.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Separator });
                    dashboardBlock.MainPanel.AddChild(separator);
                }
                OrderBlock orderBlock = uiFactory.CreateOrder(o);
                dashboardBlock.MainPanel.AddChild(orderBlock.Root);
            }

            return dashboardBlock.Root;
        }

        private UIComponent SetupProductCreator(ProductCreator productCreator)
        {
            // Create an image importer block when SelectIcon is clicked
            productCreator.SelectIcon.OnClick(() => {
                Texture2D tex = IOManager.LoadImageFromDialog();
                if (tex == null) return;

                // Creates an image importer UI block for processing tex, assigns the action to be
                // performed when accepted, and the action to be performed when rejected (destruction
                // of the block is handled within the function, so there is no need to assign any
                // further function here)
                YesNoBlock imageImporter = uiFactory.CreateImageImporter(new LTexture2D(tex), (Texture2D t) => { productCreator.Icon.Texture = new LTexture2D(IOManager.LoadImageFromFile(ImportTextureAction(t))); }, () => { });
                
                // Force imageImporter and all child components to become transparent, so they will animate
                // to the theme colours
                imageImporter.Root.ForceTransparent();
                AddOverlay(imageImporter.Root);
            });

            // Create a new product when OKButton is clicked
            productCreator.OKButton.OnClick(() => {
                Product product = new Product().SetProductID(ProductID.CreateSequential()).SetName(productCreator.Name.Text).SetCost(float.Parse(productCreator.Cost.Text));
                ProductManager.AddProduct(product);
                Console.WriteLine("Attempted to create a product with name: " + productCreator.Name.Text + ", cost " + productCreator.Cost.Text);
                });
            return productCreator.Root;
        }

        /// <summary>
        /// Adds the given UIComponent to the root in front of everything else
        /// </summary>
        /// <param name="overlayUI">the UIComponent to add as an overlay</param>
        private void AddOverlay(UIComponent overlayUI)
        {
            // Overlay a shadow to visually separate overlayUI from background elements
            BlankUI overlayShadow = new BlankUI(UITheme.ColorType.Shadow);
            overlayShadow.SetLayout(new Layout()
            {
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                // Separate from normal LUIVA layout
                Inline = false
            });

            overlayShadow.AddChild(overlayUI);

            // Force shadow to be transparent so it can animate to its normal colour
            overlayShadow.ForceTransparent();
            overlayUI.OnDestroy(() => {
                overlayShadow.Visible = false;
                // Animate shadow to transparent
                overlayShadow.ColourAnimator.SetColour(new Color(0, 0, 0, 0));
                // Destroy shadow when the transition is complete
                overlayShadow.ColourAnimator.OnTransitionAction(() => { rootComponent.RemoveChild(overlayShadow); }); });

            rootComponent.AddChild(overlayShadow);
        }

        /// <summary>
        /// Swaps out the current window with the specified window, by means of a fade transition
        /// </summary>
        /// <param name="newWindow">The new window to fade to</param>
        private void InitiateFadeTransition(UIComponent newWindow)
        {
            mainWindowContainer.AddChild(newWindow);
            // Destroy old window when the transition has completed
            newWindow.ColourAnimator.OnTransitionAction(() => { mainWindowContainer.RemoveChild(currentWindow); currentWindow = newWindow; });
        }

        /// <summary>
        /// Makes sure the right top-bar button is highlighted for the current MainWindowState
        /// </summary>
        /// <param name="mainWindowState">Which window is currently occupying the main window container</param>
        private void HighlightTopBar(MainWindowState mainWindowState)
        {
            switch (mainWindowState)
            {
                case MainWindowState.Dashboard:
                {
                    SetButtonColourType(topBarBlock.Dashboard, UITheme.ColorType.Highlit);
                    SetButtonColourType(topBarBlock.Orders, UITheme.ColorType.Main);
                    SetButtonColourType(topBarBlock.Products, UITheme.ColorType.Main);
                    break;
                }
                case MainWindowState.Orders:
                {
                    SetButtonColourType(topBarBlock.Dashboard, UITheme.ColorType.Main);
                    SetButtonColourType(topBarBlock.Orders, UITheme.ColorType.Highlit);
                    SetButtonColourType(topBarBlock.Products, UITheme.ColorType.Main);
                    break;
                }
                case MainWindowState.Products:
                {
                    SetButtonColourType(topBarBlock.Dashboard, UITheme.ColorType.Main);
                    SetButtonColourType(topBarBlock.Orders, UITheme.ColorType.Main);
                    SetButtonColourType(topBarBlock.Products, UITheme.ColorType.Highlit);
                    break;
                }
            }
        }

        /// <summary>
        /// Quickly sets the ColourType of the given TextButton to the given value
        /// </summary>
        /// <param name="button">The button to change</param>
        /// <param name="colorType">The colour type to change to</param>
        private void SetButtonColourType(TextButton button, UITheme.ColorType colorType)
        {
            button.Root.SetTheme(new UITheme() { ColourType = colorType });
            button.Label.SetTheme(new UITheme() { ColourType = colorType });
        }

        /// <summary>
        /// Gives every UIComponent the means to change the GraphicsDevice's ScissorRectangle, for clipped UIComponents
        /// </summary>
        public void SetUpdateScissorRectangleAction(GraphicsDevice graphicsDevice)
        {
            UIComponent.SetUpdateScissorRectangleAction((Rectangle r) => { graphicsDevice.ScissorRectangle = r; });
        }
    }
}
