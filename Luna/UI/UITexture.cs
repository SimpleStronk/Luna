using System;
using System.IO;
using Luna.HelperClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using Luna.UI.LayoutSystem;

namespace Luna.UI
{
    internal class UITexture : UIComponent
    {
        protected Texture2D pixel;
        protected LTexture2D texture;
        private float transformAspectRatio;
        protected int textureOffsetAxis;
        protected int maxTextureOffset;
        protected float manualTextureOffset;

        public UITexture()
        {
            pixel = GraphicsHelper.GeneratePixelTexture();
            Initialise();
            RenderDefaultRect = false;
        }

        public UITexture(LTexture2D texture)
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
                    offset.SetComponentValue((float)(maxTextureOffset / 2), textureOffsetAxis);
                    break;
                }
                case LUIVA.Alignment.End:
                {
                    offset.SetComponentValue(maxTextureOffset, textureOffsetAxis);
                    break;
                }
                case LUIVA.Alignment.Ignore:
                {
                    offset.SetComponentValue(manualTextureOffset, textureOffsetAxis);
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
                    textureOffsetAxis = texture.AspectRatio > transformAspectRatio ? LVector2.VERTICAL : LVector2.HORIZONTAL;
                    int fitAxis = LVector2.AlternateAxis(textureOffsetAxis);
                    texture.SetDisplayDimension(transform.Size.GetComponent(fitAxis), fitAxis);
                    break;
                }
                case LUIVA.Layout.FitMode.MaxFit:
                {
                    textureOffsetAxis = texture.AspectRatio > transformAspectRatio ? LVector2.HORIZONTAL : LVector2.VERTICAL;
                    int fitAxis = LVector2.AlternateAxis(textureOffsetAxis);
                    texture.SetDisplayDimension(transform.Size.GetComponent(fitAxis), fitAxis);
                    break;
                }
            }

            maxTextureOffset = (int)(transform.Size.GetComponent(textureOffsetAxis) - texture.DisplayDimensions.GetComponent(textureOffsetAxis));
        }

        protected override string GetComponentType()
        {
            return "UITexture";
        }
    }
}