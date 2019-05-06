using System;

namespace Szark
{
    public partial struct Vector
    {
        /// <summary>
        /// Linearly interpolates the vector a by vector b
        /// based on the time value.
        /// </summary>
        public static Vector Lerp(Vector a, Vector b, float time) =>
            new Vector(Mathf.Lerp(a.x, b.x, time), Mathf.Lerp(a.y, b.y, time));

        /// <summary>
        /// Returns the distance between vector a and b.
        /// </summary>
        public static float Distance(Vector a, Vector b) =>
            (float)Math.Sqrt((b.x - a.x) * (b.x - a.x) + 
                (b.y - a.y) * (b.y - a.y));

        /// <summary>
        /// Returns the inner product of the two vectors.
        /// </summary>
        public static float Dot(Vector a, Vector b) =>
            a.x * b.x + a.y * b.y;
    }
}