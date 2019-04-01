using System;

namespace Szark
{
    public enum OpacityMode
    {
        NORMAL,
        MASK,
        ALPHA
    }

    public sealed class Graphics2D
    {
        /// <summary>
        /// The Alpha 'Blending' Modes
        /// Normal - Alpha has no Affect
        /// Mask - Any Alpha below 255 doesn't get rendered
        /// Alpha - 'Proper' Alpha Blending
        /// </summary>
        public OpacityMode OpacityMode { get; set; } = OpacityMode.ALPHA;

        /// <summary>
        /// The current frame or sprite that is being drawn on
        /// </summary>
        public Sprite DrawTarget { get; set; }

        private Sprite fontSprite;

        public Graphics2D(int width, int height)
        {
            DrawTarget = new Sprite(width, height);
            ConstructFontSheet();
        }

        public Graphics2D(Sprite sprite)
        {
            DrawTarget = sprite;
            ConstructFontSheet();
        }

        /// <summary>
        /// Draws a pixel on the screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="color">Color</param>
        public void Draw(int x, int y, Color color)
        {
            if (color.alpha < 255)
            {
                if (OpacityMode == OpacityMode.MASK)
                    return;

                if (OpacityMode == OpacityMode.ALPHA)
                {
                    var l = Color.Lerp(DrawTarget.GetPixel(x, y), color, color.alpha / 255f);
                    DrawTarget.SetPixel(x, y, new Color(l.red, l.green, l.blue));
                    return;
                }
            }

            DrawTarget?.SetPixel(x, y, color);
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="color">Color</param>
        /// <param name="thickness">Thickness</param>
        public void DrawLine(int x1, int y1, int x2, int y2, Color color, int thickness = 1)
        {
            float x, y, step;
            float dx = x2 - x1;
            float dy = y2 - y1;

            float absDX = Math.Abs(dx);
            float absDY = Math.Abs(dy);

            step = absDX >= absDY ? absDX : absDY;

            dx /= step;
            dy /= step;

            x = x1;
            y = y1;

            for (int i = 1; i <= step; i++)
            {
                Draw((int)x, (int)y, color);

                if (thickness > 1)
                {
                    for(int j = 1; j < thickness; j++)
                    {
                        Draw((int)x + j, (int)y, color);
                        Draw((int)x, (int)y + j, color);
                    }
                }

                x += dx;
                y += dy;
            }
        }

        /// <summary>
        /// Draws a rectangle outline
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">Color</param>
        public void DrawRect(int x, int y, int w, int h, Color color)
        {
            if (w < 0)
            {
                w *= -1;
                x -= w;
            }

            DrawLine(x, y, x + w, y, color);
            DrawLine(x + w - 1, y, x + w - 1, y + h, color);
            DrawLine(x, y + h - 1, x + w, y + h - 1, color);
            DrawLine(x, y, x, y + h, color);
        }

        /// <summary>
        /// Draws a filled In rectangle
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">Color</param>
        public void FillRect(int x, int y, int w, int h, Color color)
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    Draw(x + i, y + j, color);
        }

        /// <summary>
        /// Draws a circle outline
        /// </summary>
        /// <param name="x0">X</param>
        /// <param name="y0">Y</param>
        /// <param name="r">Radius</param>
        /// <param name="color">Color</param>
        public void DrawCircle(int x0, int y0, int r, Color color)
        {
            x0 += r - 1;
            y0 += r - 1;

            int x = r - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (r << 1);

            while (x >= y)
            {
                Draw(x0 + x, y0 + y, color);
                Draw(x0 + y, y0 + x, color);
                Draw(x0 - y, y0 + x, color);
                Draw(x0 - x, y0 + y, color);
                Draw(x0 - x, y0 - y, color);
                Draw(x0 - y, y0 - x, color);
                Draw(x0 + y, y0 - x, color);
                Draw(x0 + x, y0 - y, color);

                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }

                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (r << 1);
                }
            }
        }

        /// <summary>
        /// Draws a filled in circle
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="r">Radius</param>
        /// <param name="color">Color</param>
        public void FillCircle(int x, int y, int r, Color color)
        {
            for (int i = 0; i < r * 2; i++)
            {
                for (int j = 0; j < r * 2; j++)
                {
                    var dist = Math.Sqrt((r - i) * (r - i) + (r - j) * (r - j));
                    if (dist < r) Draw(x - 1 + i, y - 1 + j, color);
                }
            }
        }

        /// <summary>
        /// Draw a triangle outline
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="x3">X3</param>
        /// <param name="y3">Y3</param>
        /// <param name="color">Color</param>
        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
            DrawLine(x1, y1, x3, y3, color);
        }

        /// <summary>
        /// Draws a filled in triangle
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="x3">X3</param>
        /// <param name="y3">Y3</param>
        /// <param name="color">Color</param>
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            var minX = Math.Min(Math.Min(x1, x2), x3);
            var maxX = Math.Max(Math.Max(x1, x2), x3);

            var minY = Math.Min(Math.Min(y1, y2), y3);
            var maxY = Math.Max(Math.Max(y1, y2), y3);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float d1, d2, d3;
                    bool hasNeg, hasPos;

                    d1 = Sign(x, y, x1, y1, x2, y2);
                    d2 = Sign(x, y, x2, y2, x3, y3);
                    d3 = Sign(x, y, x3, y3, x1, y1);

                    hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
                    hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

                    if (!(hasNeg && hasPos))
                    {
                        Draw(x, y, color);
                    }
                }
            }
        }

        private float Sign(int x1, int y1, int x2, int y2, int x3, int y3) =>
            (x1 - x3) * (y2 - y3) - (x2 - x3) * (y1 - y3);

        /// <summary>
        /// Draws a sprite on the screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="scale">Scale</param>
        /// <param name="sprite">The Sprite</param>
        public void DrawSprite(int x, int y, Sprite sprite, int scale = 1)
        {
            if (sprite == null || scale <= 0) return;
            for (int i = 0; i < sprite.Width; i++)
                for (int j = 0; j < sprite.Height; j++)
                    FillRect((x + i) * scale, (y + j) * scale, 
                        scale, scale, sprite.GetPixel(i, j));
        }

        /// <summary>
        /// Draws a sprite on the screen with rotation
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="sprite">The Sprite</param>
        /// <param name="angle">Angle</param>
        public void DrawSprite(int x, int y, Sprite sprite, double angle)
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

                    if (!pixel.Compare(new Color(0, 0, 0, 0)))
                        Draw((int)(x + i - sprite.Width), (int)(y + j - sprite.Height), pixel);
                }
            }
        }

        /// <summary>
        /// Partially draws a sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sprite">The Sprite</param>
        /// <param name="ox">Offset X</param>
        /// <param name="oy">Offset Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public void DrawPartialSprite(int x, int y, Sprite sprite, int ox, int oy, int w, int h)
        {
            if (sprite == null) return;
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    Draw(x + i, y + j, sprite.GetPixel(i + ox, j + oy));
        }

        /// <summary>
        /// Draws a string using embeded character data.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="text">Text</param>
        /// <param name="color">Color</param>
        /// <param name="scale">Scale</param>
        public void DrawString(int x, int y, string text, Color color, int scale = 1)
        {
            int sx = 0;
            int sy = 0;
            foreach (var c in text)
            {
                if (c == '\n')
                {
                    sx = 0; sy += 8 * scale;
                }
                else
                {
                    int ox = (c - 32) % 16;
                    int oy = (c - 32) / 16;

                    if (scale > 1)
                    {
                        for (int i = 0; i < 8; i++)
                            for (int j = 0; j < 8; j++)
                                if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).red > 0)
                                    for (int s = 0; s < scale; s++)
                                        for (int js = 0; js < scale; js++)
                                            Draw(x + sx + (i*scale) + s, y + sy + (j*scale) + js, color);
                    }
                    else
                    {
                        for (int i = 0; i < 8; i++)
                            for (int j = 0; j < 8; j++)
                                if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).red > 0)								
                                    Draw(x + sx + i, y + sy + j, color);
                    }	

                    sx += 8 * scale;
                }
            }
        }

        private void ConstructFontSheet()
        {
            string data = "";
            data += "?Q`0001oOch0o01o@F40o0<AGD4090LAGD<090@A7ch0?00O7Q`0600>00000000";
            data += "O000000nOT0063Qo4d8>?7a14Gno94AA4gno94AaOT0>o3`oO400o7QN00000400";
            data += "Of80001oOg<7O7moBGT7O7lABET024@aBEd714AiOdl717a_=TH013Q>00000000";
            data += "720D000V?V5oB3Q_HdUoE7a9@DdDE4A9@DmoE4A;Hg]oM4Aj8S4D84@`00000000";
            data += "OaPT1000Oa`^13P1@AI[?g`1@A=[OdAoHgljA4Ao?WlBA7l1710007l100000000";
            data += "ObM6000oOfMV?3QoBDD`O7a0BDDH@5A0BDD<@5A0BGeVO5ao@CQR?5Po00000000";
            data += "Oc``000?Ogij70PO2D]??0Ph2DUM@7i`2DTg@7lh2GUj?0TO0C1870T?00000000";
            data += "70<4001o?P<7?1QoHg43O;`h@GT0@:@LB@d0>:@hN@L0@?aoN@<0O7ao0000?000";
            data += "OcH0001SOglLA7mg24TnK7ln24US>0PL24U140PnOgl0>7QgOcH0K71S0000A000";
            data += "00H00000@Dm1S007@DUSg00?OdTnH7YhOfTL<7Yh@Cl0700?@Ah0300700000000";
            data += "<008001QL00ZA41a@6HnI<1i@FHLM81M@@0LG81?O`0nC?Y7?`0ZA7Y300080000";
            data += "O`082000Oh0827mo6>Hn?Wmo?6HnMb11MP08@C11H`08@FP0@@0004@000000000";
            data += "00P00001Oab00003OcKP0006@6=PMgl<@440MglH@000000`@000001P00000000";
            data += "Ob@8@@00Ob@8@Ga13R@8Mga172@8?PAo3R@827QoOb@820@0O`0007`0000007P0";
            data += "O`000P08Od400g`<3V=P0G`673IP0`@3>1`00P@6O`P00g`<O`000GP800000000";
            data += "?P9PL020O`<`N3R0@E4HC7b0@ET<ATB0@@l6C4B0O`H3N7b0?P01L3R000000020";

            fontSprite = new Sprite(128, 48);
            int px = 0, py = 0;
            for (int b = 0; b < 1024; b += 4)
            {
                uint sym1 = (uint)data[b + 0] - 48;
                uint sym2 = (uint)data[b + 1] - 48;
                uint sym3 = (uint)data[b + 2] - 48;
                uint sym4 = (uint)data[b + 3] - 48;
                uint r = sym1 << 18 | sym2 << 12 | sym3 << 6 | sym4;

                for (int i = 0; i < 24; i++)
                {
                    byte k = (byte)((r & (1 << i)) > 0 ? 255 : 0);
                    fontSprite.SetPixel(px, py, new Color(k, k, k, k));
                    if (++py == 48) { px++; py = 0; }
                }
            }
        }
    }
}