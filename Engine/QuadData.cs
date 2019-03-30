using OpenTK.Graphics.OpenGL;

namespace Szark
{
    internal struct QuadData
    {
        public readonly static int VAO, EBO;

        private static float[] vertices =
        {
            // Vertices    // Texture Coords
             1.0f,  1.0f,  1.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
            -1.0f,  1.0f,  0.0f, 1.0f
        };

        private static int[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        static QuadData()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            int VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4,
                vertices, BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 4,
                indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 16, 8);
            GL.EnableVertexAttribArray(1);
        }
    }
}
