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

        /// <summary>
        /// Creates a new LTexture2D object from the given Texture2D object
        /// </summary>
        public LTexture2D(Texture2D texture)
        {
            Texture = texture;
        }

        /// <summary>
        /// The internal Texture2D value of this object
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; aspectRatio = (float)texture.Width / (float)texture.Height;
                displayDimensions.X = texture.Width; displayDimensions.Y = texture.Height;
                scale = [1, 1]; }
        }

        /// <summary>
        /// The aspect ratio of this object
        /// </summary>
        public float AspectRatio
        {
            get { return aspectRatio; }
        }

        /// <summary>
        /// Whether to force this object to keep its aspect ratio
        /// </summary>
        public bool LockAspectRatio
        {
            get { return lockAspectRatio; }
            set { lockAspectRatio = value; }
        }

        /// <summary>
        /// Width of the internal Texture2D value
        /// </summary>
        public int Width
        {
            get { return texture.Width; }
        }

        /// <summary>
        /// Height of the internal Texture2D value
        /// </summary>
        public int Height
        {
            get { return texture.Height; }
        }

        /// <summary>
        /// How wide this LTexture2D object should display on screen
        /// </summary>
        public float DisplayWidth
        {
            get { return displayDimensions.X; }
            set { displayDimensions.X = value; if (lockAspectRatio) {
                    displayDimensions.Y = displayDimensions.X / aspectRatio;
                    scale = [displayDimensions.X / Width, displayDimensions.Y / Height]; } }
        }

        /// <summary>
        /// How tall this LTexture2D object should display on screen
        /// </summary>
        public float DisplayHeight
        {
            get { return displayDimensions.Y; }
            set { displayDimensions.Y = value; if (lockAspectRatio) {
                    displayDimensions.X = displayDimensions.Y * aspectRatio;
                    scale = [displayDimensions.X / Width, displayDimensions.Y / Height]; } }
        }

        /// <summary>
        /// How large this LTexture2D object should display on screen
        /// </summary>
        public LVector2 DisplayDimensions
        {
            get { return displayDimensions; }
        }

        /// <summary>
        /// Sets the given axis dimension of how large this object should be on screen
        /// </summary>
        /// <param name="dimension">The size to assign to the given axis</param>
        /// <param name="axis">The axis to set the size of. Use <c>LVector2.HORIZONTAL</c> or <c>LVector2.VERTICAL</c></param>
        public void SetDisplayDimension(float dimension, int axis)
        {
            if (axis == LVector2.HORIZONTAL) DisplayWidth = dimension;
            if (axis == LVector2.VERTICAL) DisplayHeight = dimension;
        }

        /// <summary>
        /// The on-screen size of this object relative to its original size
        /// </summary>
        public LVector2 Scale
        {
            get { return new LVector2(scale[0], scale[1]); }
        }
    }
}