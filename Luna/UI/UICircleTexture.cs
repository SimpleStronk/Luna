using System;
using Luna.HelperClasses;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework.Graphics;

namespace Luna.UI
{
    internal class UICircleTexture : UITexture
    {
        public UICircleTexture()
        {
            Initialise();
            RenderDefaultRect = false;
        }

        protected override void Update()
        {

        }

        protected override void Draw(SpriteBatch s)
        {
            base.Draw(s);
        }

        protected override void OnResize()
        {
            base.OnResize();

            int size = (int)Math.Min(transform.Size.X, transform.Size.Y);
            Texture = new LTexture2D(GraphicsHelper.GenerateCircleTexture(size));
        }
    }
}