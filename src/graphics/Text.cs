﻿using System.Drawing;

namespace Szark
{
    public static class Text
    {
        public static Texture Get(string text, 
            string fontFamily = "Arial", float fontSize = 16)
        {
            Bitmap charBitmap = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(charBitmap);

            Font font = new Font(fontFamily, fontSize);
            SizeF inputSize = graphics.MeasureString(text, font);
            charBitmap = new Bitmap((int)inputSize.Width, (int)inputSize.Height);
            charBitmap.MakeTransparent();

            graphics = Graphics.FromImage(charBitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            graphics.DrawString(text, font, Brushes.White, 0, 0);
            var result = new Texture(charBitmap.Width, charBitmap.Height);
            graphics.Flush();

            for (int i = 0; i < result.width; i++)
            {
                for (int j = 0; j < result.height; j++)
                {
                    var pixel = charBitmap.GetPixel(i, j);
                    result.Set(i, j, new Color(pixel.R, pixel.G, pixel.B, pixel.A));
                }
            }

            return result;
        }
    }
}
