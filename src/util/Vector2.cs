using System;

namespace Szark
{
    /// <summary>
    /// This class represents a mathematical vector
    /// of only two values. Also included is useful
    /// methods that are used in game developement.
    /// </summary>
    public partial struct Vector2
    {
        public static readonly Vector2 One = new Vector2(1, 1);
        public static readonly Vector2 Zero = new Vector2(0, 0);

        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Left = new Vector2(-1, 0);
        public static readonly Vector2 Down = new Vector2(0, -1);
        public static readonly Vector2 Up = new Vector2(0, 1);

        public float x, y;

        public Vector2(float unit) : 
            this(unit, unit) { }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // -- Public Methods --

        /// <summary>
        /// The length of this vector
        /// </summary>
        public float Magnitude() =>
            (float)Math.Sqrt(x * x + y * y);

        /// <summary>
        /// The angle of this vector as a direction
        /// </summary>
        public float Angle() =>
            (float)Math.Atan2(x, y);

        /// <summary>
        /// Returns this vector with a magnitude of 1
        /// </summary>
        public Vector2 Normalized()
        {
            float mag = Magnitude();
            return new Vector2(x, y) / (mag > 0 ? mag : 1);
        }

        /// <summary>
        /// Performs multiplication on each component
        /// of this vector based on the other vector's
        /// components.
        /// </summary>
        public Vector2 Scale(Vector2 other) =>
            new Vector2(x * other.x, y * other.y);

        // -- Static Methods --

        /// <summary>
        /// Linearly interpolates a -> b based on the time.
        /// </summary>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float time) =>
            new Vector2(Mathf.Lerp(a.x, b.x, time), Mathf.Lerp(a.y, b.y, time));

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        public static float Distance(Vector2 a, Vector2 b) =>
            (float)Math.Sqrt((b.x - a.x) * (b.x - a.x) + 
                (b.y - a.y) * (b.y - a.y));

        /// <summary>
        /// Returns the inner product of the two vectors.
        /// </summary>
        public static float Dot(Vector2 a, Vector2 b) =>
            a.x * b.x + a.y * b.y;

        // -- Overloaded Arithmetic Operators --

        public static Vector2 operator +(Vector2 a, Vector2 b) =>
            new Vector2(a.x + b.x, a.y + b.y);

        public static Vector2 operator -(Vector2 a, Vector2 b) =>
            new Vector2(a.x - b.x, a.y - b.y);

        public static Vector2 operator *(Vector2 a, float b) =>
            new Vector2(a.x * b, a.y * b);

        public static Vector2 operator /(Vector2 a, float b) =>
            new Vector2(a.x / b, a.y / b);

        public static Vector2 operator -(Vector2 a) =>
            new Vector2(-a.x, -a.y);

        // -- Overloaded Equality Operators --

        public static bool operator ==(Vector2 a, Vector2 b) =>
            a.x == b.x && a.y == b.y;

        public static bool operator !=(Vector2 a, Vector2 b) =>
            a.x != b.x || a.y != b.y;

        // -- Equals, GetHashCode, and ToString

        public override bool Equals(object obj) =>
            obj is Vector2 vector && x == vector.x && y == vector.y;

        public override int GetHashCode() =>
            HashCode.Combine(x, y);

        public override string ToString() => $"({x},{y})";
    }
}