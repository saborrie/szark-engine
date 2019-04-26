using System.Drawing;

namespace Szark
{
    public sealed class Text
    {
        public float X { get; set; }
        public float Y { get; set; }

        public string FontFamily { get; private set; }
        public float FontSize { get; private set; }

        public Sprite Sprite { get; set; }

        public Text(string text, string fontFamily, float fontSize) =>
            Set(text, fontFamily, fontSize);

        public void Set(string text, string fontFamily, float fontSize)
        {
            if (text == "") return;

            FontFamily = fontFamily;
            FontSize = fontSize;

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
            Sprite = new Sprite(charBitmap.Width, charBitmap.Height);

            graphics.Flush();

            for (int i = 0; i < Sprite.Width; i++)
            {
                for (int j = 0; j < Sprite.Height; j++)
                {
                    var pixel = charBitmap.GetPixel(i, j);
                    Sprite.SetPixel(i, j, new Color(pixel.R, pixel.G, pixel.B, pixel.A));
                }
            }

            Sprite.Refresh();
        }

        public void Render() => 
            Sprite?.Render(X, Y);
    }
}
