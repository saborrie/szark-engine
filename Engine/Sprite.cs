/*
	Sprite.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

using System;
using System.IO;

namespace PGE
{
    /// <summary>
    /// A Class that contains an Array of Pixels
    /// and may Represent an Image
    /// </summary>
    public class Sprite
    {
        public int Width { get; private set; }
        public int Height { get; private set; }        
        public Pixel[] Pixels { get; set; }

        /// <summary>
        /// Constructor for a Blank Sprite
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Sprite(int width, int height)
        {
            Width = width;
            Height = height;

            Pixels = new Pixel[width * height];
            for (var i = 0; i < Pixels.Length; i++)
                Pixels[i] = new Pixel();
        }

        /// <summary>
        /// Constructor to make a Hard Copy of another Sprite
        /// </summary>
        /// <param name="sprite"></param>
        public Sprite(Sprite sprite)
        {
            if (sprite == null) return;

            Pixels = new Pixel[sprite.Width * sprite.Height];
            for (int i = 0; i < sprite.Pixels.Length; i++)
                Pixels[i] = sprite.Pixels[i];
        }

        /// <summary>
        /// Constructor for a Sprite that will
        /// be retrieved from a file on the computer
        /// </summary>
        /// <param name="path">The Path on the Computer</param>
        public Sprite(string path)
        {
            try
            {
                var image = System.Drawing.Image.FromFile(path);
                var b = new System.Drawing.Bitmap(image);

                Width = b.Width;
                Height = b.Height;

                Pixels = new Pixel[Width * Height];

                for (var x = 0; x < b.Width; x++)
                {
                    for (var y = 0; y < b.Height; y++)
                    {
                        var p = b.GetPixel(x, y);
                        var col = new Pixel(p.R, p.G, p.B, p.A);
                        SetPixel(x, y, col);
                    }
                }

                image.Dispose();
                b.Dispose();
            }
            catch(FileNotFoundException) {
                Console.WriteLine($"[ERROR]: Image / File not found at\n \"{path}\"");
            }
        }

        /// <summary>
        /// The Whole Array of Pixels each converted
        /// to UInts
        /// </summary>
        /// <returns>A UInt Array</returns>
        public uint[] GetData()
        {
            uint[] data = new uint[Pixels.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = Pixels[i].ToInt();
            return data;
        }

        /// <summary>
        /// Retrieves a Pixel from the Sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>A Pixel</returns>
        public Pixel GetPixel(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return Pixels[y * Width + x];
            else
                return new Pixel();
        }

        /// <summary>
        /// Sets a Pixel in the Sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="color">Pixel Replacement</param>
        public void SetPixel(int x, int y, Pixel p)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Pixels[y * Width + x] = p;
        }

        /// <summary>
        /// Clears the Sprite to a Specific Color
        /// </summary>
        /// <param name="p">Pixel / Color</param>
        public void Clear(Pixel p)
        {
            for(int i = 0; i < Pixels.Length; i++)
                Pixels[i] = p;
        }
    }
}