using System;
using System.IO;
using Luna.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using Luna.UI.LayoutSystem;
using Luna.DataClasses;

namespace Luna.UI
{
    internal class UITexture : UIComponent
    {
        protected Texture2D pixel;
        protected LTexture2D texture;
        private float transformAspectRatio;
        protected int fitAxis;
        protected int maxTextureOffset;
        protected float manualTextureOffset;

        public UITexture() : base(false)
        {
            pixel = GraphicsHelper.GeneratePixelTexture();
            Initialise();
            RenderDefaultRect = false;
        }

        public UITexture(LTexture2D texture) : base(false)
        {
            Texture = texture;
            Initialise();
            RenderDefaultRect = false;
        }

        protected override void Update()
        {
        }

        protected override void Draw(SpriteBatch s)
        {
            LVector2 offset = GetAlignmentOffset();
            s.Draw(texture == null ? pixel : texture.Texture, new Rectangle(transform.GetGlobalRect().X + (int)offset.X, transform.GetGlobalRect().Y + (int)offset.Y, (int)texture.DisplayWidth, (int)texture.DisplayHeight),
                texture == null ? new Rectangle(0, 0, 1, 1) : new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 1);
        }

        protected override void OnResize()
        {
            base.OnResize();

            transformAspectRatio = transform.Size.X / transform.Size.Y;
            RecalculateFitAxis();
        }

        public LTexture2D Texture
        {
            get { return texture; }
            set { texture = value; RecalculateFitAxis(); }
        }

        private LVector2 GetAlignmentOffset()
        {
            LVector2 offset = new LVector2();

            switch (layout.ImageAlignment)
            {
                case LUIVA.Alignment.Begin:
                {
                    return LVector2.Zero;
                }
                case LUIVA.Alignment.Middle:
                {
                    manualTextureOffset = maxTextureOffset / 2;
                    offset.SetComponentValue(manualTextureOffset, LVector2.AlternateAxis(fitAxis));
                    break;
                }
                case LUIVA.Alignment.End:
                {
                    manualTextureOffset = maxTextureOffset;
                    offset.SetComponentValue(manualTextureOffset, LVector2.AlternateAxis(fitAxis));
                    break;
                }
                case LUIVA.Alignment.Ignore:
                {
                    offset.SetComponentValue(manualTextureOffset, LVector2.AlternateAxis(fitAxis));
                    break;
                }
            }

            return offset;
        }

        private void RecalculateFitAxis()
        {
            switch (layout.ImageFitMode)
            {
                case LUIVA.Layout.FitMode.MinFit:
                {
                    fitAxis = texture.AspectRatio > transformAspectRatio ? LVector2.HORIZONTAL : LVector2.VERTICAL;
                    int offsetAxis = LVector2.AlternateAxis(fitAxis);
                    texture.SetDisplayDimension(transform.Size.GetComponent(fitAxis), fitAxis);
                    break;
                }
                case LUIVA.Layout.FitMode.MaxFit:
                {
                    fitAxis = texture.AspectRatio > transformAspectRatio ? LVector2.VERTICAL : LVector2.HORIZONTAL;
                    int offsetAxis = LVector2.AlternateAxis(fitAxis);
                    texture.SetDisplayDimension(transform.Size.GetComponent(fitAxis), fitAxis);
                    break;
                }
            }

            maxTextureOffset = (int)(transform.Size.GetComponent(LVector2.AlternateAxis(fitAxis)) - texture.DisplayDimensions.GetComponent(LVector2.AlternateAxis(fitAxis)));
        }

        public Texture2D GetVisibleSubtexture()
        {
            Color[] imageData = new Color[texture.Width * texture.Height];
            texture.Texture.GetData(imageData);

            Rectangle textureRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle displayRect = GetDisplayRelativeToTexture();
            Rectangle samplerRect = Rectangle.Intersect(textureRect, displayRect);

            Color[] subTextureData = GraphicsHelper.GetImageData(imageData, texture.Width, samplerRect);
            Texture2D subTexture = new Texture2D(GraphicsHelper.GetGraphicsDevice(), samplerRect.Width, samplerRect.Height);
            subTexture.SetData(subTextureData);
            
            return subTexture;
        }

        private Rectangle GetDisplayRelativeToTexture()
        {
            // transform rect / scale of texture
            LVector2 relativeOffset = LVector2.Zero;
            relativeOffset.SetComponentValue(0, fitAxis);
            relativeOffset.SetComponentValue(-manualTextureOffset / texture.Scale.GetComponent(LVector2.AlternateAxis(fitAxis)), LVector2.AlternateAxis(fitAxis));
            LVector2 relativeSize = transform.Size / texture.Scale;
            return new Rectangle((int)relativeOffset.X, (int)relativeOffset.Y, (int)relativeSize.X, (int)relativeSize.Y);
        }

        protected override string GetComponentType()
        {
            return "UITexture";
        }
    }
}