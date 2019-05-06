using System;

namespace Szark
{
    /// <summary>
    /// This class represents a mathematical vector
    /// of only two values. Also included is useful
    /// methods that are used in game developement.
    /// </summary>
    public partial struct Vector
    {
        public float x, y;

        public Vector(float x, float y)
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
        public Vector Normalized()
        {
            float mag = Magnitude();
            return new Vector(x, y) / (mag > 0 ? mag : 1);
        }

        /// <summary>
        /// Performs multiplication on each component
        /// of this vector based on the other vector's
        /// components.
        /// </summary>
        public Vector Scale(Vector other) =>
            new Vector(x * other.x, y * other.y);

        // -- Overloaded Arithmetic Operators --

        public static Vector operator +(Vector a, Vector b) =>
            new Vector(a.x + b.x, a.y + b.y);

        public static Vector operator -(Vector a, Vector b) =>
            new Vector(a.x - b.x, a.y - b.y);

        public static Vector operator *(Vector a, float b) =>
            new Vector(a.x * b, a.y * b);

        public static Vector operator /(Vector a, float b) =>
            new Vector(a.x / b, a.y / b);

        public static Vector operator -(Vector a) =>
            new Vector(-a.x, -a.y);

        // -- Overloaded Equality Operators --

        public static bool operator ==(Vector a, Vector b) =>
            a.x == b.x && a.y == b.y;

        public static bool operator !=(Vector a, Vector b) =>
            a.x != b.x || a.y != b.y;

        // -- Equals, GetHashCode, and ToString

        public override bool Equals(object obj) =>
            obj is Vector vector && x == vector.x && y == vector.y;

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode = (hashCode * 397) ^ x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            return hashCode;
        }

        public override string ToString() => $"({x},{y})";
    }
}