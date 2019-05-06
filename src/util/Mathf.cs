namespace Szark
{
    public static class Mathf
    {
        /// <summary>
        /// Interpolates from a -> b based on t
        /// </summary>
        public static float Lerp(float a, float b, float t) =>
            (1 - t) * a + t * b;

        /// <summary>
        /// Locks the given value between a min and a max
        /// </summary>
        public static float Clamp(float val, float min, float max) =>
            val < min ? min : val > max ? max : val;

        /// <summary>
        /// Smoothly curves given x between edgeA and edgeB
        /// </summary>
        public static float Smoothstep(float edgeA, float edgeB, float x)
        {
            x = Clamp((x - edgeA) / (edgeB - edgeA), 0.0f, 1.0f);
            return x * x * (3 - 2 * x);
        }
    }
}
