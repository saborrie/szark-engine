namespace Szark
{
    /// <summary>
    /// Represents a Position, Rotation, Scale, and Layer.
    /// </summary>
    public struct Transform
    {
        public Vector2 Position { get; set; }
        
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public int Layer { get; set; }

        public Transform(float x, float y, float rotation = 0, float scale = 1, 
            int layer = 1) : this(new Vector2(x, y), rotation, scale, layer) {}

        public Transform(Vector2 position, float rotation = 0, 
            float scale = 1, int layer = 1)
        {
            Position = position;
            Layer = layer;
            Rotation = rotation;
            Scale = scale;
        }
    }
}