using PGE;
using System;

namespace PGEX.Affine
{
    public class Affine
    {
        public static void DrawSprite(PixelGameEngine e, int x, int y, Sprite sprite, double angle)
        {
            if (sprite == null) return;

            int hWidth = sprite.Width / 2;
            int hHeight = sprite.Height / 2;

            double sin = Math.Sin(-angle);
            double cos = Math.Cos(-angle);

            for (int i = 0; i < sprite.Width * 2f; i++)
            {
                for (int j = 0; j < sprite.Height * 2f; j++)
                {
                    int xt = i - sprite.Width;
                    int yt = j - sprite.Height;
                    
                    int xs = (int)Math.Round(cos * xt - sin * yt) + hWidth;
                    int ys = (int)Math.Round(sin * xt + cos * yt) + hHeight;

                    var pixel = sprite.GetPixel(xs, ys);

                    if (!pixel.Compare(new Pixel(0, 0, 0, 0)))
                        e.Draw((int)(x + i - sprite.Width), (int)(y + j - sprite.Height), pixel);
                }
            }
        }
    }
}   