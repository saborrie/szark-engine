namespace Szark
{
    public struct Transform
    {
        public Vector position;
        public float rotation, scale;
        public int layer;

        public Transform(float x, float y, float rotation = 0, float scale = 1, 
            int layer = 1) : this(new Vector(x, y), rotation, scale, layer) {}

        public Transform(Vector position, float rotation = 0, 
            float scale = 1, int layer = 1)
        {
            this.position = position;

            this.layer = layer;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}