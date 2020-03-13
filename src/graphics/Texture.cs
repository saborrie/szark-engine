using System.Drawing;
using System.Runtime.CompilerServices;
using System.IO;
using System;

namespace Szark
{
    /// <summary>
    /// Holds a representation of an Image
    /// </summary>
    public sealed class Texture
    {
        public int Width { get; private set; }
        public Color[] Pixels { get; internal set; }
        public int Height { get; private set; }

        /// <summary>
        /// Whether or not the texture can be written to.
        /// </summary>
        public bool IsLocked { get; private set; }

        public Color this[int x, int y]
        {
            get => IsInside(x, y) ? Pixels[y * Width + x] : Color.Black;
            set 
            { 
                if (!IsLocked && IsInside(x, y)) 
                    Pixels[y * Width + x] = value; 
            }
        }

        /// <summary>
        /// Creates a texture from a file at the specified path.
        /// Returns None if file is not found.
        /// </summary>
        public static Option<Texture> FromFile(string path)
        {
            try
            {
                using var image = Image.FromFile(path);
                using var bitmap = new Bitmap(image);
                Texture tex = Create(bitmap.Width, bitmap.Height);

                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        var p = bitmap.GetPixel(x, y);
                        tex[x, y] = new Color(p.R, p.G, p.B, p.A);
                    }
                }

                return Option<Texture>.Some(tex);
            }
            catch (FileNotFoundException)
            {
                Debug.Log($"Image not found at: [{path}]", LogLevel.ERROR);
                return Option<Texture>.None();
            }
        }

        /// <summary>
        /// Creates a texture with a desired width and height
        /// </summary>
        public static Texture Create(int width, int height)
        {
            return new Texture
            {
                Width = width,
                Pixels = new Color[width * height],
                Height = height
            };
        }

        /// <summary>
        /// Clears the texture to a solid color.
        /// Texture must be unlocked.
        /// </summary>
        public void ClearToColor(Color color)
        {
            if (IsLocked) return;
            for(int i = 0; i < Pixels.Length; i++)
                Pixels[i] = color;
        }

        /// <summary>
        /// Copies this texture's pixels to another.
        /// </summary>
        public void CopyTo(Texture other)
        {
            if (IsLocked && other == this) return;
            Array.Copy(Pixels, 0, other.Pixels, 0, 
                Math.Min(other.Pixels.Length, Pixels.Length));
        }

        /// <summary>
        /// Returns a sliced out chunk of this texture.
        /// If slice requested is out of bounds, then None is returned.
        /// </summary>
        public Option<Texture> Slice(int x0, int y0, int width, int height)
        {
            if (x0 >= 0 && (x0 + width) < Width && y0 >= 0 && (y0 + height) < Height)
            {
                Texture tex = Create(width, height);
                for (int x = x0; x < width; x++)
                    for (int y = y0; y < height; y++)
                        tex[x, y] = this[x, y];
                return Option<Texture>.Some(tex);
            }

            return Option<Texture>.None();
        }

        /// <summary>
        /// Loops over the texture with x and y index.
        /// Texture must be unlocked.
        /// </summary>
        public void ForEach(Func<int, int, Color> action)
        {
            if (IsLocked) return;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    this[x, y] = action(x, y);
        }

        /// <summary>
        /// Unlocks the texture to be written to.
        /// </summary>
        public void Unlock() => IsLocked = false;

        /// <summary>
        /// Locks the texture from being written to.
        /// </summary>
        public void Lock() => IsLocked = true;

        /// <summary>
        /// Creates a canvas from this texture
        /// </summary>
        public Canvas GetCanvas() => new Canvas() { Target = this };

        /// <summary>
        /// Checks if a Point is within the texture
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsInside(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;
    }
}