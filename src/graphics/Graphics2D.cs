using System;

namespace Szark
{
    public sealed class Graphics2D
    {
        public readonly Texture target;

        public Graphics2D(int width, int height) =>
            target = new Texture(width, height);

        public Graphics2D(Texture tex) =>
            target = tex;

        public void Draw(int x, int y, Color color) =>
            target.Set(x, y, color);

        public void Draw(Vector point, Color color) =>
            Draw((int)point.x, (int)point.y, color);

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

        public void DrawLine(Vector pointA, Vector pointB, Color color, int thickness = 1) =>
            DrawLine((int)pointA.x, (int)pointA.y, (int)pointB.x, (int)pointB.y, color, thickness);

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            if (width < 0)
            {
                width *= -1;
                x -= width;
            }

            DrawLine(x, y, x + width, y, color);
            DrawLine(x + width - 1, y, x + width - 1, y + height, color);
            DrawLine(x, y + height - 1, x + width, y + height - 1, color);
            DrawLine(x, y, x, y + height, color);
        }

        public void DrawRectangle(Vector point, int width, int height, Color color) =>
            DrawRectangle((int)point.x, (int)point.y, width, height, color);

        public void FillRectangle(int x, int y, int width, int height, Color color)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    Draw(x + i, y + j, color);
        }

        public void FillRectangle(Vector point, int width, int height, Color color) =>
            FillRectangle((int)point.x, (int)point.y, width, height, color);

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

        public void DrawCircle(Vector point, int radius, Color color) =>
            DrawCircle((int)point.x, (int)point.y, radius, color);

        public void FillCircle(int x, int y, int radius, Color color)
        {
            for (int i = 0; i < radius * 2; i++)
            {
                for (int j = 0; j < radius * 2; j++)
                {
                    var dist = Math.Sqrt((radius - i) * (radius - i) + 
                        (radius - j) * (radius - j));
                    if (dist < radius) Draw(x - 1 + i, y - 1 + j, color);
                }
            }
        }

        public void FillCircle(Vector point, int radius, Color color) =>
            FillCircle((int)point.x, (int)point.y, radius, color);

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
            DrawLine(x1, y1, x3, y3, color);
        }

        public void DrawTriangle(Vector pointA, Vector pointB, Vector pointC, Color color) =>
            DrawTriangle((int)pointA.x, (int)pointA.y, (int)pointB.x, (int)pointB.y, 
                (int)pointC.x, (int)pointC.y, color);

        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            float Area(int x1, int y1, int x2, int y2, int x3, int y3) =>
                Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0f);

            var minX = Math.Min(Math.Min(x1, x2), x3);
            var maxX = Math.Max(Math.Max(x1, x2), x3);

            var minY = Math.Min(Math.Min(y1, y2), y3);
            var maxY = Math.Max(Math.Max(y1, y2), y3);

            float a = Area(x1, y1, x2, y2, x3, y3);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float a1 = Area(x, y, x2, y2, x3, y3);
                    float a2 = Area(x1, y1, x, y, x3, y3);
                    float a3 = Area(x1, y1, x2, y2, x, y);

                    if (a == a1 + a2 + a3)
                        Draw(x, y, color);
                }
            }
        }

        public void FillTriangle(Vector pointA, Vector pointB, Vector pointC, Color color) =>
            FillTriangle((int)pointA.x, (int)pointA.y, (int)pointB.x, (int)pointB.y, 
                (int)pointC.x, (int)pointC.y, color);

        public void DrawTexture(int x, int y, Texture texture, int scale = 1)
        {
            if (scale <= 0) return;
            for (int i = 0; i < texture.width; i++)
                for (int j = 0; j < texture.height; j++)
                    FillRectangle((x + i) * scale, (y + j) * scale,
                        scale, scale, texture.Get(i, j));
        }

        public void DrawTexture(Vector point, Texture texture, int scale = 1) =>
            DrawTexture((int)point.x, (int)point.y, texture, scale);
    }
}