using System;
using System.Drawing;
using System.IO;

namespace Szark
{
    /// <summary>
    /// A Class that contains an array of 
    /// s
    /// and may represent an image
    /// </summary>
    public class Sprite
    {
        public int Width { get; private set; }
        public int Height { get; private set; }        
        public Color[] Pixels { get; set; }

        /// <summary>
        /// Constructor for a blank sprite
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Sprite(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[width * height];
        }

        /// <summary>
        /// Constructor to make a hard copy of another sprite
        /// </summary>
        /// <param name="other"></param>
        public Sprite(Sprite other)
        {
            if (other == null) return;
            Pixels = new Color[other.Width * other.Height];
            for (int i = 0; i < other.Pixels.Length; i++)
                Pixels[i] = other.Pixels[i];
        }

        /// <summary>
        /// Constructor for a sprite that will
        /// be retrieved from a file on the computer
        /// </summary>
        /// <param name="path">The Path on the Computer</param>
        public Sprite(string path)
        {
            try
            {
                var image = Image.FromFile(path);
                var bitmap = new Bitmap(image);

                Width = bitmap.Width;
                Height = bitmap.Height;

                Pixels = new Color[Width * Height];

                for (var x = 0; x < bitmap.Width; x++)
                {
                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        var p = bitmap.GetPixel(x, y);
                        var col = new Color(p.R, p.G, p.B, p.A);
                        SetPixel(x, y, col);
                    }
                }

                image.Dispose();
                bitmap.Dispose();
            }
            catch(FileNotFoundException) {
                Debug.Log($"Image / File not found at\n \"{path}\"", LogLevel.ERROR);
            }
        }

        /// <summary>
        /// Retrieves a pixel from the sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>A Pixel</returns>
        public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return Pixels[y * Width + x];
            else return new Color();
        }

        /// <summary>
        /// Sets a pixel in the sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="color">Pixel Replacement</param>
        public void SetPixel(int x, int y, Color p)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Pixels[y * Width + x] = p;
        }

        /// <summary>
        /// Clears the sprite to a specific color
        /// </summary>
        /// <param name="p">Pixel / Color</param>
        public void Clear(Color p)
        {
            for(int i = 0; i < Pixels.Length; i++)
                Pixels[i] = p;
        }

        /// <summary>
        /// Copies all pixels into another sprite
        /// (Both sprites must be the same size!)
        /// </summary>
        /// <param name="destination">The Sprite to Copy To</param>
        public void CopyTo(Sprite destination)
        {
            if (destination == null) 
                throw new NullReferenceException();

            if (destination.Width != Width || destination.Height != Height)
                throw new Exception("Destination sprite size is not the same" +
                    "as the source sprite!");

            Pixels.CopyTo(destination.Pixels, 0);
        }
    }
}