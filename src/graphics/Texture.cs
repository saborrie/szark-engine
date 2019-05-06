using System.Drawing;
using System.IO;

namespace Szark
{
    public sealed class Texture
    {
        public int Width { get; private set; }
        public int Height { get; private set; }        
        public Color[] Pixels { get; set; }

        public Texture(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[width * height];
        }

        public Texture(string path)
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
                        Set(x, y, col);
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
        /// Retrieves a pixel from the texture
        /// </summary>
        public Color Get(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return Pixels[y * Width + x];
            else return new Color();
        }

        /// <summary>
        /// Sets a pixel in the texture
        /// </summary>
        public void Set(int x, int y, Color color)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Pixels[y * Width + x] = color;
        }

        /// <summary>
        /// Makes the texture to a solid color
        /// </summary>
        public void Fill(Color color)
        {
            for(int i = 0; i < Pixels.Length; i++)
                Pixels[i] = color;
        }
    }
}