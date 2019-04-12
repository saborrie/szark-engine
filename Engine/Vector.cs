using System;

namespace Szark
{
    public struct Vector
    {
        public float x, y;

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Magnitude() =>
            (float)Math.Sqrt(x * x + y * y);

        public Vector Normalized()
        {
            float mag = Magnitude();
            if (mag > 0) return new Vector(x / mag, y / mag);
            else return new Vector(x, y);
        }

        public Vector Scale(Vector b) =>
            new Vector(x * b.x, y * b.y);

        public static Vector Lerp(Vector a, Vector b, float t) =>
            new Vector((1 - t) * a.x + t * b.x, (1 - t) * a.y + t * b.y);

        public static float Distance(Vector a, Vector b) =>
            (float)Math.Sqrt((b.x - a.x) * (b.x - a.x) + 
                (b.y - a.y) * (b.y - a.y));

        public static float Dot(Vector a, Vector b) =>
            a.x * b.x + a.y * b.y;

        public static Vector operator +(Vector a, Vector b) =>
            new Vector(a.x + b.x, a.y + b.y);

        public static Vector operator -(Vector a, Vector b) =>
            new Vector(a.x - b.x, a.y - b.y);

        public static Vector operator *(Vector a, float b) =>
            new Vector(a.x * b, a.y * b);

        public static Vector operator /(Vector a, float b) =>
            new Vector(a.x / b, a.y / b);
    }
}
