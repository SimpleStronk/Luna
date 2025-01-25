using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace Luna.ManagerClasses
{
    internal class IOManager
    {
        static GraphicsDevice graphicsDevice;

        public static Texture2D LoadImageFromDialog()
        {
            FileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg";
            DialogResult result = fileDialog.ShowDialog();

            if (result != DialogResult.OK) return null;

            return LoadImageFromFile(fileDialog.FileName);
        }

        public static Texture2D LoadImageFromFile(string filePath)
        {
            Console.WriteLine(filePath);
            return Texture2D.FromFile(graphicsDevice, filePath);
        }

        public static void SetGraphicsDevice(GraphicsDevice graphicsDevice)
        {
            IOManager.graphicsDevice = graphicsDevice;
        }
    }
}