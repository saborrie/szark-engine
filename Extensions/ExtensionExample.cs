using System;

namespace olc
{
    class ExtensionExample : PGEX 
    {
        // Example Static Method
        public static Pixel AvgPixels(Pixel a, Pixel b)
        {
            return new Pixel((byte)((a.r + b.r) / 2), (byte)((a.g + b.g) / 2), 
                (byte)((a.b + b.b) / 2), (byte)((a.a + b.a) / 2));
        }
    }
}
