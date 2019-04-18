using System.Drawing;

namespace Szark
{
    public sealed class Text
    {
        public float X { get; set; }
        public float Y { get; set; }

        public string FontFamily { get; private set; }
        public float FontSize { get; private set; }

        private readonly Sprite[] characters;
        private const string charSheet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-=_+[]{}\\|;:'\".,<>/?`~ ";

        public Text(string fontFamily, float fontSize)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;

            characters = new Sprite[charSheet.Length];
            int index = 0;

            foreach (var c in charSheet)
            {
                Bitmap charBitmap = new Bitmap(1, 1);
                Graphics graphics = Graphics.FromImage(charBitmap);

                Font font = new Font(fontFamily, fontSize);
                SizeF inputSize = graphics.MeasureString(c.ToString(), font);
                charBitmap = new Bitmap((int)inputSize.Width, (int)inputSize.Height);
                charBitmap.MakeTransparent();

                graphics = Graphics.FromImage(charBitmap);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                graphics.DrawString(c.ToString(), font, Brushes.White, 0, 0);
                characters[index] = new Sprite(charBitmap.Width, charBitmap.Height);

                graphics.Flush();

                for (int i = 0; i < characters[index].Width; i++)
                {
                    for (int j = 0; j < characters[index].Height; j++)
                    {
                        var pixel = charBitmap.GetPixel(i, j);
                        characters[index].SetPixel(i, j, new Color(pixel.R, pixel.G, pixel.B, pixel.A));
                    }
                }

                characters[index].Refresh();

                index++;
            }
        }

        public float DrawString(string text, float x, float y, int spacing = -8, float scale = 1, int layer = 1)
        {
            X = x;
            Y = y;
            return DrawString(text, spacing, scale, layer);
        }

        public float DrawString(string text, int spacing = -8, float scale = 1, int layer = 1)
        {
            float space = 0;

            for (int i = 0; i < text.Length; i++)
            {
                int charIndex = charSheet.IndexOf(text[i]);
                characters[charIndex].Render(X + space, Y, 0, scale, layer);
                space += characters[charIndex].Width + spacing;
                if (text[i] == ' ') space += FontSize + spacing;
            }

            return space;
        }

        public void SetShader(Shader shader)
        {
            for (int i = 0; i < characters.Length; i++)
                characters[i].Shader = shader;
        }
    }
}
