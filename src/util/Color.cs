using System;

namespace Szark
{
    public partial struct Color
    {
        public byte r, g, b, a;

        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }

        // -- Methods --

        public Color Saturate()
        {
            return new Color
            {
                r = (byte)Mathf.Clamp(r, 0, 255),
                g = (byte)Mathf.Clamp(g, 0, 255),
                b = (byte)Mathf.Clamp(b, 0, 255),
                a = (byte)Mathf.Clamp(a, 0, 255),
            };
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

        // -- Object Overrides --

        public override bool Equals(object obj)
        {
            return obj is Color color && r == color.r &&
                g == color.g && b == color.b &&
                   a == color.a;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = (hashCode * 397) ^ r;
            hashCode = (hashCode * 397) ^ g;
            hashCode = (hashCode * 397) ^ b;
            hashCode = (hashCode * 397) ^ a;
            return hashCode;
        }

        public override string ToString() =>
            $"({r},{g},{b},{a})";

        // -- Arithmetic Operators --

        public static Color operator +(Color f, Color p) =>
            new Color((byte)(f.r + p.r), (byte)(f.g + p.g),
                (byte)(f.b + p.b), (byte)(f.a + p.a));

        public static Color operator -(Color f, Color p) =>
            new Color((byte)(f.r - p.r), (byte)(f.g - p.g),
                (byte)(f.b - p.b), (byte)(f.a - p.a));

        public static Color operator *(Color f, float t) =>
            new Color((byte)(f.r * t), (byte)(f.g * t),
                (byte)(f.b * t), (byte)(f.a * t));

        // -- Equality Operators --

        public static bool operator ==(Color a, Color b) =>
            a.r == b.r && a.g == b.g && 
                a.b == b.b && a.a == b.a;

        public static bool operator !=(Color a, Color b) =>
            a.r != b.r || a.g != b.g || 
                a.b != b.b || a.a != b.a;
    }
}