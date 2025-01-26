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

        private UIComponent rootComponent;
        private UIComponent overlayComponent;
        private UIComponent windowControlsPanel, windowControlsParentTop;
        private (Action alertFocus, Action alertUnfocus, int priority) focusedComponent;
        private Action quitAction;
        private LUIVA luiva;
        private GameWindow window;
        private bool windowBorderless;

        public UIManager(GameWindow window, Action quitAction, GraphicsDevice graphicsDevice, SystemManager systemManager)
        {
            this.window = window;
            this.quitAction = quitAction;

            rootComponent = new BlankUI(PlumTheme, UITheme.ColorType.Background);
            rootComponent.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.VERTICAL
            });

            overlayComponent = new BlankUI(PlumTheme, UITheme.ColorType.Shadow);
            overlayComponent.SetLayout(new Layout()
            {
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Inline = false
            });
            overlayComponent.Visible = false;

            windowControlsPanel = new BlankUI(PlumTheme, UITheme.ColorType.Placeholder);
            windowControlsPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Inline = false,
            });
            windowControlsPanel.FocusIgnore = true;

            windowControlsParentTop = new BlankUI(PlumTheme, UITheme.ColorType.Placeholder);
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

            BlankUI topBar = new BlankUI(PlumTheme, UITheme.ColorType.Main);
            topBar.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(65),
                Padding = new Tetra(5),
                Spacing = 5
            });

            Button logoContainer = new Button(PlumTheme, UITheme.ColorType.Main);
            logoContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(120),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Label logo = new Label("Luna", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Main);

            Button dashboardContainer = new Button(PlumTheme, UITheme.ColorType.Main);
            dashboardContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Wrap(),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Label dashboardLabel = new Label("Ay up", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Main);

            Button ordersContainer = new Button(PlumTheme, UITheme.ColorType.Main);
            ordersContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Wrap(),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Label ordersLabel = new Label("This button must conform to the whim of the text", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Main);

            BlankUI topBarSeparator = new BlankUI(PlumTheme, UITheme.ColorType.Background);
            topBarSeparator.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(2)
            });

            BlankUI mainPanel = new BlankUI(PlumTheme, UITheme.ColorType.Background);
            mainPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.HORIZONTAL
            });

            BlankUI leftPanel = new BlankUI(PlumTheme, UITheme.ColorType.Background);
            leftPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10),
                Spacing = 10
            });

            BlankUI rightPanel = new BlankUI(PlumTheme, UITheme.ColorType.MainSoft);
            rightPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(2),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(20),
                LayoutAxis = LVector2.VERTICAL
            });

            BlankUI contentContainer = new BlankUI(PlumTheme, UITheme.ColorType.Background);
            contentContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL,
                Padding = new Tetra(10),
                Spacing = 10
            });

            Button b1 = new Button(PlumTheme, UITheme.ColorType.Background);
            b1.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(70),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Button b2 = new Button(PlumTheme, UITheme.ColorType.Background);
            b2.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                Padding = new Tetra(10)
            });

            Button b3 = new Button(PlumTheme, UITheme.ColorType.Background);
            b3.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(70),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            BlankUI contentSeparator = new(PlumTheme, UITheme.ColorType.Background);
            contentSeparator.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(2),
                LayoutHeight = Sizing.Grow(1)
            });

            Label l1 = new Label("Testing", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Background);
            Label l2 = new Label("This label is definitely too big. Let's have a moment of silence. In fact, notice how this button is bigger because the text is longer!", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Background);
            l2.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1)
            });
            Label l3 = new Label("Holy hell!", GraphicsHelper.GetDefaultFont(), UIFactory.PlumTheme, UITheme.ColorType.Background);

            b1.AddChild(l1);
            b2.AddChild(l2);
            b3.AddChild(l3);

            contentContainer.AddChild(b1);
            contentContainer.AddChild(b2);
            contentContainer.AddChild(b3);

            leftPanel.AddChild(contentContainer);

            UIFactory uiFactory = new UIFactory();
            foreach (Order o in systemManager.GetOrders())
            {
                if (rightPanel.GetChildCount() != 0)
                {
                    BlankUI separator = new BlankUI(PlumTheme, UITheme.ColorType.Background);
                    separator.SetLayout(new Layout()
                    {
                        LayoutWidth = Sizing.Grow(1),
                        LayoutHeight = Sizing.Fixed(2)
                    });
                    separator.GetTheme().ColourType = UITheme.ColorType.Separator;
                    rightPanel.AddChild(separator);
                }
                OrderBlock orderBlock = uiFactory.CreateOrder(o);
                rightPanel.AddChild(orderBlock.Root);
                orderBlock.Root.OnClick(() => { Texture2D tex = IOManager.LoadImageFromDialog();
                    if (tex == null) return;
                    overlayComponent.Visible = true;
                    overlayComponent.AddChild(uiFactory.CreateImageImporter(new LTexture2D(tex), ImportTextureAction, CancelImportAction).Root); });
            }

            mainPanel.AddChild(leftPanel);
            mainPanel.AddChild(contentSeparator);
            mainPanel.AddChild(rightPanel);

            logoContainer.AddChild(logo);

            dashboardContainer.AddChild(dashboardLabel);

            ordersContainer.AddChild(ordersLabel);

            topBar.AddChild(logoContainer);
            topBar.AddChild(dashboardContainer);
            topBar.AddChild(ordersContainer);

            rootComponent.AddChild(topBar);
            //rootComponent.AddChild(topBarSeparator);
            rootComponent.AddChild(mainPanel);

            rootComponent.AddChild(overlayComponent);
            rootComponent.AddChild(windowControlsPanel);

            rootComponent.ForceSynchChildren();

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

            RecalculatePriority();
            luiva.CalculateLayout();
            focusedComponent.alertUnfocus?.Invoke();
            focusedComponent = (null, null, -1);
            rootComponent.PreUpdate();
            focusedComponent.alertFocus?.Invoke();
        }

        private void PostUpdate()
        {
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
            Button minimise = new Button(PlumTheme, UITheme.ColorType.Background);
            minimise.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(50),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            minimise.GetTheme().CornerRadius = (0, 0, 7, 0);
            minimise.OnClick(() => NativeWindowHelper.ShowWindow(window.Handle, NativeWindowHelper.SW_MINIMISE));

            Label minimiseLabel = new Label("_", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Background);
            minimise.AddChild(minimiseLabel);

            Button maximise = new Button(PlumTheme, UITheme.ColorType.Background);
            maximise.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(50),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            maximise.OnClick(() => NativeWindowHelper.ShowWindow(window.Handle, NativeWindowHelper.IsZoomed(window.Handle) ? NativeWindowHelper.SW_RESTORE : NativeWindowHelper.SW_MAXIMISE));

            Label maximiseLabel = new Label("O", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Background);
            maximise.AddChild(maximiseLabel);
            
            Button quit = new Button(PlumTheme, UITheme.ColorType.Emergency);
            quit.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(50),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });
            quit.GetTheme().ColourType = UITheme.ColorType.Emergency;
            quit.OnClick(quitAction);

            Label quitButton = new Label("X", GraphicsHelper.GetDefaultFont(), PlumTheme, UITheme.ColorType.Emergency);
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

        public void SetUpdateScissorRectangleAction(GraphicsDevice graphicsDevice)
        {
            UIComponent.SetUpdateScissorRectangleAction((Rectangle r) => { graphicsDevice.ScissorRectangle = r; });
        }
    }
}
