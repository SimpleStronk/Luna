using System;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;

namespace Luna.UI.LayoutSystem
{
    internal class LTexture2D
    {
        private Texture2D texture;
        private float aspectRatio;
        private bool lockAspectRatio = true;
        private LVector2 displayDimensions = new LVector2(0, 0);
        private float[] scale = [1, 1];

        public LTexture2D(Texture2D texture)
        {
            Texture = texture;
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; aspectRatio = (float)texture.Width / (float)texture.Height;
                displayDimensions.X = texture.Width; displayDimensions.Y = texture.Height;
                scale = [1, 1]; }
        }

        public float AspectRatio
        {
            get { return aspectRatio; }
        }

        public bool LockAspectRatio
        {
            get { return lockAspectRatio; }
            set { lockAspectRatio = value; }
        }

        public int Width
        {
            get { return texture.Width; }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public float DisplayWidth
        {
            get { return displayDimensions.X; }
            set { displayDimensions.X = value;
                if (lockAspectRatio) { displayDimensions.Y = displayDimensions.X / aspectRatio;
                scale = [displayDimensions.X / Width, displayDimensions.Y / Height]; } }
        }

        public float DisplayHeight
        {
            get { return displayDimensions.Y; }
            set { displayDimensions.Y = value;
                if (lockAspectRatio) { displayDimensions.X = displayDimensions.Y * aspectRatio;
                scale = [displayDimensions.X / Width, displayDimensions.Y / Height]; } }
        }

        public LVector2 DisplayDimensions
        {
            get { return displayDimensions; }
        }

        public void SetDisplayDimension(float dimension, int axis)
        {
            if (axis == LVector2.HORIZONTAL) DisplayWidth = dimension;
            if (axis == LVector2.VERTICAL) DisplayHeight = dimension;
        }

        public LVector2 Scale
        {
            get { return new LVector2(scale[0], scale[1]); }
        }
    }
}