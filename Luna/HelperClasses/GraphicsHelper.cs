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

        public static Texture2D GeneratePixelTexture()
        {
            if (graphicsDevice == null) throw new Exception("graphicsDevice not initialised in class GraphicsHelper");

            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });
            return texture;
        }

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

        public static Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
        {
            Color[] color = new Color[rectangle.Width * rectangle.Height];
            for (int x = 0; x < rectangle.Width; x++)
                for (int y = 0; y < rectangle.Height; y++)
                    color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
            return color;
        }
    }
}
