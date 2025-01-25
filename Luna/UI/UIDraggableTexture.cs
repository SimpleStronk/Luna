using System;
using Luna.HelperClasses;
using Luna.ManagerClasses;
using Luna.UI.LayoutSystem;

namespace Luna.UI
{
    internal class UIDraggableTexture : UITexture
    {
        bool dragging = false;

        public UIDraggableTexture()
        {
            pixel = GraphicsHelper.GeneratePixelTexture();
            Initialise();
            RenderDefaultRect = false;
        }

        public UIDraggableTexture(LTexture2D texture)
        {
            this.texture = texture;
            Initialise();
            RenderDefaultRect = false;
            layout.ImageAlignment = LUIVA.Alignment.Ignore;
        }

        protected override void Update()
        {
            if (dragging)
            {
                if (textureOffsetAxis == LVector2.HORIZONTAL) manualTextureOffset += MouseHandler.DeltaPosition.X;
                if (textureOffsetAxis == LVector2.VERTICAL) manualTextureOffset += MouseHandler.DeltaPosition.Y;
            }

            if (maxTextureOffset < 0) manualTextureOffset = Math.Clamp(manualTextureOffset, maxTextureOffset, 0);

            DoDraggableCheck();
        }

        private void DoDraggableCheck()
        {
            Console.WriteLine("Draggable Check");
            if (maxTextureOffset >= 0) return;

            if (MouseHandler.IsJustClicked(MouseHandler.MouseButton.Left) && focused)
            {
                dragging = true;
                Console.WriteLine("Dragging");
            }

            if (!MouseHandler.IsClicked(MouseHandler.MouseButton.Left)) dragging = false;
        }
    }
}