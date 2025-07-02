using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Luna.HelperClasses
{
    internal class GraphicsHelper
    {
        private static GraphicsDevice graphicsDevice;
        private static SpriteFont defaultFont, boldFont;
        private static int msaaRes = 3;
        private static Dictionary<int, Texture2D> circleCache = new Dictionary<int, Texture2D>();
        private static Texture2D luivaLogo;

        /// <summary>
        /// Generates a Texture2D object representing a single white pixel
        /// </summary>
        /// <exception cref="Exception">Thrown when this object's GraphicsDevice is null</exception>
        public static Texture2D GeneratePixelTexture()
        {
            if (graphicsDevice == null) throw new Exception("graphicsDevice not initialised in class GraphicsHelper");

            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });
            return texture;
        }

        /// <summary>
        /// Generates a Texture2D object representing a circle with the given diameter
        /// </summary>
        /// <param name="diameter">Diameter of the generated circle</param>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown when this object's GraphicsDevice is null</exception>
        public static Texture2D GenerateCircleTexture(int diameter)
        {
            if (diameter <= 0) return null;
            
            if (circleCache.ContainsKey(diameter)) return circleCache[diameter];

            if (graphicsDevice == null) throw new Exception("graphicsDevice not initialised in class GraphicsHelper");

            Texture2D tex = new Texture2D(graphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            for (int x = 0; x < tex.Width; x++)
            {
                for (int y = 0; y < tex.Height; y++)
                {
                    float weight = 0;
                    for (int i = 0; i < msaaRes; i++)
                    {
                        for (int j = 0; j < msaaRes; j++)
                        {
                            Vector2 relativePos = new Vector2(x * msaaRes + i, y * msaaRes + j) / (msaaRes * diameter * 0.5f);
                            relativePos -= new Vector2(1, 1);

                            weight += (relativePos.LengthSquared() > 1 ? 0 : 1) / (float)Math.Pow(msaaRes, 2);
                        }
                    }
                    data[x * diameter + y] = new Color(255, 255, 255, 255) * weight;
                }
            }
            tex.SetData(data);
            circleCache.Add(diameter, tex);
            return tex;
        }

        public static GraphicsDevice GetGraphicsDevice()
        {
            return graphicsDevice;
        }

        public static void SetGraphicsDevice(GraphicsDevice graphicsDevice)
        {
            GraphicsHelper.graphicsDevice = graphicsDevice;
        }

        public static void SetDefaultFont(SpriteFont defaultFont)
        {
            GraphicsHelper.defaultFont = defaultFont;
        }

        public static void SetBoldFont(SpriteFont boldFont)
        {
            GraphicsHelper.boldFont = boldFont;
        }

        public static SpriteFont GetDefaultFont()
        {
            return defaultFont;
        }

        public static SpriteFont GetBoldFont()
        {
            return boldFont;
        }

        /// <summary>
        /// Gets the colour data of a smaller region within a larger image
        /// </summary>
        /// <param name="colourData">Colour information of the source image</param>
        /// <param name="width">Width of the original image we are sampling from</param>
        /// <param name="sampleRectangle">The region within the original image we are sampling</param>
        /// <returns></returns>
        public static Color[] GetImageData(Color[] colourData, int width, Rectangle sampleRectangle)
        {
            Color[] sample = new Color[sampleRectangle.Width * sampleRectangle.Height];

            for (int x = 0; x < sampleRectangle.Width; x++)
            {
                for (int y = 0; y < sampleRectangle.Height; y++)
                {
                    sample[x + y * sampleRectangle.Width] = colourData[x + sampleRectangle.X + ((y + sampleRectangle.Y) * width)];
                }
            }

            return sample;
        }

        public static Texture2D LuivaLogo
        {
            get { return luivaLogo; }
            set { luivaLogo = value; }
        }
    }
}
