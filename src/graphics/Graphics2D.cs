using System;

namespace Szark
{
    public sealed class Graphics2D
    {
        public Texture Target { get; set; }

        public Graphics2D(int width, int height) =>
            Target = new Texture(width, height);

        public Graphics2D(Texture tex) =>
            Target = tex;

        public void Draw(int x, int y, Color color) =>
            Target.Set(x, y, color);

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
                    for (int j = 1; j < thickness; j++)
                    {
                        Draw((int)x + j, (int)y, color);
                        Draw((int)x, (int)y + j, color);
                    }
                }

                x += dx;
                y += dy;
            }
        }

        public void DrawRectangle(int x, int y, int w, int h, Color color)
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

        public void FillRectangle(int x, int y, int w, int h, Color color)
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    Draw(x + i, y + j, color);
        }

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

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
            DrawLine(x1, y1, x3, y3, color);
        }

        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            float Sign(int aX, int aY, int bX, int bY, int cX, int cY) =>
                (aX - x3) * (bY - cY) - (bX - cX) * (aY - cY);

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
                        Draw(x, y, color);
                }
            }
        }

        public void DrawTexture(int x, int y, Texture sprite, int scale = 1)
        {
            if (scale <= 0) return;
            for (int i = 0; i < sprite.width; i++)
                for (int j = 0; j < sprite.height; j++)
                    FillRectangle((x + i) * scale, (y + j) * scale,
                        scale, scale, sprite.Get(i, j));
        }
    }
}