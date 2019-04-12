using System.Drawing;
using System.IO;

using OpenTK.Graphics.OpenGL4;

namespace Szark
{
    /// <summary>
    /// A Class that contains an array of 
    /// s
    /// and may represent an image
    /// </summary>
    public struct Sprite
    {
        public int ID { get; private set; }
        public Shader Shader { get; set; }

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

            ID = GL.GenTexture();
            Shader = Shader.Default;
            CreateTexture();
        }

        /// <summary>
        /// Constructor to make a hard copy of another sprite
        /// </summary>
        /// <param name="other"></param>
        public Sprite(Sprite other)
        {
            Width = other.Width;
            Height = other.Height;

            Pixels = new Color[other.Width * other.Height];
            for (int i = 0; i < other.Pixels.Length; i++)
                Pixels[i] = other.Pixels[i];

            ID = GL.GenTexture();
            Shader = Shader.Default;
            CreateTexture();
        }

        /// <summary>
        /// Constructor for a sprite that will
        /// be retrieved from a file on the computer
        /// </summary>
        /// <param name="path">The Path on the Computer</param>
        public Sprite(string path)
        {
            Width = 1;
            Height = 1;
            Pixels = new Color[1];

            ID = GL.GenTexture();
            Shader = Shader.Default;

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

            CreateTexture();
        }

        // Creates the Texture for the GPU
        private void CreateTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                    Pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);
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
        public void ClearToColor(Color p)
        {
            for(int i = 0; i < Pixels.Length; i++)
                Pixels[i] = p;
            Refresh();
        }

        /// <summary>
        /// Will refresh the texture if edited by the CPU
        /// </summary>
        public void Refresh()
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height,
                PixelFormat.Rgba, PixelType.UnsignedByte, Pixels);
        }
    }
}