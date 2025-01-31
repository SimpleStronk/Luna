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
using System.Windows.Forms.VisualStyles;

namespace Luna.ManagerClasses
{
    internal class UIManager
    {
        private Texture2D pixelTexture;
        private SystemManager systemManager;

        private UIComponent rootComponent;
        private UIComponent overlayComponent;
        private UIComponent mainWindowContainer;
        private UIComponent windowControlsPanel, windowControlsParentTop;
        private (Action alertFocus, Action alertUnfocus, int priority) focusedComponent;
        private Action quitAction;
        private LUIVA luiva;
        private GameWindow window;
        private UIFactory uiFactory = new UIFactory();
        private bool windowBorderless;
        bool themeToggle = false;

        enum MainWindowState { Dashboard, Orders };
        MainWindowState mainWindowState = MainWindowState.Dashboard;
        private TopBarBlock topBarBlock;
        UIComponent currentWindow;

        public UIManager(GameWindow window, Action quitAction, GraphicsDevice graphicsDevice, SystemManager systemManager)
        {
            this.systemManager = systemManager;
            this.window = window;
            this.quitAction = quitAction;
            pixelTexture = GraphicsHelper.GeneratePixelTexture();

            rootComponent = new BlankUI(UITheme.ColorType.Background);
            rootComponent.CascadeTheme(PlumTheme);
            rootComponent.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.VERTICAL
            });

            overlayComponent = new BlankUI(UITheme.ColorType.Shadow);
            overlayComponent.SetLayout(new Layout()
            {
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Inline = false
            });
            overlayComponent.Visible = false;

            windowControlsPanel = new BlankUI(UITheme.ColorType.Placeholder);
            windowControlsPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Inline = false,
            });
            windowControlsPanel.FocusIgnore = true;

            windowControlsParentTop = new BlankUI(UITheme.ColorType.Placeholder);
            windowControlsParentTop.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(30),
                LayoutAxis = LVector2.HORIZONTAL,
                HorizontalAlignment = Alignment.End
            });
            windowControlsParentTop.FocusIgnore = true;

            windowControlsPanel.AddChild(windowControlsParentTop);

            if (window.IsBorderless) AddWindowControls();
            windowBorderless = window.IsBorderless;

            topBarBlock = uiFactory.CreateTopBar();
            UIComponent topBar = topBarBlock.Root;
            topBarBlock.Orders.Root.OnClick(() => { SetMainWindowState(MainWindowState.Orders); } );
            topBarBlock.Dashboard.Root.OnClick(() => { SetMainWindowState(MainWindowState.Dashboard); });

            mainWindowContainer = new BlankUI(UITheme.ColorType.Background);
            mainWindowContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
            });

            UIComponent dashboard = SetupDashboard(uiFactory.CreateDashboard());

            currentWindow = dashboard;
            HighlightTopBar(MainWindowState.Dashboard);

            mainWindowContainer.AddChild(dashboard);

            rootComponent.AddChild(topBar);
            rootComponent.AddChild(mainWindowContainer);

            rootComponent.AddChild(overlayComponent);
            rootComponent.AddChild(windowControlsPanel);

            RecalculatePriority();
            rootComponent.SetCheckFocusCallback(CheckFocus);

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
            if (priority > focusedComponent.priority)
            {
                if (focusedComponent.alertUnfocus != null) focusedComponent.alertUnfocus();
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
            focusedComponent.alertUnfocus?.Invoke();
            focusedComponent = (null, null, -1);
            rootComponent.PreUpdate();
            focusedComponent.alertFocus?.Invoke();
        }

        private void PostUpdate()
        {
            rootComponent.PostUpdate();
            KeyboardHandler.SetOldKeyboard();
            MouseHandler.SetOldMouse();
        }

        private void ImportTextureAction(Texture2D texture)
        {
            Console.WriteLine("Importing Texture!");
            overlayComponent.Visible = false;

            string filePath = $"C:\\Users\\bill\\Documents\\C#\\{LunaDateTime.Now.ShortDisplayAlt}.png";
            Console.WriteLine(filePath);
            FileStream s = new FileStream(filePath, FileMode.Create);
            texture.SaveAsPng(s, texture.Width, texture.Height);
            s.Close();
        }

        private void CancelImportAction()
        {
            overlayComponent.Visible = false;
        }

        private void AddWindowControls()
        {
            Button minimise = new Button(UITheme.ColorType.Background);
            minimise.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(50),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            minimise.SetTheme(new UITheme(){ CornerRadius = (0, 0, 10, 0) });
            minimise.OnClick(() => NativeWindowHelper.ShowWindow(window.Handle, NativeWindowHelper.SW_MINIMISE));

            Label minimiseLabel = new Label("_", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            minimise.AddChild(minimiseLabel);

            Button maximise = new Button(UITheme.ColorType.Background);
            maximise.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(50),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            maximise.SetTheme(new UITheme(){ Rounded = false });
            maximise.OnClick(() => NativeWindowHelper.ShowWindow(window.Handle, NativeWindowHelper.IsZoomed(window.Handle) ? NativeWindowHelper.SW_RESTORE : NativeWindowHelper.SW_MAXIMISE));

            Label maximiseLabel = new Label("O", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Background);
            maximise.AddChild(maximiseLabel);
            
            Button quit = new Button(UITheme.ColorType.Background);
            quit.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(50),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            quit.SetTheme(new UITheme(){ ColourType = UITheme.ColorType.Emergency, Rounded = false });
            quit.OnClick(quitAction);

            Label quitButton = new Label("X", GraphicsHelper.GetDefaultFont(), UITheme.ColorType.Emergency);
            quit.AddChild(quitButton);

            windowControlsParentTop.AddChild(minimise);
            windowControlsParentTop.AddChild(maximise);
            windowControlsParentTop.AddChild(quit);
        }

        private void RemoveWindowControls()
        {
            foreach (UIComponent c in windowControlsParentTop.GetChildren())
            {
                c.Destroy();
            }
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
                orderBlock.Root.OnClick(() => { Texture2D tex = IOManager.LoadImageFromDialog();
                    if (tex == null) return;
                    overlayComponent.Visible = true;
                    YesNoBlock imageImporter = uiFactory.CreateImageImporter(new LTexture2D(tex), ImportTextureAction, CancelImportAction);
                    imageImporter.Root.ForceTransparent();
                    overlayComponent.AddChild(imageImporter.Root); });
            }

            return dashboardBlock.Root;
        }

        private UIComponent SetupOrders(OrdersBlock ordersBlock)
        {
            return ordersBlock.Root;
        }

        private void InitiateFadeTransition(UIComponent newWindow)
        {
            mainWindowContainer.AddChild(newWindow); newWindow.ColourAnimator.OnTransitionAction(() => { mainWindowContainer.RemoveChild(currentWindow); currentWindow = newWindow; });
        }

        private void HighlightTopBar(MainWindowState mainWindowState)
        {
            switch (mainWindowState)
            {
                case MainWindowState.Dashboard:
                {
                    topBarBlock.Dashboard.Root.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Highlit });
                    topBarBlock.Dashboard.Label.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Highlit });
                    topBarBlock.Orders.Root.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Main });
                    topBarBlock.Orders.Label.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Main });
                    break;
                }
                case MainWindowState.Orders:
                {
                    topBarBlock.Dashboard.Root.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Main });
                    topBarBlock.Dashboard.Label.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Main });
                    topBarBlock.Orders.Root.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Highlit });
                    topBarBlock.Orders.Label.SetTheme(new UITheme() { ColourType = UITheme.ColorType.Highlit });
                    break;
                }
            }
        }

        public void SetUpdateScissorRectangleAction(GraphicsDevice graphicsDevice)
        {
            UIComponent.SetUpdateScissorRectangleAction((Rectangle r) => { graphicsDevice.ScissorRectangle = r; });
        }
    }
}
