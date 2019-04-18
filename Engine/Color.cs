using System;

namespace Szark
{
    public struct Color
    {
        public byte red, green, blue, alpha;

        /// <summary>
        /// The Constructor for a color in RGBA form.
        /// Alpha is not required and will just render
        /// completely opaque.
        /// </summary>
        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        // -- Constants --

        public readonly static Color Clear = new Color(0, 0, 0, 0);
        public readonly static Color White = new Color(255, 255, 255);
        public readonly static Color Grey = new Color(192, 192, 192);
        public readonly static Color Black = new Color(0, 0, 0);

        public readonly static Color Red = new Color(255, 0, 0);
        public readonly static Color Green = new Color(0, 255, 0);
        public readonly static Color Blue = new Color(0, 0, 255);

        public readonly static Color Yellow = new Color(255, 255, 0);
        public readonly static Color Magenta = new Color(255, 0, 255);
        public readonly static Color Cyan = new Color(0, 255, 255);

        // -- Methods --

        /// <summary>
        /// Interpolates between two colors
        /// </summary>
        public static Color Lerp(Color first, Color second, float value)
        {
            var red = (byte)((1 - value) * first.red + value * second.red);
            var green = (byte)((1 - value) * first.green + value * second.green);
            var blue = (byte)((1 - value) * first.blue + value * second.blue);
            var alpha = (byte)((1 - value) * first.alpha + value * second.alpha);

            return new Color(red, green, blue, alpha);
        }

        /// <summary>
        /// Creates a Color from HSV
        /// </summary>
        /// <param name="H">Hue</param>
        /// <param name="S">Saturation</param>
        /// <param name="V">Value</param>
        /// <param name="alpha">Alpha</param>
        public static Color FromHSV(float H, float S, float V, byte alpha = 255)
        {
            float R, G, B;
            float hf = (H %= 360) / 60.0f;
            int i = (int)Math.Floor(hf);

            float pv = V * (1 - S);
            float qv = V * (1 - S * (hf - i));
            float tv = V * (1 - S * (1 - (hf - i)));

            switch (i)
            {
                case 0:  R = V;  G = tv; B = pv; break;
                case 1:  R = qv; G = V;  B = pv; break;
                case 2:  R = pv; G = V;  B = tv; break;
                case 3:  R = pv; G = qv; B = V;  break;
                case 4:  R = tv; G = pv; B = V;  break;
                default: R = V;  G = pv; B = qv; break;
            }

            return new Color((byte)(R * 255.0), 
                (byte)(G * 255.0), (byte)(B * 255.0), alpha);
        }

        public override bool Equals(object obj)
        {
            return obj is Color color && red == color.red &&
                green == color.green && blue == color.blue &&
                   alpha == color.alpha;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = (hashCode * 397) ^ red;
            hashCode = (hashCode * 397) ^ green;
            hashCode = (hashCode * 397) ^ blue;
            hashCode = (hashCode * 397) ^ alpha;
            return hashCode;
        }

        // -- Arithmetic Operators --

        public static Color operator +(Color f, Color p) =>
            new Color((byte)(f.red + p.red), (byte)(f.green + p.green),
                (byte)(f.blue + p.blue), (byte)(f.alpha + p.alpha));

        public static Color operator -(Color f, Color p) =>
            new Color((byte)(f.red - p.red), (byte)(f.green - p.green),
                (byte)(f.blue - p.blue), (byte)(f.alpha - p.alpha));

        public static Color operator *(Color f, float t) =>
            new Color((byte)(f.red * t), (byte)(f.green * t),
                (byte)(f.blue * t), (byte)(f.alpha * t));

        // -- Equality Operators --

        public static bool operator ==(Color a, Color b) =>
            a.red == b.red && a.green == b.green && 
                a.blue == b.blue && a.alpha == b.alpha;

        public static bool operator !=(Color a, Color b) =>
            a.red != b.red || a.green != b.green || 
                a.blue != b.blue || a.alpha != b.alpha;
    }
}