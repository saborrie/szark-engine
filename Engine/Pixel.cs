using System;

namespace Szark
{
    /// <summary>
    /// A struct containing RGBA information for a pixel.
    /// This also can be used for representing color
    /// </summary>
    public struct Pixel
    {
        public byte red, green, blue, alpha;

        /// <summary>
        /// The Constructor for a pixel in RGBA form.
        /// Alpha is not required and will just render
        /// completely opaque.
        /// </summary>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        /// <param name="alpha">Alpha</param>
        public Pixel(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        #region Constants

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

        #endregion

        /// <summary>
        /// Interpolates two pixels
        /// </summary>
        /// <param name="first">First Pixel</param>
        /// <param name="second">Second Pixel</param>
        /// <param name="value">Mix Value (0-1)</param>
        /// <returns>The Blended Pixel</returns>
        public static Pixel Lerp(Pixel first, Pixel second, float value)
        {
            var red = (byte)((1 - value) * first.red + value * second.red);
            var green = (byte)((1 - value) * first.green + value * second.green);
            var blue = (byte)((1 - value) * first.blue + value * second.blue);
            var alpha = (byte)((1 - value) * first.alpha + value * second.alpha);

            return new Pixel(red, green, blue, alpha);
        }

        /// <summary>
        /// Compares this pixel's color to another
        /// </summary>
        /// <param name="other">The other pixel</param>
        /// <returns>If both colors are the same</returns>
        public bool Compare(Pixel other) =>
            other.red == red && other.green == green &&
                other.blue == blue && other.alpha == alpha;


        /// <summary>
        /// Creates a instance of Pixel from HSV color
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="S">Saturation</param>
        /// <param name="V">Value</param>
        /// <param name="alpha">Alpha</param>
        /// <returns></returns>
        public static Pixel FromHSV(double h, double S, double V, byte alpha = 255)
        {
            double H = h;
            double R, G, B;

            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
        
            if (V <= 0) { R = G = B = 0; }
            else if (S <= 0) { R = G = B = V; }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {
                    case 0: R = V;  G = tv; B = pv; break;
                    case 1: R = qv; G = V;  B = pv; break;
                    case 2: R = pv; G = V;  B = tv; break;
                    case 3: R = pv; G = qv; B = V; break;
                    case 4: R = tv; G = pv; B = V; break;
                    case 5: R = V;  G = pv; B = qv; break;
                    case 6: R = V;  G = tv; B = pv; break;
                    case -1: R = V; G = pv; B = qv; break;
                    default: R = G = B = V; break;
                }
            }

            return new Pixel(
                Clamp((byte)(R * 255.0)), 
                Clamp((byte)(G * 255.0)), 
                Clamp((byte)(B * 255.0)), 
                alpha);
        }


        private static byte Clamp(byte i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        // -- Operators --

        public static Pixel operator +(Pixel f, Pixel p) =>
            new Pixel((byte)(f.red + p.red), (byte)(f.green + p.green),
                (byte)(f.blue + p.blue), (byte)(f.alpha + p.alpha));

        public static Pixel operator -(Pixel f, Pixel p) =>
            new Pixel((byte)(f.red - p.red), (byte)(f.green - p.green),
                (byte)(f.blue - p.blue), (byte)(f.alpha - p.alpha));

        public static Pixel operator *(Pixel f, float t) =>
            new Pixel((byte)(f.red * t), (byte)(f.green * t),
                (byte)(f.blue * t), (byte)(f.alpha * t));
    }
}