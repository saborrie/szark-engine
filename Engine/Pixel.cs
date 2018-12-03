/*
	Pixel.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

namespace PGE
{
    /// <summary>
    /// A struct containing RGBA information for a pixel.
    /// This also can be used for representing color
    /// </summary>
    public struct Pixel
    {
        public byte r, g, b, a;

        /// <summary>
        /// The Constructor for a Pixel in RGBA Form.
        /// Alpha is not required and will just render
        /// completely opaque.
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <param name="a">Alpha</param>
        public Pixel(byte r, byte g, byte b, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        // Greyscale Colors
        public static Pixel BLANK = new Pixel(0, 0, 0, 0);
        public static Pixel WHITE = new Pixel(255, 255, 255);
        public static Pixel GREY = new Pixel(192, 192, 192);
        public static Pixel BLACK = new Pixel(0, 0, 0);

        public static Pixel DARK_GREY = new Pixel(128, 128, 128);

        public static Pixel VERY_DARK_GREY = new Pixel(64, 64, 64);

        // RGB Colors
        public static Pixel RED = new Pixel(255, 0, 0);
        public static Pixel GREEN = new Pixel(0, 255, 0);
        public static Pixel BLUE = new Pixel(0, 0, 255);

        public static Pixel DARK_RED = new Pixel(128, 0, 0);
        public static Pixel DARK_GREEN = new Pixel(0, 128, 0);
        public static Pixel DARK_BLUE = new Pixel(0, 0, 128);

        public static Pixel VERY_DARK_RED = new Pixel(64, 0, 0);
        public static Pixel VERY_DARK_GREEN = new Pixel(0, 64, 0);
        public static Pixel VERY_DARK_BLUE = new Pixel(0, 0, 64);

        // CYM Colors
        public static Pixel YELLOW = new Pixel(255, 255, 0);
        public static Pixel MAGENTA = new Pixel(255, 0, 255);
        public static Pixel CYAN = new Pixel(0, 255, 255);

        public static Pixel DARK_YELLOW = new Pixel(128, 128, 0);
        public static Pixel DARK_MAGENTA = new Pixel(128, 0, 128);
        public static Pixel DARK_CYAN = new Pixel(0, 128, 128);

        public static Pixel VERY_DARK_YELLOW = new Pixel(64, 64, 0);
        public static Pixel VERY_DARK_MAGENTA = new Pixel(64, 0, 64);
        public static Pixel VERY_DARK_CYAN = new Pixel(0, 64, 64);

        /// <summary>
        /// Interpolates Two Pixels based on T between 0 to 1
        /// </summary>
        /// <param name="a">First Pixel</param>
        /// <param name="b">Second Pixel</param>
        /// <param name="t">Alpha</param>
        /// <returns>The Blended Pixel</returns>
        public static Pixel Lerp(Pixel a, Pixel b, float t) =>
            new Pixel((byte)((1 - t) * a.r + t * b.r), (byte)((1 - t) * a.g + t * b.g),
                (byte)((1 - t) * a.b + t * b.b), (byte)((1 - t) * a.a + t * b.a));

        /// <summary>
        /// Converts a Pixel in a UInt
        /// </summary>
        /// <returns>A UInt that represents a Pixel</returns>
        public uint ToInt() =>
            (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));

        /// <summary>
        /// Converts a UInt to a Pixel
        /// </summary>
        /// <param name="i">UInt</param>
        /// <returns>Pixel</returns>
        public static Pixel ToPixel(uint i) =>
            new Pixel((byte)(i >> 0), (byte)(i >> 8), 
                (byte)(i >> 16), (byte)(i >> 24));

        public static Pixel operator+(Pixel f, Pixel p) =>
            new Pixel((byte)(f.r + p.r), (byte)(f.g + p.g), (byte)(f.b + p.b), (byte)(f.a + p.a));

        public static Pixel operator-(Pixel f, Pixel p) =>
            new Pixel((byte)(f.r - p.r), (byte)(f.g - p.g), (byte)(f.b - p.b), (byte)(f.a - p.a));

        public static Pixel operator*(Pixel f, float t) =>
            new Pixel((byte)(f.r * t), (byte)(f.g * t), (byte)(f.b * t), (byte)(f.a * t));

        /// <summary>
        /// Compares this pixel's color to another
        /// </summary>
        /// <param name="other">The other pixel</param>
        /// <returns>If both colors are the same</returns>
        public bool Compare(Pixel other) =>
            other.r == r && other.g == g && other.b == b && other.a == a;
    }
}