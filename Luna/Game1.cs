using System;
using System.IO;
using FontStashSharp;
using Luna.HelperClasses;
using Luna.ManagerClasses;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Luna;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem defaultFontSystem = new FontSystem(),
        boldFontSystem = new FontSystem();

    UIManager uiManager;
    SystemManager systemManager = new SystemManager();

    RasterizerState s;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.IsBorderless = false;
        Window.AllowUserResizing = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        IOManager.SetGraphicsDevice(GraphicsDevice);
        GraphicsHelper.SetGraphicsDevice(GraphicsDevice);
        GraphicsHelper.SetDefaultFont(Content.Load<SpriteFont>(@"MontserratLight"));
        GraphicsHelper.SetBoldFont(Content.Load<SpriteFont>(@"MontserratRegular"));
        defaultFontSystem.AddFont(File.ReadAllBytes(@"Content/Montserrat-Light.ttf"));
        boldFontSystem.AddFont(File.ReadAllBytes(@"Content/Montserrat-Regular.ttf"));
        GraphicsHelper.SetDefaultFontSystem(defaultFontSystem);
        GraphicsHelper.SetBoldFontSystem(boldFontSystem);
        GraphicsHelper.LuivaLogo = Content.Load<Texture2D>(@"LUIVA");
        uiManager = new UIManager(Window, Exit, GraphicsDevice, systemManager);
        uiManager.SetPixelTexture(GraphicsHelper.GeneratePixelTexture());

        s = new RasterizerState { ScissorTestEnable = true };
        GraphicsDevice.RasterizerState = s;
        uiManager.SetUpdateScissorRectangleAction(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (!IsActive) return;

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
           Exit();
        KeyboardHandler.IncrementTime((float)gameTime.ElapsedGameTime.TotalSeconds);
        
        uiManager.Update(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, s, null, null);

        uiManager.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
