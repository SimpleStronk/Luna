using Luna.UI.LayoutSystem;
using Luna.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Luna.UI.LayoutSystem.LUIVA;
using Luna.DataClasses;
using static Luna.UI.UIFactory;
using Luna.UI;

namespace Luna.ManagerClasses
{
    internal class UIManager
    {
        private Texture2D pixelTexture;

        private UIComponent rootComponent;
        private (Action alertFocus, Action alertUnfocus, int priority) focusedComponent;
        private LUIVA luiva;

        public UIManager(GameWindow window, GraphicsDevice graphicsDevice, SystemManager systemManager)
        {
            rootComponent = new BlankUI();
            rootComponent.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.VERTICAL
            });

            BlankUI topBar = new BlankUI();
            topBar.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(50),
                Padding = new Tetra(5),
                Spacing = 5
            });

            Button logoContainer = new Button();
            logoContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Fixed(120),
                LayoutHeight = Sizing.Grow(1),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Label logo = new Label("Luna", GraphicsHelper.GetDefaultFont());

            Button dashboardContainer = new Button();
            dashboardContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Wrap(),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10)
            });

            Label dashboardLabel = new Label("Ay up", GraphicsHelper.GetDefaultFont());

            Button ordersContainer = new Button();
            ordersContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Wrap(),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10)
            });

            Label ordersLabel = new Label("This button must conform to the whim of the text", GraphicsHelper.GetDefaultFont());

            BlankUI mainPanel = new BlankUI();
            mainPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                LayoutAxis = LVector2.HORIZONTAL
            });

            BlankUI leftPanel = new BlankUI();
            leftPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(10),
                Spacing = 10
            });

            BlankUI rightPanel = new BlankUI();
            rightPanel.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(2),
                LayoutHeight = Sizing.Grow(1),
                Padding = new Tetra(20),
                LayoutAxis = LVector2.VERTICAL
            });

            BlankUI contentContainer = new BlankUI();
            contentContainer.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                LayoutAxis = LVector2.VERTICAL,
                Padding = new Tetra(10),
                Spacing = 10
            });

            Button b1 = new Button();
            b1.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(70),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Button b2 = new Button();
            b2.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Wrap(),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                Padding = new Tetra(10)
            });

            Button b3 = new Button();
            b3.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1),
                LayoutHeight = Sizing.Fixed(70),
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            });

            Label l1 = new Label("Testing", GraphicsHelper.GetDefaultFont());
            Label l2 = new Label("This label is definitely too big. Let's have a moment of silence. In fact, notice how this button is bigger because the text is longer!", GraphicsHelper.GetDefaultFont());
            l2.SetLayout(new Layout()
            {
                LayoutWidth = Sizing.Grow(1)
            });

            b1.AddChild(l1);
            b2.AddChild(l2);

            contentContainer.AddChild(b1);
            contentContainer.AddChild(b2);
            contentContainer.AddChild(b3);

            leftPanel.AddChild(contentContainer);

            UIFactory uiFactory = new UIFactory();
            foreach (Order o in systemManager.GetOrders())
            {
                OrderBlock orderBlock = uiFactory.CreateOrder(o);
                rightPanel.AddChild(orderBlock.Root);
                orderBlock.Root.OnClick(() => { rightPanel.AddChild(uiFactory.CreateImageImporter(new LTexture2D(IOManager.LoadImageFromDialog()), ImportTextureAction).Root); });
            }

            mainPanel.AddChild(leftPanel);
            mainPanel.AddChild(rightPanel);

            logoContainer.AddChild(logo);

            dashboardContainer.AddChild(dashboardLabel);

            ordersContainer.AddChild(ordersLabel);

            topBar.AddChild(logoContainer);
            topBar.AddChild(dashboardContainer);
            topBar.AddChild(ordersContainer);

            rootComponent.AddChild(topBar);
            rootComponent.AddChild(mainPanel);

            rootComponent.ForceSynchChildren();
            
            rootComponent.CascadeTheme(MainTheme);
            logoContainer.SetTheme(ButtonTheme);
            logo.SetTheme(ButtonTheme);
            dashboardContainer.SetTheme(ButtonTheme);
            dashboardLabel.SetTheme(ButtonTheme);
            ordersContainer.SetTheme(ButtonTheme);
            ordersLabel.SetTheme(ButtonTheme);
            b1.SetTheme(ButtonTheme);
            l1.SetTheme(ButtonTheme);
            b2.SetTheme(ButtonTheme);
            l2.SetTheme(ButtonTheme);
            b3.SetTheme(ButtonTheme);
            rightPanel.SetTheme(BackgroundTheme);

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
        }

        public void SetUpdateScissorRectangleAction(GraphicsDevice graphicsDevice)
        {
            UIComponent.SetUpdateScissorRectangleAction((Rectangle r) => { graphicsDevice.ScissorRectangle = r; });
        }

        #region themes
        private UITheme MainTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(70, 70, 70, 255),
                    SecondaryColour = new Color(255, 255, 255, 255),
                    HoveredColour = new Color(80, 80, 80, 255),
                    SelectedColour = new Color(90, 90, 90, 255)
                };
            }
        }

        private UITheme ButtonTheme
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

        private UITheme OrderTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(230, 230, 230, 255),
                    CornerRadius = (7, 7, 7, 7)
                };
            }
        }

        private UITheme BackgroundTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(50, 50, 50, 255)
                };
            }
        }

        private UITheme ShadowTheme
        {
            get
            {
                return new UITheme()
                {
                    MainColour = new Color(0, 0, 0, 255) * 0.5f
                };
            }
        }
        #endregion
    }
}
