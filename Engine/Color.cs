using System;

namespace Szark
{
    /// <summary>
    /// A struct containing RGBA information.
    /// </summary>
    public struct Color
    {
        public byte red, green, blue, alpha;

        /// <summary>
        /// The Constructor for a color in RGBA form.
        /// Alpha is not required and will just render
        /// completely opaque.
        /// </summary>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        /// <param name="alpha">Alpha</param>
        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        #region Constants

        // Greyscale Colors
        public static Color BLANK = new Color(0, 0, 0, 0);
        public static Color WHITE = new Color(255, 255, 255);
        public static Color GREY = new Color(192, 192, 192);
        public static Color BLACK = new Color(0, 0, 0);

        public static Color DARK_GREY = new Color(128, 128, 128);

        public static Color VERY_DARK_GREY = new Color(64, 64, 64);

        // RGB Colors
        public static Color RED = new Color(255, 0, 0);
        public static Color GREEN = new Color(0, 255, 0);
        public static Color BLUE = new Color(0, 0, 255);

        public static Color DARK_RED = new Color(128, 0, 0);
        public static Color DARK_GREEN = new Color(0, 128, 0);
        public static Color DARK_BLUE = new Color(0, 0, 128);

        public static Color VERY_DARK_RED = new Color(64, 0, 0);
        public static Color VERY_DARK_GREEN = new Color(0, 64, 0);
        public static Color VERY_DARK_BLUE = new Color(0, 0, 64);

        // CYM Colors
        public static Color YELLOW = new Color(255, 255, 0);
        public static Color MAGENTA = new Color(255, 0, 255);
        public static Color CYAN = new Color(0, 255, 255);

        public static Color DARK_YELLOW = new Color(128, 128, 0);
        public static Color DARK_MAGENTA = new Color(128, 0, 128);
        public static Color DARK_CYAN = new Color(0, 128, 128);

        public static Color VERY_DARK_YELLOW = new Color(64, 64, 0);
        public static Color VERY_DARK_MAGENTA = new Color(64, 0, 64);
        public static Color VERY_DARK_CYAN = new Color(0, 64, 64);

        #endregion

        /// <summary>
        /// Interpolates two colors
        /// </summary>
        /// <param name="first">First Color</param>
        /// <param name="second">Second Color</param>
        /// <param name="value">Mix Value (0-1)</param>
        /// <returns>The Blended Color</returns>
        public static Color Lerp(Color first, Color second, float value)
        {
            var red = (byte)((1 - value) * first.red + value * second.red);
            var green = (byte)((1 - value) * first.green + value * second.green);
            var blue = (byte)((1 - value) * first.blue + value * second.blue);
            var alpha = (byte)((1 - value) * first.alpha + value * second.alpha);

            return new Color(red, green, blue, alpha);
        }

        /// <summary>
        /// Compares this color to another
        /// </summary>
        /// <returns>If both colors are the same</returns>
        public bool Compare(Color other) =>
            other.red == red && other.green == green &&
                other.blue == blue && other.alpha == alpha;


        /// <summary>
        /// Creates a instance of Color from HSV color
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="S">Saturation</param>
        /// <param name="V">Value</param>
        /// <param name="alpha">Alpha</param>
        /// <returns></returns>
        public static Color FromHSV(double h, double S, double V, byte alpha = 255)
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

            return new Color(
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

        public static Color operator +(Color f, Color p) =>
            new Color((byte)(f.red + p.red), (byte)(f.green + p.green),
                (byte)(f.blue + p.blue), (byte)(f.alpha + p.alpha));

        public static Color operator -(Color f, Color p) =>
            new Color((byte)(f.red - p.red), (byte)(f.green - p.green),
                (byte)(f.blue - p.blue), (byte)(f.alpha - p.alpha));

        public static Color operator *(Color f, float t) =>
            new Color((byte)(f.red * t), (byte)(f.green * t),
                (byte)(f.blue * t), (byte)(f.alpha * t));
    }
}