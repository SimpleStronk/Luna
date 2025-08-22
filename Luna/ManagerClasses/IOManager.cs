using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;

namespace Luna.ManagerClasses
{
    internal class IOManager
    {
        static GraphicsDevice graphicsDevice;

        /// <summary>
        /// Creates a FileDialog popup and loads the resultant file as a Texture2D object
        /// </summary>
        public static Texture2D LoadImageFromDialog()
        {
            FileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg";
            DialogResult result = fileDialog.ShowDialog();

            if (result != DialogResult.OK) return null;

            return LoadImageFromFile(fileDialog.FileName);
        }

        /// <summary>
        /// Loads the specified image as a Texture2D object
        /// </summary>
        /// <param name="filePath">File path of the image to load</param>
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