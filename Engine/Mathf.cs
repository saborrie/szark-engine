namespace Szark
{
    public static class Mathf
    {
        public static float Lerp(float a, float b, float t) =>
            (1 - t) * a + t * b;

        public static float Clamp(float val, float min, float max) =>
            val < min ? min : val > max ? max : val;

        public static float Smoothstep(float edgeA, float edgeB, float x)
        {
            x = Clamp((x - edgeA) / (edgeB - edgeA), 0.0f, 1.0f);
            return x * x * (3 - 2 * x);
        }
    }
}
