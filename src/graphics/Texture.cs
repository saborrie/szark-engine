using System.Drawing;
using System.IO;

namespace Szark
{
    public sealed class Texture
    {
        public readonly int width, height;        
        public Color[] Pixels { get; set; }

        public Texture(int width, int height)
        {
            this.width = width;
            this.height = height;
            Pixels = new Color[width * height];
        }

        public Texture(string path)
        {
            try
            {
                var image = Image.FromFile(path);
                var bitmap = new Bitmap(image);

                width = bitmap.Width;
                height = bitmap.Height;

                Pixels = new Color[width * height];

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
            if (x >= 0 && x < width && y >= 0 && y < height)
                return Pixels[y * width + x];
            else return new Color();
        }

        /// <summary>
        /// Sets a pixel in the texture
        /// </summary>
        public void Set(int x, int y, Color color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                Pixels[y * width + x] = color;
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